using System.Collections.Generic;
using One_Tap_UI.UI.Data;
using One_Tap_UI.UI.Others;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace One_Tap_UI.UI.MenuController
{
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName = "Menu Specific Settings", menuName = "One Tap UI/UI/Menu Specific Settings")]
    public class MenuSpecificSettings : ScriptableObject
    {
        public List<string> menuNames;
        private static Variables Variables;

        [SerializeField] public List<Tab> tabs = new();
        public UnityEvent<Variable> getOptionIndex;

        public void OnValidate() {
            if (Variables == null || Variables.menuTypes == null) return;
            Variables.SetMenuTypes(menuNames);
        }
    }
}
