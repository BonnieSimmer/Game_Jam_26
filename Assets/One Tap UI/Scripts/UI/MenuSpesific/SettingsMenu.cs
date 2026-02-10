using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using One_Tap_UI.Audio_Controller;
using One_Tap_UI.UI.Data;
using One_Tap_UI.UI.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using One_Tap_UI.UI.Others;
using Tab = One_Tap_UI.UI.Others.Tab;
using UnityEngine.UI;
using Slider = UnityEngine.UIElements.Slider;
using Button = UnityEngine.UIElements.Button;

namespace One_Tap_UI.UI.MenuSpesific
{
    [RequireComponent(typeof(UIDocument))]
    public class SettingsMenu : MonoBehaviour
    {
        [NonSerialized] public List<Tab> tabs;
        [NonSerialized] public int currentTab;

        public Variables variables;
        
        [SerializeField] public PostProcessLayer postProcessLayer;
        [SerializeField] public VolumeControllerMono volumeControllers;
        
        [NonSerialized] public Enums.InputSystem inputSystem;
        
        public UnityEvent<Variable> getOptionIndex;
        
        public Action<bool> onApply;
        public Button apply, bottom;

        private int[] keyCodes;

        private VisualElement mainContainer;
        public VisualElement tabsContainer, root;
        private UIDocument uiDocument;

        public VisualElement CurrentVariableContainer => root.Query<ListView>("Variables").AtIndex(currentTab);

        public ApprovalPopupHandler approvalPopupHandler = new ApprovalPopupHandler();

        private Action assignPresets;

        private Variable preset;

