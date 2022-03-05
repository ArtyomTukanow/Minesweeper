using System;
using System.IO;
using Libraries.RSG;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace GameEditor.Editor
{
    public class LoadModel : UnityEditor.Editor
    {
        [MenuItem("Editor/UpdateModel", false, 1)]
        private static void SaveModelIntoResources()
        {
            try
            {
                SameModel("langs", "1Vgqkg7JvYvv7aGLy5JcwClKb6Q2U9BoRmzWqZxlVkPA");
                SameModel("products", "1Vgqkg7JvYvv7aGLy5JcwClKb6Q2U9BoRmzWqZxlVkPA");
                
                EditorUtility.DisplayDialog("Model", "Model loaded", "ok");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Model", "Model was not loaded! Error:\n" + e.Message, "ok");
                throw e;
            }

        }

        private static void SameModel(string name, string sheetName)
        {
            SendRequest($"https://opensheet.elk.sh/{sheetName}/{name}")
                .Then(text => SaveJsonAsset(name, text));
        }

        private static void SaveJsonAsset(string name, string text)
        {
            File.WriteAllText($"{Application.dataPath}/Resources/Model/{name}.json", text);
            Debug.Log($"{name}.json saved!");
        }

        private static Promise<string> SendRequest(string url)
        {
            var promise = new Promise<string>();
            
            var request = new UnityWebRequest
            {
                url = url,
                method = UnityWebRequest.kHttpVerbPOST,
                downloadHandler = new DownloadHandlerBuffer(),
            };

            request.SendWebRequest().completed += operation =>
            {
                promise.Resolve(request.downloadHandler.text);
            };

            return promise;
        }
    }
}