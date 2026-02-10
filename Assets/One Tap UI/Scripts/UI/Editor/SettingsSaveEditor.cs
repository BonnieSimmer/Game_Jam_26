using One_Tap_UI.UI.Data;
using UnityEditor;
using UnityEngine;

namespace OneTapUI.Editor {
    [CustomEditor(typeof(SettingsSave), true)]
    public class SettingsSaveEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorStyles.label.fontSize = 50;
            GUI.color = new Color(0.5f, 0.5f, 0.5f);
            EditorGUILayout.LabelField("?", GUILayout.MaxWidth(30), GUILayout.MaxHeight(70));
            EditorGUILayout.BeginVertical();
            EditorStyles.label.fontSize = 15;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);
            EditorGUILayout.LabelField("This file stores current game settings!");
            EditorGUILayout.LabelField("If you are using the Built-In/Legacy Input System, here is the syntax:");
            EditorGUILayout.BeginHorizontal("Box");
            EditorStyles.label.fontSize = 12;
            GUI.color = new Color(1f, 1f, 1f);
            EditorGUILayout.LabelField("Input.GetKey(settingsSave.bindings.list.<name of the button>)");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            base.OnInspectorGUI();
        }
    }
}