using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Range = One_Tap_UI.Utilities.Range;

namespace One_Tap_UI.UI.Others
{
    /// <summary>
    /// It contains all the variables and methods needed to create a tab
    /// </summary>
    [Serializable]
    public class Tab
    {
        public bool isExpanded; // For the inspector
        
        public string name;
        [SerializeField] public List<Variable> variables = new();

        
    }
        
}