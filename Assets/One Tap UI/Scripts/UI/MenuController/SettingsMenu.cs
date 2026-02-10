using UnityEngine.UIElements;

namespace One_Tap_UI.UI.MenuController
{
    public class SettingsMenu
    {
        private MenuSpesific.SettingsMenu sm;
        private VisualElement tabsContainer, apply, root;
        
        public void Init(MenuSpesific.SettingsMenu sm)
        {
            this.sm = sm;
            tabsContainer = sm.tabsContainer;
            apply = sm.apply;
            root = sm.root;
        }
        
        public void ChangeVisibility()
        {
            var value = !tabsContainer.visible;
            tabsContainer.visible = value;
            apply.parent.visible = value;
            sm.CurrentVariableContainer.visible = value;
            root.visible = value;
        } 
    }
}