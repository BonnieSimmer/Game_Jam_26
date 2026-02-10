using System.Linq;
using UnityEditor;
using UnityEngine;
using One_Tap_UI.UI.Others;

namespace One_Tap_UI.UI.Editor
{
    [CustomEditor(typeof(ThemeMaker), true)]
    public class ThemeMakerEditor : UnityEditor.Editor
    {
        private static FilePopup filePopup;
        
        public override void OnInspectorGUI()
        {
            var themeMaker = (ThemeMaker) target;
            Draw(themeMaker);
        }

        public static void Draw(ThemeMaker themeMaker)
        {
            EditorGUILayout.BeginVertical("Box");
            themeMaker.tabDirection = (Enums.TabDirection) EditorGUILayout.EnumPopup("Tab Direction", themeMaker.tabDirection);
            themeMaker.Layout = (Enums.Layout) EditorGUILayout.EnumPopup("Background Type", themeMaker.Layout);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(20);
            
            EditorGUILayout.BeginVertical("Box");
            if (GUILayout.Button("Change Files"))
            {
                filePopup = new FilePopup {themeMaker = themeMaker};
                PopupWindow.Show(new Rect(20, 0, 200, 200), filePopup);
            }
            if (string.IsNullOrEmpty(themeMaker.paths.JsonPath))
            {
                Error("No json file selected");
                return;
            }
            
            if (GUILayout.Button("Read Colors"))
            {
                themeMaker.ReadColors(themeMaker.paths.JsonPath);
            }
            
            if (GUILayout.Button("Overwrite And Refresh"))
            {
                if (themeMaker.paths.ussName.Length == 0)
                {
                    Error("No USS file selected at: " + themeMaker.paths.USSPath);
                    return;
                }
                if (themeMaker.colorDatas.Count == 0)
                {
                    Error("No colors found at: " + themeMaker.paths.JsonPath);
                    return;
                }
                themeMaker.Overwrite();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(20);
            
            if (string.IsNullOrEmpty(themeMaker.paths.ussName))
            {
                Error("No colors found at: " + themeMaker.paths.JsonPath, false);
                return;
            }
            
            if (themeMaker.colorDatas.Count == 0)
            {
                AssetDatabase.Refresh(); // to avoid the error message
                Error("No colors found at: " + themeMaker.paths.JsonPath, false);
                return;
            }
            
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel, GUILayout.MaxWidth(Screen.width));
            if (themeMaker.colorDatas.Any(colorDatas => colorDatas == null || colorDatas.Count == 0))
            {
                Error("Color is null");
                return;
            }
            for (int i = 0, count = themeMaker.colorDatas.Count; i < count; i++)
            {
                var colorDatas = themeMaker.colorDatas[i];
                var colorData = colorDatas[0];
                if (colorData == null)
                {
                    Error("Color is null");
                    return;
                }
                var newHueColor = EditorGUILayout.ColorField(colorData.name, colorData.tintColor);
                if (newHueColor != colorData.tintColor)
                {
                    for (int j = 0, count2 = themeMaker.colorDatas[i].Count; j < count2; j++)
                    {
                        colorDatas[j].SetTintColor(newHueColor);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        private static void Error(string message, bool endVertical = true)
        {
            if (endVertical) EditorGUILayout.EndVertical();
            EditorGUILayout.HelpBox(message, MessageType.Error);
        }

        private void OnEnable()
        {
            var themeMaker = (ThemeMaker) target;
            if (themeMaker.colorDatas.Count == 0)
            {
                themeMaker.InitializeColors();
            }
        }
        private class FilePopup : PopupWindowContent
        {
            public ThemeMaker themeMaker;
            
            public override void OnGUI(Rect rect)
            {
                EditorGUILayout.BeginVertical("Box");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel, GUILayout.MaxWidth(50));
                themeMaker.paths.jsonName = EditorGUILayout.TextField(themeMaker.paths.jsonName);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("USS", EditorStyles.boldLabel, GUILayout.MaxWidth(50));
                themeMaker.paths.ussName = EditorGUILayout.TextField(themeMaker.paths.ussName);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                
                if (GUILayout.Button("Done"))
                {
                    themeMaker.SavePaths();
                    editorWindow.Close();
                }
            }
        }
    }
}