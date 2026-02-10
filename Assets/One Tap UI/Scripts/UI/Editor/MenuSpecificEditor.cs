using System;
using System.Collections.Generic;
using One_Tap_UI.Audio_Controller;
using One_Tap_UI.UI.Data;
using One_Tap_UI.UI.MenuSpesific;
using One_Tap_UI.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using One_Tap_UI.UI.Others;
using One_Tap_UI.UI.MenuController;

namespace One_Tap_UI.UI.Editor
{
    [CustomEditor(typeof(MenuSpecificSettings), true)]
    public class MenuSpecificEditor : UnityEditor.Editor
    {
        private MenuSpecificSettings self;
        private bool isTabsExpanded;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            self = (MenuSpecificSettings) target;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("menuNames"));

            EditorGUILayout.BeginVertical("Box");
            isTabsExpanded = EditorGUILayout.Foldout(isTabsExpanded, "Tabs");
            if (isTabsExpanded)
            {
                SerializeTabList(self.tabs);
            }
            EditorGUILayout.EndVertical();
            
            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(self);
                AssetDatabase.SaveAssets();
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        private void SerializeTabList(IList<Tab> tabs)
        {
            if (GUILayout.Button("Add Tab"))
            {
                tabs.Add(new Tab());
            }
            
            if (tabs.Count == 0) return;
            for (var i = 0; i < tabs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                tabs[i].isExpanded = EditorGUILayout.Foldout(tabs[i].isExpanded,"");
                tabs[i].name = EditorGUILayout.TextField(tabs[i].name);
                if (i != 0 && GUILayout.Button("Up"))
                {
                    tabs.Insert(i - 1, tabs[i]);
                    tabs.RemoveAt(i + 1);
                }
                if (i + 1 != tabs.Count && GUILayout.Button("Down"))
                {
                    tabs.Insert(i + 2, tabs[i]);
                    tabs.RemoveAt(i);
                    return;
                }
                if (GUILayout.Button("Remove"))
                {
                    tabs.RemoveAt(i);
                    return;
                }
                
                var variable = tabs[i].variables;
                if (GUILayout.Button("Add Variable"))
                {
                    variable.Add(new Variable());
                }
                
                EditorGUILayout.EndHorizontal();
                
                if (tabs[i].isExpanded)
                {
                    EditorGUILayout.BeginVertical("Box");
                    SerializeVariableList(tabs[i].variables, i);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();
            }
        }

        private void SerializeVariableList(IList<Variable> variables, int index)
        {
            if (variables.Count == 0) return;
            for (var i = 0; i < variables.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (variables[i].optionType != Variable.OptionType.Binding)
                {
                    variables[i].isExpanded = EditorGUILayout.Foldout(variables[i].isExpanded, "");
                }
                variables[i].name = EditorGUILayout.TextField(variables[i].name);
                if (i != 0 && GUILayout.Button("Up"))
                {
                    variables.Insert(i - 1, variables[i]);
                    variables.RemoveAt(i + 1);
                    return;
                }
                if (i + 1 != variables.Count && GUILayout.Button("Down"))
                {
                    variables.Insert(i + 2, variables[i]);
                    variables.RemoveAt(i);
                    return;
                }
                if (GUILayout.Button("Remove"))
                {
                    variables.RemoveAt(i);
                    return;
                }

                if (variables[i].optionType is Variable.OptionType.Dropdown or Variable.OptionType.Preset)
                {
                    if (GUILayout.Button("Add Option"))
                    {
                        variables[i].options.Add("New Option");
                    }
                }
                
                var name = variables[i].optionType switch
                {
                    Variable.OptionType.Dropdown => "Dropdown",
                    Variable.OptionType.ActionDropdown => "Action Dropdown",
                    Variable.OptionType.Binding => "Binding",
                    Variable.OptionType.Slider => "Slider",
                    Variable.OptionType.Preset => "Preset",
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                if (GUILayout.Button(name))
                {
                    variables[i].optionType = variables[i].optionType.Next();
                }

                if (variables[i].optionType == Variable.OptionType.Slider)
                {
                    var passVariableType = variables[i].passVariableType.ToString();
                    if (GUILayout.Button(passVariableType))
                    {
                        variables[i].passVariableType = variables[i].passVariableType.Next();
                    }
                }
                
                EditorGUILayout.EndHorizontal();

                if (variables[i].isExpanded)
                {
                    EditorGUILayout.BeginVertical("Box");
                    SerializeVariable(variables, i, index);
                    EditorGUILayout.EndVertical();
                }
            }
        }
        
        private void SerializeVariable(IList<Variable> variable, int i, int index)
        {
            variable[i].getOptionIndex = self.getOptionIndex;

            if (variable[i].optionType == Variable.OptionType.Dropdown)
            {
                SerializeOptionList(variable[i].options);
            }
            else if (variable[i].optionType == Variable.OptionType.ActionDropdown)
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty($"tabs.Array.data[{index}].variables.Array.data[{i}].getOptionList"));
            }
            else if (variable[i].optionType == Variable.OptionType.Preset)
            {
                SerializeOptionList(variable[i].options);
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty($"tabs.Array.data[{index}].variables.Array.data[{i}].variableNamesEffectedByPreset"));
            }

            if (variable[i].optionType != Variable.OptionType.Binding)
            {
                if (variable[i].optionType == Variable.OptionType.Slider)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Range:");
                    variable[i].range.min = EditorGUILayout.FloatField(variable[i].range.min);
                    variable[i].range.max = EditorGUILayout.FloatField(variable[i].range.max);
                    EditorGUILayout.EndHorizontal();
                }
                    
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        $"tabs.Array.data[{index}].variables.Array.data[{i}].onAssign{variable[i].passVariableType}"));
            }
            EditorGUILayout.Space();
        }
        
        private void SerializeOptionList(IList<string> options)
        {
            if (options.Count == 0) return;
            for (var i = 0; i < options.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                options[i] = EditorGUILayout.TextField("Option:", options[i]);
                
                if (i != 0 && GUILayout.Button("Up"))
                {
                    options.Insert(i - 1, options[i]);
                    options.RemoveAt(i + 1);
                    return;
                }
                if (i + 1 != options.Count && GUILayout.Button("Down"))
                {
                    options.Insert(i + 2, options[i]);
                    options.RemoveAt(i);
                    return;
                }
                if (GUILayout.Button("Remove"))
                {
                    options.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(self);
                AssetDatabase.SaveAssets();
            }
        }
    }
}