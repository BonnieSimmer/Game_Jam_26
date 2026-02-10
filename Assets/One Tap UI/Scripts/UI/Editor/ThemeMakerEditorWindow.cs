 using UnityEditor;
using UnityEngine;
using One_Tap_UI.UI.Others;

namespace One_Tap_UI.UI.Editor
{
    public class ThemeMakerEditorWindow : EditorWindow
    {
        public static ThemeMaker themeMaker;
        
        [MenuItem("Window/One Tap UI/Theme Maker")]
        public static void Open()
        {
            var window = GetWindow<ThemeMakerEditorWindow>();
            window.titleContent = new GUIContent("Theme Maker");
            var themeMakerPath = AssetDatabase.FindAssets("t:ThemeMaker");
            themeMaker = AssetDatabase.LoadAssetAtPath<ThemeMaker>(AssetDatabase.GUIDToAssetPath(themeMakerPath[0]));
        }
        
        public void OnGUI()
        {
            if (themeMaker == null)
            {
                EditorGUILayout.HelpBox("Theme Maker is missing", MessageType.Error);
                return;
            }
            
            ThemeMakerEditor.Draw(themeMaker);
        }
    }
}