﻿using UnityEngine;

namespace Assets.Scripts.UI.Utils
{
    public static class VectorUtils
    {
        public static Vector3 ToVector3(this Vector2 vec) => new Vector3(vec.x, vec.y);

        public static Vector3 ToVector3(this float f) => new Vector3(f, f, f);

        public static Vector2 ToVector2(this Vector3 vec) => new Vector2(vec.x, vec.y);


        // То же что Mathf.Clamp но для векторов
        public static Vector3 Clamp(this Vector3 vector, Vector3 minBounds, Vector3 maxBounds)
        {
            var x = Mathf.Clamp(vector.x, minBounds.x, maxBounds.x);
            var y = Mathf.Clamp(vector.y, minBounds.y, maxBounds.y);
            var z = Mathf.Clamp(vector.z, minBounds.z, maxBounds.z);
            return new Vector3(x, y, z);
        }

        // То же что Mathf.Clamp но для векторов
        public static Vector2 Clamp(this Vector2 vector, Vector2 minBounds, Vector2 maxBounds)
        {
            var x = Mathf.Clamp(vector.x, minBounds.x, maxBounds.x);
            var y = Mathf.Clamp(vector.y, minBounds.y, maxBounds.y);
            return new Vector2(x, y);
        }


        /// <summary>
        /// Заменяет компоненты вектора на заданные если они есть (число) или не меняет если их нет (null)
        /// </summary>
        /// <returns> Новый вектор с нужными значениями </returns>
        public static Vector3 Set(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);

        public static Vector2 Set(this Vector2 v, float? x = null, float? y = null) =>
            new Vector2(x ?? v.x, y ?? v.y);

        /// <summary>
        /// Прибавляет к компонентам вектора что-то если они есть (число) или не меняет если их нет (null)
        /// </summary>
        /// <returns> Новый вектор с нужными значениями </returns>
        public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new Vector3(v.x + (x ?? 0), v.y + (y ?? 0), v.z + (z ?? 0));

        public static Vector2 Add(this Vector2 v, float? x = null, float? y = null) =>
            new Vector2(v.x + (x ?? 0), v.y + (y ?? 0));

        public static Vector3 MultiplyElementWise(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new Vector3(x == null ? v.x : v.x * x.Value, 
                        y == null ? v.y : v.y * y.Value, 
                        z == null ? v.z : v.z * z.Value);


        public static bool IsInside(this Vector2 vec, Vector2 min, Vector2 max) =>
            vec.x.Between(min.x, max.x) && vec.y.Between(min.x, max.x);

        public static bool IsInside(this Vector2 vec, Rect rect) =>
            vec.x.Between(rect.xMin, rect.xMax) && vec.y.Between(rect.yMin, rect.yMax);

        public static bool IsInside(this Vector3 vec, Vector3 min, Vector3 max) =>
            vec.x.Between(min.x, max.x) && vec.y.Between(min.x, max.x) && vec.z.Between(min.z, max.z);


        public static Vector3Int ToVector3Int(this Vector2Int vec, int z = 0) => new Vector3Int(vec.x, vec.y, z);

        public static bool InScreenRect(this Vector2 vector) => (vector.x >= 0 && vector.x <= Screen.width) &&
                                                                (vector.y >= 0 && vector.y <= Screen.height);

        public static bool InScreenRect(this Vector3 vector) => InScreenRect(vector.ToVector2());
    }
}