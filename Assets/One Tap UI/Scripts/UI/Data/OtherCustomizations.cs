using UnityEngine;
using Enums = One_Tap_UI.UI.Enums;

namespace One_Tap_UI.UI.Data {
    [CreateAssetMenu(fileName = "Other Customizations", menuName = "One Tap UI/UI/Other Customizations")]
    public class OtherCustomizations : ScriptableObject
    {
        public Enums.InputSystem inputSystem;
        public Enums.RenderPipeline renderPipeline;
        public float blurValue;
        public float blurDuration;
        public float tabButtonHeight;
    }
}