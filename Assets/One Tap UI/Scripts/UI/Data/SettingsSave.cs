using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using One_Tap_UI.UI.Others;

namespace One_Tap_UI.UI.Data
{
    /// <summary>
    /// <para> This class is used to save and load settings. </para>
    /// </summary>
    [CreateAssetMenu(fileName = "Settings Save", menuName = "One Tap UI/UI/Settings Save")]
    public class SettingsSave : ScriptableObject
    {
        [Serializable]
        public class Settings
        {
            [Serializable]
            public struct Setting
            {
                public string name;
                public float value;

                public void SetIndex(int index)
                {
                    value = index;
                }
            }

            public List<Setting> list = new List<Setting>();

            public float this[string key]
            {
                get => list.Find(x => x.name == key).value;
                set
                {
                    var i = list.FindIndex(x => x.name == key);
                    try
                    {
                        var setting = list[i];
                        setting.value = value;
                        list[i] = setting;
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Setting {key} not found");
                    }
                }
            }
        }
        
        /// <summary>
        /// <para> A class that holds the data of all the bindings. </para>
        /// </summary>
        [Serializable]
        public class Bindings
        {
            /// <summary>
            /// <para> A class that holds the data of a single binding. </para>
            /// </summary>
            [Serializable]
            public class Bind
            {
                public string name;
                public KeyCode key, previousKey;
                
                
                public void Set(string key)
                {
                    this.key = (KeyCode) Enum.Parse(typeof(KeyCode), key);
                }
                public void Set(KeyCode key)
                {
                    this.key = key;
                }
                
                
                public KeyControl ToKeyControl(string key)
                {
                    return (KeyControl) Enum.Parse(typeof(KeyControl), key);
                }
            }

            public List<Bind> list = new List<Bind>();
            
            public Bind this[string key]
            {
                get => list.Find(x => x.name == key);
                set
                {
                    var i = list.FindIndex(x => x.name == key);
                    var bind = list[i];
                    bind.key = value.key;
                    list[i] = bind;
                }
            }
        }

        public Settings settings = new();
        public Bindings bindings = new();

        /// <summary>
        /// <para> Sets the value of a setting the correct one. </para>
        /// </summary>
        /// <param name="variable"> The variable to set. </param>
        public void GetValue(Variable variable)
        {
            variable.CurrentValue = (int)settings[variable.name];
        }
    }
}