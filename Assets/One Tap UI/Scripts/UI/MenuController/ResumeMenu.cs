using One_Tap_UI.UI.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace One_Tap_UI.UI.MenuController
{
    public class ResumeMenu
    {
        [SerializeField] private Variables variables;
        public UnityEngine.InputSystem.PlayerInput playerInput;
        private VisualElement root;
        
        public void Init(MenuSpesific.ResumeMenu msResumeMenu, UnityEngine.InputSystem.PlayerInput playerInput)
        {
            variables = msResumeMenu.variables;
            this.playerInput = playerInput;
            root = msResumeMenu.uiDocument.rootVisualElement;
        }

        public void ChangeVisibility()
        {
            ChangeVisibility(!root.visible);
        }

        private void ChangeVisibility(bool value)
        {
            root.visible = value;
        }
        
        public void ChangeInputSystem()
        {
            if (variables.otherCustomizables.inputSystem != Enums.InputSystem.InputSystemPackage) return;
            var actionMap = playerInput.currentActionMap.name == "UI" ? "Default" : "UI";
            playerInput.SwitchCurrentActionMap(actionMap);
        }
    }
}