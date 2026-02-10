using One_Tap_UI.UI.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace One_Tap_UI.UI.MenuSpesific
{
    [RequireComponent(typeof(UIDocument))]
    public class ResumeMenu : MonoBehaviour
    {
        public Variables variables;
        public UIDocument uiDocument;
        public MenuController.MenuController menuController;
        private VisualElement container, root;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;
            container = root.Q<VisualElement>("Container");
        }

    
        public void GenerateAll()
        {
            container.Clear();
            GenerateElements();
        }
    
        private void GenerateElements()
        {
            if (variables.menuTypes == null) return;
            for (int i = 1, count = variables.menuTypes.Count; i < count; i++)
            {
                container.Add(variables.resumeMenuElement.CloneTree());
                var button = container[i - 1].Q<Button>();
                var menuType = variables.menuTypes[i];
                button.text = menuType;
                button.clickable.clicked += () => {
                    variables.menuType.Set(menuType);
                    menuController.ChangeMenu();
                };
            }
        }
    }
}