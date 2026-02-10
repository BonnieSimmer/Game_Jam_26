using System.Linq;
using One_Tap_UI.Audio_Controller.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace One_Tap_UI.Audio_Controller
{
    [CustomEditor(typeof(VolumeControllerObject)), CanEditMultipleObjects]
    public class VolumeControllerWindow : Editor
    {
        private VisualTreeAsset baseUI, tagUI;
        private VolumeControllerMono volumeController;
        
        public override VisualElement CreateInspectorGUI()
        {
            baseUI = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/One Tap UI/Audio Controller/UI Toolkit/Volume Controller.uxml");
            var root = baseUI.CloneTree();
            InspectorObjectToEdit(root);
            return root;
        }

        private Color Red = new(.5f, 0f, 0f, 1f), Green = new(0f, .5f, 0f, 1f);

        private void InspectorObjectToEdit(VisualElement root)
        {
            var container = root.Q<VisualElement>("MonoContainer");
            var objField = new ObjectField("Volume Controller")
            {
                value = volumeController,
                objectType = typeof(VolumeControllerMono)
            };
            var existance = container.Q<Label>("Existance");
            existance.style.color = Color.white;
            CallInspector(root, existance);
            objField.RegisterValueChangedCallback(evt => {
                volumeController = (VolumeControllerMono)evt.newValue;
                CallInspector(root, existance);
            });
            container.Q<VisualElement>("Stretch").Add(objField);
        }

        private void CallInspector(VisualElement root, Label existance)
        {
            if (volumeController != null)
            {
                existance.text = "✓";
                existance.style.backgroundColor = Green;
                InspectorFillAudioData(root);
                return;
            }
            existance.text = "✗";
            existance.style.backgroundColor = Red;
            InspectorDeleteAudioData(root);
        }

        private void InspectorFillAudioData(VisualElement root)
        {
            var container = root.Q<VisualElement>("AudioDataContainer");
            container.Q<VisualElement>("Stretch").Clear();
            var objField = new ObjectField("Audio Data")
            {
                value = volumeController.audioData,
                objectType = typeof(AudioData)
            };
            objField.RegisterValueChangedCallback(evt => volumeController.audioData = (AudioData)evt.newValue);
            container.Q<VisualElement>("Stretch").Add(objField);
            
            var get = root.Q<Button>("Get");
            var count = container.Q<Label>("Count");
            get.clicked += () =>
            {
                var acs = volumeController.GetAudioSources();
                volumeController.list.Clear();
                foreach (var tag in volumeController.audioData.audioTags)
                {
                    volumeController.list.Add(new VolumeController(tag));
                    volumeController.list.Last().Get(acs);
                }
                count.text = volumeController.list.Count.ToString();
            };
            count.text = volumeController.list.Count.ToString();
        }

        private void InspectorDeleteAudioData(VisualElement root)
        {
            var container = root.Q<VisualElement>("AudioDataContainer");
            container.Q<VisualElement>("Stretch").Clear();
            container.Q<Label>("Count").text = "0";
            var audioData = container.Q<ObjectField>("Audio Data");
            if (audioData != null)
            {
                audioData.value = null;
                audioData.MarkDirtyRepaint();
            }
            
            // add error message to container
            var error = new Label("Volume Controller is not set.");
            error.style.color = Color.red;
            error.style.unityFontStyleAndWeight = FontStyle.Italic;
            error.style.fontSize = 24;
            container.Q<VisualElement>("Stretch").Add(error);
        }
    }
}