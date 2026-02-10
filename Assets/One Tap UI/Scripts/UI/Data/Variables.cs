using System;
using System.Collections.Generic;
using One_Tap_UI.Audio_Controller;
using One_Tap_UI.Audio_Controller.Data;
using One_Tap_UI.Utilities;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using One_Tap_UI.UI.Others;
using System.IO;
using UnityEditor;
using One_Tap_UI.UI.MenuController;
using UnityEngine.Rendering;

namespace One_Tap_UI.UI.Data
{
    [CreateAssetMenu(fileName = "Variables", menuName = "One Tap UI/UI/Saves/Variables")]
    public class Variables : ScriptableObject
    {
        public VolumeProfile urpProfile;
        public VolumeProfile hdrpProfile;
        public OtherCustomizations otherCustomizables;
        public ThemeMaker themeMaker;
        public DisplayData displayData;
        public GraphicsData graphicsData;
        public AudioData audioData;
        public SettingsSave settingsSave;
        public MenuSpecificSettings menuSpecificSettings;
        public ThemeStyleSheet outlineTheme, containerTheme;
        public VisualTreeAsset horizontalBase,
            verticalBase,
            bottom,
            tabButton,
            variableList,
            variableDropdown,
            variableBinding,
            variableSlider,
            variableSliderInt,
            variableContainer;
        [NonSerialized] public VisualTreeAsset baseMenu;
        public VisualTreeAsset resumeMenuElement;
        [SerializeField] public List<string> menuTypes;
        [NonSerialized] public FakeEnum menuType;
        
        public void OnGenerate(VolumeControllerMono volumeControllers)
        {
            graphicsData.OnGenerate(
                otherCustomizables.renderPipeline == Enums.RenderPipeline.URP
                ? urpProfile
                : hdrpProfile);
            audioData.OnGenerate(volumeControllers);
        }
        public void SetMenuTypes(List<string> menuTypes)
        {
            this.menuTypes = menuTypes;
            menuType = new(menuTypes);
        }
    }
}