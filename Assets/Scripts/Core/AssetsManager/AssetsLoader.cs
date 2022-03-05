using Libraries.RSG;
using UnityEngine;

namespace Core.AssetsManager
{
    public class AssetsLoader
    {
        public static T CreateSync<T>(string path, Transform parent = null) where T : Object
        {
            var prefab = LoadSync<T>(path);
            
            if (!prefab) 
                return null;
            
            return Object.Instantiate(prefab, parent);
        }

        public static IPromise<T> CreateAsync<T>(string path, Transform parent = null) where T : Object
        {
            return LoadAsync<T>(path)
                .Then(prefab => Promise<T>.Resolved(Object.Instantiate(prefab, parent)));
        }
        
        
        
        public static T LoadSync<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public static IPromise<T> LoadAsync<T>(string path) where T : Object
        {
            return Promise<T>.Resolved(LoadSync<T>(path));
        }
    }
}