        private void Awake()
        {
            variables.baseMenu = variables.themeMaker.tabDirection == TabDirection.Horizontal ? variables.horizontalBase : variables.verticalBase;

            tabs = variables.menuSpecificSettings.tabs;
            uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement.Q("Root");
            root.Add(variables.baseMenu.CloneTree());
            root.Children().First().style.height = new Length(100, LengthUnit.Percent);
            root.Add(variables.bottom.CloneTree());
            apply = root.Q<Button>("Apply");
            bottom = root.Q<Button>("Back");
            root = root.Q($"{variables.themeMaker.tabDirection.ToString()}Base");
            mainContainer = root.Q("Container");
            tabsContainer = root.Q("Tabs");
            approvalPopupHandler.Setup("Do you want to keep changes you made?", "Keep", "Discard");

            mainContainer.AddToClassList($"rounded-{(variables.themeMaker.tabDirection == TabDirection.Vertical ? "right" : "bottom")}-corners");
            switch (variables.themeMaker.Layout)
            {
                case Layout.Floating:
                    mainContainer.AddToClassList("semi-transparent-background");
                    tabsContainer.AddToClassList("tabs");
                    root.AddToClassList("container-base");
                    apply.parent.AddToClassList("container-base");
                    tabsContainer.AddToClassList("tabs-container");
                    uiDocument.panelSettings.themeStyleSheet = variables.containerTheme;
                    break;
                case Layout.Containered:
                    root.AddToClassList("semi-transparent-background");
                    apply.parent.AddToClassList("semi-transparent-background");
                    root.AddToClassList("fullscreen-base");
                    apply.parent.AddToClassList("fullscreen-base");
                    tabsContainer.AddToClassList("tabs-container");
                    mainContainer.AddToClassList("fullscreen-container");
                    uiDocument.panelSettings.themeStyleSheet = variables.containerTheme;
                    break;
                case Layout.Outlined:
                    root.AddToClassList("semi-transparent-background");
                    apply.parent.AddToClassList("semi-transparent-background");
                    root.AddToClassList("fullscreen-base");
                    apply.parent.AddToClassList("fullscreen-base");
                    tabsContainer.AddToClassList("tabs-outline");
                    mainContainer.AddToClassList($"fullscreen-outline-{variables.themeMaker.tabDirection.ToString().ToLower()}");
                    uiDocument.panelSettings.themeStyleSheet = variables.outlineTheme;
                    break;
                case Layout.HalfOutlined:
                    root.AddToClassList("semi-transparent-background");
                    apply.parent.AddToClassList("semi-transparent-background");
                    root.AddToClassList("fullscreen-base");
                    apply.parent.AddToClassList("fullscreen-base");
                    tabsContainer.AddToClassList("tabs-container");
                    mainContainer.AddToClassList($"fullscreen-outline-{variables.themeMaker.tabDirection.ToString().ToLower()}");
                    uiDocument.panelSettings.themeStyleSheet = variables.outlineTheme;
                    break;
                case Layout.HalfTransparent:
                    root.AddToClassList("semi-transparent-background");
                    apply.parent.AddToClassList("semi-transparent-background");
                    root.AddToClassList("fullscreen-base");
                    apply.parent.AddToClassList("fullscreen-base");
                    tabsContainer.AddToClassList("tabs-container");
                    uiDocument.panelSettings.themeStyleSheet = variables.outlineTheme;
                    break;
                case Layout.Transparent:
                    root.AddToClassList("semi-transparent-background");
                    apply.parent.AddToClassList("semi-transparent-background");
                    root.AddToClassList("fullscreen-base");
                    apply.parent.AddToClassList("fullscreen-base");
                    uiDocument.panelSettings.themeStyleSheet = variables.outlineTheme;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Clear()
        {
            tabsContainer.Clear();
            for (int i = 1, count = mainContainer.childCount; i < count; i++)
            {
                mainContainer.RemoveAt(i);
                i--;
                count--;
            }
        }

        /// <summary>
        /// <para> Generates all elements in the settings menu. </para>
        /// </summary>
        public void GenerateAll()
        {
            Clear(); // Clear all generated elements
            variables.OnGenerate(volumeControllers); // Generate all variables
            ExecuteOptions();
            GenerateTabs();
            if (inputSystem == Enums.InputSystem.LegacyInputSystem)
                keyCodes = (int[]) Enum.GetValues(typeof(KeyCode));
            assignPresets?.Invoke();
        }
        
        /// <summary>
        /// <para> Executes all options in the settings menu. </para>
        /// </summary>
        private void ExecuteOptions()
        {
            for (int i = 0, count = tabs.Count; i < count; i++)
            {
                for (int j = 0, count2 = tabs[i].variables.Count; j < count2; j++)
                {
                    tabs[i].variables[j].Execute();
                }
            }
        }

        /// <summary>
        /// <para> Generates all tabs in the settings menu. </para>
        /// </summary>
        private void GenerateTabs()
        {
            var containerSize = mainContainer.layout.height;
            var floatHeight = containerSize - variables.otherCustomizables.tabButtonHeight;
            var height = (StyleLength) new Length(floatHeight, LengthUnit.Pixel);
            for (int i = 0, count = tabs.Count; i < count; i++)
            {
                GenerateTabButton(tabsContainer, i);
                GenerateVariableList(mainContainer, i, height);
            }

            tabsContainer.AddToClassList(variables.themeMaker.tabDirection == TabDirection.Horizontal
                ? "rounded-top-corners"
                : "rounded-left-corners");
            
            mainContainer.Query<VisualElement>("Variables").ForEach(x =>
            {
                x.style.height = x.parent.parent.layout.height; // to fix the weird gap under the scroll view
            });
        }

        /// <summary>
        /// <para> Generates a tab button. </para>
        /// </summary>
        /// <param name="root"> The root element to add the button to. </param>
        /// <param name="i"> The index of the tab. </param>
        private void GenerateTabButton(VisualElement root, int i)
        {
            root.Add(variables.tabButton.CloneTree());
            var b = root.Query<Button>("TabButton").AtIndex(i);
            b.text = tabs[i].name;
            var buttonIndex = i;
            b.RegisterCallback<ClickEvent>(_ =>
            {
                ChangeTab(buttonIndex);
                b.parent.parent.Query<Button>("TabButton").ForEach(x => x.RemoveFromClassList("tab-button-active"));
                b.AddToClassList("tab-button-active");
            });
            b.AddToClassList("unround-corners");
            if (i == 0)
            {
                b.AddToClassList("tab-button-active");
                b.AddToClassList("rounded-top-left-corner");
            }
            else if (i == tabs.Count - 1)
            {
                b.AddToClassList(variables.themeMaker.tabDirection == TabDirection.Horizontal
                    ? "rounded-top-right-corner"
                    : "rounded-bottom-left-corner");
            }
        }

        /// <summary>
        /// <para> Generates a variable list. </para>
        /// </summary>
        /// <param name="root"> The root element to add the list to. </param>
        /// <param name="i"> The index of the tab. </param>
        /// <param name="height"> The height of the list. </param>
        private void GenerateVariableList(VisualElement root, int i, StyleLength height)
        {
            root.Add(variables.variableList.Instantiate());
            var list = root.Query<ListView>("Variables").AtIndex(i);
            var localIndex = i;
            list.makeItem = () => GenerateVariable(localIndex);
            list.bindItem = BindVariable;
            list.itemsSource = tabs[i].variables.Select(v => v.name).ToList();
            list.visible = false;
            list.style.height = height;
        }
        
        /// <summary>
        /// <para> Generates a variable. </para>
        /// </summary>
        /// <param name="i"> The index of the tab. </param>
        /// <returns> The generated variable. </returns>
        private VisualElement GenerateVariable(int i)
        {
            var container = variables.variableContainer.CloneTree();
            container.userData = tabs[i];
            return container;
        }
        
        /// <summary>
        /// <para> Binds a variable to a the scroll view. </para>
        /// </summary>
        /// <param name="item"> The visual element to bind to. </param>
        /// <param name="index"> The index of the variable. </param>
        private void BindVariable(VisualElement item, int index)
        {
            var v = ((Tab) item.userData).variables[index];
            item.Clear();
            v.displayedElement = item;
            switch (v.optionType)
            {
                case Variable.OptionType.Dropdown or Variable.OptionType.Preset:
                    BindDropdown(item, v);
                    break;
                case Variable.OptionType.ActionDropdown:
                    v.options.Clear();
                    v.getOptionList.Invoke(v.options);
                    BindDropdown(item, v);
                    break;
                case Variable.OptionType.Binding:
                    BindBinding(item, v);
                    break;
                case Variable.OptionType.Slider:
                    BindSlider(item, v);
                    break;
            }
            item.name = v.name;
            item.AddToClassList("item");
            if (variables.themeMaker.tabDirection == TabDirection.Horizontal) HorizontalStyling(item, index);
            else VerticalStyling(item, index);
        }
        
        private void HorizontalStyling(VisualElement item, int index)
        {
            
            var x = Screen.height * 0.7f;
            if (index == ((Tab) item.userData).variables.Count - 1 && variables.otherCustomizables.tabButtonHeight * ((Tab) item.userData).variables.Count > x)
            {
                item.AddToClassList("rounded-bottom-left-corner");
                return;
            }
            item.RemoveFromClassList("rounded-bottom-left-corner");
        }
        
        private void VerticalStyling(VisualElement item, int index)
        {
            var x = Screen.height * 0.7f;
            if (index == 0 && variables.otherCustomizables.tabButtonHeight * ((Tab) item.userData).variables.Count < x)
            {
                item.AddToClassList("rounded-top-right-corner");
            }
        }

        /// <summary>
        /// <para> Binds a dropdown to a variable. </para>
        /// </summary>
        /// <param name="item"> The visual element to bind to. </param>
        /// <param name="variable"> The variable to bind. </param>
        private void BindDropdown(VisualElement item, Variable variable)
        {
            var newDropdown = variables.variableDropdown.CloneTree();
            item.Add(newDropdown);
            if (variable.optionType == Variable.OptionType.Preset)
            {
                preset = variable;
            }
            var dropdown = item.Q<DropdownField>();
            dropdown.label = variable.name;
            if (variable.CurrentValue >= variable.options.Count)
            {
                variable.CurrentValue = 0;
                Debug.LogError($"Current option is out of range! ({variable.name})");
            }

            if (variable.options.Count <= 0) return;
            
            dropdown.choices = variable.options;
            dropdown.value = variable.options[(int)variable.CurrentValue];

            Action presetToCustom;
            if (preset != null && preset.variableNamesEffectedByPreset.Contains(variable.name))
            {
                var de = preset.displayedElement.Q<DropdownField>();
                presetToCustom = () =>
                {
                    if (preset.CurrentValue == variable.CurrentValue ||
                        (variable.options.Count - 1 == variable.CurrentValue && variable.CurrentValue < preset.CurrentValue))
                        return;
                    preset.CurrentValue = preset.options.Count - 1;
                    de.value = de.choices[(int)preset.CurrentValue];
                };
            }
            else
            {
                presetToCustom = () => { };
            }

            dropdown.RegisterValueChangedCallback(_ =>
            {
                variable.CurrentValue = variable.options.IndexOf(dropdown.value);
                presetToCustom();   
                if (variable.optionType == Variable.OptionType.Preset)
                {
                    if (variable.variablesEffectedByPreset != null && variable.variablesEffectedByPreset.Count == 0)
                        variable.variableNamesEffectedByPreset.ForEach(x =>
                        {
                            if (preset.CurrentValue == preset.options.Count - 1) return;
                            var v = tabs[currentTab].variables.FirstOrDefault(y => y.name == x);
                            if (v == null) return;
                            switch (v.optionType)
                            {
                                case Variable.OptionType.Dropdown or Variable.OptionType.ActionDropdown or Variable.OptionType.Preset:
                                    if (v.displayedElement == null || v.displayedElement.Q<DropdownField>() == null) return;
                                    v.displayedElement.Q<DropdownField>().value = v.OptionAtIndex((int)variable.CurrentValue);
                                    break;
                                case Variable.OptionType.Slider:
                                    switch (v.passVariableType)
                                    {
                                        case Variable.PassVariableType.Int:
                                            v.displayedElement.Q<SliderInt>().value = (int)variable.CurrentValue;
                                            break;
                                        case Variable.PassVariableType.Float:
                                            v.displayedElement.Q<Slider>().value = variable.CurrentValue;
                                            break;
                                    }
                                    break;
                            }
                        });
                }
                AddToOnApply(b =>
                {
                    if (b)
                    {
                        variable.onAssignInt?.Invoke((int)variable.CurrentValue);
                        variables.settingsSave.settings[variable.name] = (int)variable.CurrentValue;
                    }
                    else
                    {
                        variable.CurrentValue = variables.settingsSave.settings[variable.name];
                        dropdown.value = variable.options[(int)variable.CurrentValue];
                    }
                });
            });
        }

        private void BindBinding(VisualElement item, Variable variable)
        {
            var newBinding = variables.variableBinding.CloneTree();
            item.Add(newBinding);
            var text = item.Q<Label>();
            text.text = variable.name;
            var button = item.Q<Button>();
            button.text = variables.settingsSave.bindings[variable.name].key.ToString();
            button.RegisterCallback<ClickEvent>(_ =>
            {
                GetKeyCodeInput(button, variable.name);
            });
        }
        
        /// <summary>
        /// <para> Binds a slider to a variable. </para>
        /// </summary>
        /// <param name="item"> The visual element to bind to. </param>
        /// <param name="variable"> The variable to bind. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when the variable is not a slider. </exception>
        private void BindSlider(VisualElement item, Variable variable)
        {
            var isFloat = variable.passVariableType == Variable.PassVariableType.Float;
            var sliderObject = isFloat
                ? variables.variableSlider
                : variables.variableSliderInt;
            var newSlider = sliderObject.CloneTree();
            item.Add(newSlider);
            
            variable.CurrentValue = variables.settingsSave.settings[variable.name];
            
            var slider = new GeneralizedSlider(isFloat ? SliderType.Float : SliderType.Int, item.Q<Slider>(), item.Q<SliderInt>());
            var numField = item.Q<TextField>();

            variable.CurrentValue = variable.range.Clamp(variable.CurrentValue);
            
            slider.SetLabel(variable.name);
            slider.SetRange(variable.range);
            slider.SetValue(variable.CurrentValue);
            slider.RegisterValueChangedCallback(_ =>
            {
                SetNumFieldValue(numField, slider.value, isFloat);
                variable.CurrentValue = slider.value;
                AddToOnApply(b =>
                {
                    if (b)
                    {
                        switch (variable.passVariableType)
                        {
                            case Variable.PassVariableType.Int:
                                variable.onAssignInt?.Invoke((int)slider.value);
                                variables.settingsSave.settings[variable.name] = (int)slider.value;
                                break;
                            case Variable.PassVariableType.Float:
                                variable.onAssignFloat?.Invoke(slider.value);
                                variables.settingsSave.settings[variable.name] = slider.value;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        variable.CurrentValue = variables.settingsSave.settings[variable.name];
                        slider.SetValue(variable.CurrentValue);
                    }
                });
            });
                
            SetNumFieldValue(numField, variable.CurrentValue, isFloat);
            numField.RegisterValueChangedCallback(_ =>
            {
                variable.CurrentValue = variable.passVariableType switch
                {
                    Variable.PassVariableType.Int => int.Parse(numField.value),
                    Variable.PassVariableType.Float => float.Parse(numField.value),
                    _ => throw new ArgumentOutOfRangeException()
                };
                variable.CurrentValue = variable.range.Clamp(variable.CurrentValue);
                SetNumFieldValue(numField, variable.CurrentValue, isFloat);
                slider.SetValue(variable.CurrentValue);
                AddToOnApply((b) =>
                {
                    if (b)
                    {
                        switch (variable.passVariableType)
                        {
                            case Variable.PassVariableType.Int:
                                variable.onAssignInt?.Invoke((int)variable.CurrentValue);
                                variables.settingsSave.settings[variable.name] = (int)variable.CurrentValue;
                                break;
                            case Variable.PassVariableType.Float:
                                variable.onAssignFloat?.Invoke(variable.CurrentValue);
                                variables.settingsSave.settings[variable.name] = variable.CurrentValue;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        variable.CurrentValue = variables.settingsSave.settings[variable.name];
                        SetNumFieldValue(numField, variable.CurrentValue, isFloat);
                        slider.SetValue(variable.CurrentValue);
                    }
                });
            });
        }
        
        private void SetNumFieldValue(TextField numField, float value, bool isFloat = true)
        {
            if (isFloat)
            {
                SetNumFieldValue(numField, value);
            }
            else
            {
                SetNumFieldValue(numField, (int)value);
            }
        }

        private void SetNumFieldValue(TextField numField, float value)
        {
            numField.value = value.ToString("F", CultureInfo.CurrentUICulture).TrimEnd('0').TrimEnd('.');
        }
        
        private void SetNumFieldValue(TextField numField, int value)
        {
            numField.value = value.ToString(CultureInfo.CurrentUICulture);
        }
        
        private void GetKeyCodeInput(TextElement button, string name)
        {
            button.text = $"<color=#{ColorUtility.ToHtmlStringRGB(variables.themeMaker.BakedColor("secondary-accent-color"))}>Press any key...</color>";
            switch (inputSystem)
            {
                case Enums.InputSystem.LegacyInputSystem:
                    StartCoroutine(WaitForKeyPressOld(button, name));
                    break;
                case Enums.InputSystem.InputSystemPackage:
                    StartCoroutine(WaitForKeyPressNew(button, name));
                    break;
            }
        }
        
        #region Old Input System
    
        private IEnumerator WaitForKeyPressOld(TextElement button, string name)
        {
            var b = true;
            while (b)
            {
                if (Input.GetKeyDown(KeyCode.Escape)) yield break;
            
                b = CheckKeyPressOld(button, name);
            
                yield return null;
            }
        }

        private bool CheckKeyPressOld(TextElement button, string name)
        {
            for(int i = 0, count = keyCodes.Length; i < count; i++)
            {
                if (Input.GetKey((KeyCode)keyCodes[i]))
                {
                    OnKeyPressOld((KeyCode)keyCodes[i], button, name);
                    return false;
                }
            }

            return true;
        }

        private void OnKeyPressOld(KeyCode key, TextElement button, string name)
        {
            SetButtonText(button, key);
            AddToOnApply(b =>
            {
                if (b) variables.settingsSave.bindings[name].Set(key);
                else
                {
                    SetButtonText(button, variables.settingsSave.bindings[name].key);
                }
            });
        }
        
        #endregion
        
        #region New Input System
        
        private IEnumerator WaitForKeyPressNew(TextElement button, string name)
        {
            var b = true;
            while (b)
            {
                if (Keyboard.current.escapeKey.isPressed) yield break;
                
                b = CheckKeyPressNew(button, name);
                
                yield return null;
            }
        }
        
        private bool CheckKeyPressNew(TextElement button, string name)
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return true;
            var keys = keyboard.allKeys;
            for(int i = 0, count = keys.Count; i < count; i++)
            {
                if (keys[i].isPressed)
                {
                    OnKeyPressNew(keys[i], button, name);
                    return false;
                }
            }
            return true;
        }
        
        private void OnKeyPressNew(InputControl key, TextElement button, string name)
        {
            SetButtonText(button, (KeyCode) Enum.Parse(typeof(KeyCode), key.displayName));
            AddToOnApply((b) =>
            {
                if (b) variables.settingsSave.bindings[name].Set(key.displayName);
                else
                {
                    SetButtonText(button, variables.settingsSave.bindings[name].key);
                }
            });
        }
        
        #endregion

        private void SetButtonText(TextElement button, KeyCode key)
        {
            button.text = string.Concat(key.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
        
        private void AddToOnApply(Action<bool> action)
        {
            apply.UnregisterCallback<ClickEvent>(_ =>
            {
                onApply?.Invoke(true);
                onApply = null;
            });
                
            onApply += action;

            apply.RegisterCallback<ClickEvent>(_ =>
            {
                onApply?.Invoke(true);
                onApply = null;
            });
        }

        private void ChangeTab(int desiredIndex)
        {
            if (desiredIndex == currentTab) return;
            root.Query<ListView>("Variables").AtIndex(currentTab).visible = false;
            root.Query<ListView>("Variables").AtIndex(desiredIndex).visible = true;
            root.Query<Button>("TabButton").AtIndex(currentTab).RemoveFromClassList("selected");
            root.Query<Button>("TabButton").AtIndex(desiredIndex).AddToClassList("selected");
            currentTab = desiredIndex;
        }
    }
}