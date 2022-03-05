using System;
using System.Reflection;

namespace AllUtils.MergeUtil
{
    public static class MergeUtil
    {
        public static T Merge<T>(this T original, T other)
        {
            if (original == null)
                return other;
            
            FieldInfo[] fields = original.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties = original.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            bool wasMergeAttributes = false;
            
            var parameters = GetParameters(fields, properties);
            foreach (var param in parameters)
            {
                MergeAttribute mergeAttribute = param.GetMergeAttribute();
                if (mergeAttribute == null)
                    continue;

                wasMergeAttributes = true;

                switch (mergeAttribute.Type)
                {
                    case MergeType.UPDATE:
                        UpdateProperty(param, original, other);
                        break;
                    case MergeType.REWRITE:
                        RewriteProperty(param, original, other);
                        break;
                    default:
                        throw new MergeException($"Unknown MergeType: {mergeAttribute.Type} for {original.GetType().Name}.{param.GetName()}");
                }
            }

            if (!wasMergeAttributes)
                ThrowIfWasNotMerge(original.GetType().Name);
                
            return original;
        }

        private static void ThrowIfWasNotMerge(string className)
        {
            throw new MergeException($"Nothing to merge in {className} class." +
                                     "\n1) Use [Merge] Attribute in class." +
                                     "\n2) Check if field has readonly mark." +
                                     "\n3) Check if property has private set access." +
                                     "\n4) Change UPDATE type to REWRITE in parent class.");
        }

        private static void MergeAsLists(this System.Collections.IList original, System.Collections.IList other)
        {
            UpdateLists(original, other);
        }
        private static void MergeAsDictionaries(this System.Collections.IDictionary original, System.Collections.IDictionary other)
        {
            UpdateDictionary(original, other);
        }

        private static ParamDecorator[] GetParameters(FieldInfo[] fields, PropertyInfo[] properties)
        {
            ParamDecorator[] fieldsProperties = new ParamDecorator[fields.Length + properties.Length];
            
            for (int i = 0; i < fields.Length; i++)
                fieldsProperties[i] = new ParamDecorator(fields[i]);
            
            for (int i = 0; i < properties.Length; i++)
                fieldsProperties[fields.Length + i] = new ParamDecorator(properties[i]);

            return fieldsProperties;
        }

        private static bool IsDefault(this object obj)
        {
            if (obj == null)
                return true;
            if (obj.GetType().IsValueType)
                return obj.Equals(Activator.CreateInstance(obj.GetType()));
            return false;
        }

        private static void UpdateProperty(ParamDecorator param, object original, object other)
        {
            var originalValue = param.GetValue(original);
            var otherValue = param.GetValue(other);

            if (otherValue.IsDefault())
                return;
            
            if (originalValue.IsDefault())
                RewritePropertyByValue(param, original, otherValue);
            else if (originalValue is System.Collections.IList originalList && otherValue is System.Collections.IList otherList)
                UpdateLists(originalList, otherList);
            else if (originalValue is System.Collections.IDictionary originalDict && otherValue is System.Collections.IDictionary otherDict)
                UpdateDictionary(originalDict, otherDict);
            else if (originalValue is string && otherValue is string otherString)
                UpdateString(param, original, otherString);
            else if (originalValue.GetType().IsPrimitive && otherValue.GetType().IsPrimitive)
                RewritePropertyByValue(param, original, otherValue);
            else
                InnerUpdateProperty(param, original, other);
                //RewriteProperty(param, original, other);
        }


        private static void UpdateLists(System.Collections.IList original, System.Collections.IList other)
        {
            if (original == null || other == null || original.Equals(other)) 
                return;

            int originalCount = original.Count;
            int otherCount = other.Count;

            //Первый проход, удаляем ненужные позиции, которых нет в новом списке
            if (otherCount == 0)
                original.Clear();
            else
                while (otherCount < original.Count)
                    original.RemoveAt(otherCount - 1);

            //Добавляем новые записи в конец списка
            for (int i = originalCount; i < otherCount; i++)
                original.Add(other[i]);
            
            object originalElement;
            object otherElement;
            
            int maxMergeElement = Math.Min(originalCount, otherCount);                        
            for (int i = 0; i < maxMergeElement; i++)
            {
                /* Если рекурсовное слияние и оба объекта в списке являются классами и не пустые, то сливаем
                 * Иначе просто применяем новый обеъкт к старому списку
                */
                originalElement = original[i];
                otherElement = other[i];
                if (originalElement != null && otherElement != null
                    && !(originalElement is string) && !(otherElement is string)
                    && originalElement.GetType().IsClass && otherElement.GetType().IsClass)
                {
                    if (originalElement is System.Collections.IList originalList &&
                        otherElement is System.Collections.IList otherList)
                        originalList.MergeAsLists(otherList);
                    else if (originalElement is System.Collections.IDictionary originalDict &&
                        otherElement is System.Collections.IDictionary otherDict)
                        originalDict.MergeAsDictionaries(otherDict);
                    else
                        original[i].Merge(otherElement);
                }
                else
                    original[i] = otherElement;
            }
        }

