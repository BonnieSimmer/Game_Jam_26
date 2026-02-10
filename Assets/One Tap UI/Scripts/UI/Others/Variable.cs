using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Range = One_Tap_UI.Utilities.Range;

namespace One_Tap_UI.UI.Others
{
[Serializable]
public class Variable
{
    public bool isExpanded; // For the inspector
    
    public string name = "New Variable";
    public List<string> options;
    [NonSerialized] public float CurrentValue;
    public string OptionAtIndex(int index)
    {
        if (options.Count > index)
        {
            return options[index];
        }
        return options.Last();
    }

    [Serializable]
    public enum OptionType
    {
        Dropdown,
        ActionDropdown,
        Binding,
        Slider,
        Preset,
    }
    
    [Serializable]
    public enum PassVariableType
    {
        Int,
        Float,
    }
    public OptionType optionType = OptionType.Dropdown;
    public PassVariableType passVariableType = PassVariableType.Int;

    public UnityEvent<Variable> getOptionIndex;
    public UnityEvent<List<string>> getOptionList;
    public UnityEvent<int> onAssignInt;

    public Range range;
    public UnityEvent<float> onAssignFloat;

    [SerializeReference]
    public List<Variable> variablesEffectedByPreset = new List<Variable>();
    public List<string> variableNamesEffectedByPreset = new List<string>();

    public VisualElement displayedElement;

    public string reset;

    public void Execute()
    {
        if (getOptionIndex != null)
        {
            getOptionIndex.Invoke(this);
        }
        else Debug.LogWarning("OptionIndexCallback is null!");

        if (optionType == OptionType.ActionDropdown)
        {
            if (getOptionList != null)
            {
                getOptionList.Invoke(options);
            }
            else Debug.LogWarning("OptionListCallback is null!");
        }
    }
}
}