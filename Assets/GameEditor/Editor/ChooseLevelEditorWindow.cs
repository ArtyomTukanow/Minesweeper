using Controller.Map;
using Core;
using ModelData.TileMap;
using UnityEditor;
using UnityEngine;
using UserData.Level;

namespace GameEditor.Editor
{
    public class ChooseLevelEditorWindow : EditorWindow
    {
        [MenuItem("Editor/Choose Level")]
        private static void ShowWindow()
        {
            var window = GetWindow<ChooseLevelEditorWindow>();
            window.titleContent = new GUIContent("Choose level");
            window.Show();
        }

        private int seed;

        private void OnGUI()
        {
            if (!Game.Instance)
                return;
            
            Game.User.Level = EditorGUILayout.IntField("Уровень: ", Game.User.Level);
            seed = EditorGUILayout.IntField("Seed: ", seed);

            if (GUILayout.Button("Загрузить уровень"))
                MapController.Create(Game.User.Level, seed == default ? null : (int?) seed);

            if (GUILayout.Button("Beginner"))
                MapController.Create(MapDataType.beginner);
            
            if (GUILayout.Button("Intermediate"))
                MapController.Create(MapDataType.intermediate);
            
            if (GUILayout.Button("Expert"))
                MapController.Create(MapDataType.expert);

            Game.User.Settings.Vip = EditorGUILayout.Toggle("Vip", Game.User.Settings.Vip);
        }
    }
}