        private static void UpdateDictionary(System.Collections.IDictionary original, System.Collections.IDictionary other)
        {
            if (original == null || other == null || original.Equals(other)) 
                return;

            //Первый проход, удаляем ненужные ключи, которых нет в новом словаре
            var keys = original.Keys;
            object[] keyArray = new object[keys.Count];
            keys.CopyTo(keyArray, 0);

            foreach (object key in keyArray)
                if (!other.Contains(key))
                    original.Remove(key);

            object originalElement;
            object otherElement;
            
            //Второй проход, сливаем значения в словарях
            foreach (object key in other.Keys)
                if (original.Contains(key))
                {
                    /* Если рекурсовное слияние и оба объекта под ключом в словарях являются классами и не пустые, то сливаем
                     * Иначе просто применяем новый обеъкт к старому словарю
                     */
                    originalElement = original[key];
                    otherElement = other[key];
                    
                    if (originalElement != null && otherElement != null
                        && !(originalElement is string) && !(otherElement is string)
                        && originalElement.GetType().IsClass && otherElement.GetType().IsClass)
                    {
                        if (originalElement is System.Collections.IList originalList &&
                            otherElement is System.Collections.IList otherList)
                            originalList.MergeAsLists(otherList);
                        else if (originalElement is System.Collections.IDictionary originalDict &&
                            otherElement is System.Collections.IDictionary otherDict)
                            originalDict.MergeAsDictionaries(otherDict);
                        else
                            original[key].Merge(other[key]);
                    }
                    else
                        original[key] = other[key];
                }
                else
                    original.Add(key, other[key]);
        }

        private static void UpdateString(ParamDecorator param, object originalObject, string otherValue)
        {
            RewritePropertyByValue(param, originalObject, otherValue);
        }

        private static void RewriteProperty(ParamDecorator param, object original, object other)
        {
            RewritePropertyByValue(param, original, param.GetValue(other));
        }

        private static void RewritePropertyByValue(ParamDecorator param, object original, object otherValue)
        {
            try
            {
                param.SetValue(original, otherValue);
            }
            catch (Exception e)
            {
                throw new MergeException($"Can't set parameter: {original.GetType().Name}.{param.GetName()}. {e.Message}");
            }
        }

        private static void InnerUpdateProperty(ParamDecorator param, object original, object other)
        {
            param.GetValue(original).Merge(param.GetValue(other));
        }
    }
    
    public enum MergeType
    {
        /*Полностью перезаписать*/ REWRITE,
        /*Удаляем лишние старые, обновляем текущие, добавляем новые*/ UPDATE,
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MergeAttribute : Attribute
    {
        public MergeType Type;

        public MergeAttribute(MergeType mergeType)
        {
            Type = mergeType;
        }
    }

    internal struct ParamDecorator
    {
        private readonly FieldInfo _field;
        private readonly PropertyInfo _property;

        public ParamDecorator(FieldInfo field)
        {
            _field = field;
            _property = null;
        }

        public ParamDecorator(PropertyInfo property)
        {
            _property = property;
            _field = null;
        }

        public string GetName()
        {
            if (_field != null)
                return _field.Name;
            else
                return _property.Name;
        }

        public object GetValue(object obj)
        {
            if (_field != null)
                return _field.GetValue(obj);
            else
                return _property.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (_field != null)
                _field.SetValue(obj, value);
            else
                _property.SetValue(obj, value);
        }

        public MergeAttribute GetMergeAttribute()
        {
            if (_field != null)
                return _field.GetCustomAttribute<MergeAttribute>();
            else
                return _property.GetCustomAttribute<MergeAttribute>();
        }
    }
}
