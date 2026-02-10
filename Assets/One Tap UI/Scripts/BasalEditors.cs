using UnityEditor;
using One_Tap_UI.UI.Data;
using One_Tap_UI.Audio_Controller.Data;
using One_Tap_UI.Utilities;
using One_Tap_UI.UI.MenuController;

namespace One_Tap_UI.Scripts
{
    [CustomEditor(typeof(OtherCustomizations))]
    class OtherCustomizationsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(DisplayData), true)]
    class DisplayDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(GraphicsData), true)]
    class GraphicsDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(AudioData), true)]
    class AudioDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(SoundLister), true)]
    class SoundListerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(ResumeMenu), true)]
    class ResumeMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}