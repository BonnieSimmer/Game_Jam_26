using System;
using System.Collections.Generic;
using One_Tap_UI.Audio_Controller;
using One_Tap_UI.UI.Data;
using One_Tap_UI.UI.MenuSpesific;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;

namespace One_Tap_UI.UI.Editor
{
    [CustomEditor(typeof(SettingsMenu), true)]
    public class SettingsMenuEditor : UnityEditor.Editor
    {
        private SettingsMenu generateSettingsMenu;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            generateSettingsMenu = (SettingsMenu) target;
            
            generateSettingsMenu.variables = (Variables) EditorGUILayout.ObjectField("Variables", generateSettingsMenu.variables, typeof(Variables), false);
            generateSettingsMenu.postProcessLayer = (PostProcessLayer) EditorGUILayout.ObjectField("Post Process Layer", generateSettingsMenu.postProcessLayer, typeof(PostProcessLayer), true);
            generateSettingsMenu.volumeControllers = (VolumeControllerMono) EditorGUILayout.ObjectField("Volume Controllers", generateSettingsMenu.volumeControllers, typeof(VolumeControllerMono), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("approvalPopupHandler.document"));
        }
    }
}