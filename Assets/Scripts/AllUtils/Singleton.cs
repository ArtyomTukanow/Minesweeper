using UnityEngine;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance => instance ??= CreateInstance();

        private static T CreateInstance()
        {
            GameObject singleton = new GameObject();
            var inst = singleton.AddComponent<T>();
            singleton.name = typeof(T).ToString();

            DontDestroyOnLoad(singleton);
            return inst;
        }
    }
}