using System;
using System.Collections;
using One_Tap_UI.Audio_Controller;
using One_Tap_UI.Utilities;
using One_Tap_UI.UI.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using InputSystem = One_Tap_UI.UI.Enums.InputSystem;
using Tab = One_Tap_UI.UI.Others.Tab;
using One_Tap_UI.UI.Others;

namespace One_Tap_UI.UI.MenuController
{
    public class MenuController : MonoBehaviour
    {
        private FakeEnum lastMenuType;
        
        [SerializeField] private Variables variables;
        [SerializeField] public UnityEngine.InputSystem.PlayerInput playerInput;

        [SerializeField] private MenuSpesific.ResumeMenu msResumeMenu;
        [SerializeField] private MenuSpesific.SettingsMenu msSettingsMenu;
        
        [SerializeField] private SoundLister clickSounds;
        [SerializeField] private VolumeControllerMono volumeControllers;
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private Material blur;
        
        private ResumeMenu resumeMenu = new ResumeMenu();
        private SettingsMenu settingsMenu = new SettingsMenu();

        private SRPs.MenuController SRPSpecific = new SRPs.MenuController();

        private void Awake() {
            variables.SetMenuTypes(variables.menuSpecificSettings.menuNames);

            if (variables.menuType == null) return;
            variables.menuType.index = 0;
        }
        [Obsolete]
        private IEnumerator Start()
        {
            if (msResumeMenu == null || msSettingsMenu == null)
            {
                Debug.LogError("MenuController: MenuSpecific is not assigned!");
                yield break;
            }
            SRPSpecific.Start(variables.urpProfile, blur, variables.otherCustomizables.blurValue, variables.otherCustomizables.blurDuration);
            
            msSettingsMenu.bottom.RegisterCallback((MouseUpEvent _) => { Set("Resume"); });
            
            // The line below may cause the menu to show up for a single frame in the beginning. But it is necessary 
            // for the scroll functionality. If we remove the line, any tab that has a scroll view will overflow.
            yield return null; 
            
            msResumeMenu.GenerateAll();
            msSettingsMenu.GenerateAll();
            
            resumeMenu.Init(msResumeMenu, playerInput);
            settingsMenu.Init(msSettingsMenu);
            
            resumeMenu.ChangeVisibility();
            settingsMenu.ChangeVisibility();
            
            lastMenuType = variables.menuType.Clone() as FakeEnum;

            StartCoroutine(variables.otherCustomizables.inputSystem == InputSystem.LegacyInputSystem
                ? BuiltInInputSystem()
                : InputSystemPackage());

            InitVolumeControllers();
            LoadSettings();
        }
        
        private void LoadSettings()
        {
            for (int i = 0, count = msSettingsMenu.tabs.Count; i < count; i++)
            {
                for (int j = 0, count2 = msSettingsMenu.tabs[i].variables.Count; j < count2; j++)
                {
                    var variable = msSettingsMenu.tabs[i].variables[j];
                    if (variable.passVariableType == Variable.PassVariableType.Int)
                    {
                        variable.CurrentValue = variables.settingsSave.settings[variable.name];
                        if (variable.onAssignInt == null) Debug.LogError($"No onAssignInt function assigned to {variable.name}!");
                        variable.onAssignInt?.Invoke((int)variable.CurrentValue);
                        continue; 
                    }
                    variable.CurrentValue = variables.settingsSave.settings[variable.name];
                    variable.onAssignFloat?.Invoke(variable.CurrentValue);
                }
            }
        }

        private void InitVolumeControllers()
        {
            var sounds = msSettingsMenu.tabs.Find(x => x.name == "Sounds");
            var maxVolumes = new float[sounds.variables.Count];
            for (int i = 0, count = maxVolumes.Length; i < count; i++)
            {
                maxVolumes[i] = sounds.variables[i].range.max;
            }
            volumeControllers.Init(maxVolumes);
        }

        public void PlayClickSfx(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                audioSource.PlayOneShot(clickSounds.Get());
            }
        }
        
        /// <summary>
        /// <para> Sets the menu type. </para>
        /// </summary>
        /// <param name="menuType"> Name of the menu type (None, Resume, Settings). </param>
        public void Set(string menuType)
        {
            variables.menuType.Set(menuType);
        }
        
        private IEnumerator BuiltInInputSystem()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || !variables.menuType.Is(lastMenuType))
                    ChangeMenu();
                yield return null;
            }
        }
        
        private IEnumerator InputSystemPackage()
        {
            while (true)
            {
                if (playerInput.actions["Escape"].WasPerformedThisFrame() || !variables.menuType.Is(lastMenuType))
                {
                    ChangeMenu();
                }
                yield return null;
            }
        }

        /// <summary>
        /// <para> Changes the menu type. </para>
        /// </summary>
        public void ChangeMenu()
        {
           OnDeinitialize();
        }

        /// <summary>
        /// <para> Runs when the menu is deinitialized. </para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void OnDeinitialize()
        {
            switch (lastMenuType.GetCurrent())
            {
                case "None":
                    resumeMenu.ChangeInputSystem();
                    ChangeState();
                    break;
                case "Resume":
                    resumeMenu.ChangeVisibility();
                    ChangeState();
                    break;
                case "Settings":
                    if (msSettingsMenu.onApply == null)
                    {
                        settingsMenu.ChangeVisibility();
                        ChangeState();
                        break;
                    }
                    msSettingsMenu.approvalPopupHandler.Show(
                    onApprove: () =>
                    {
                        msSettingsMenu.onApply(true);
                        settingsMenu.ChangeVisibility();
                        ChangeState();
                    }, 
                    onDecline: () =>
                    {
                        msSettingsMenu.onApply(false);
                        settingsMenu.ChangeVisibility();
                        ChangeState();
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// <para> Changes the state of the menu. </para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ChangeState()
        {
            if (variables.menuType.Is(lastMenuType))
            {
                variables.menuType.Set(variables.menuType.GetCurrent() switch
                {
                    "None" => "Resume",
                    "Resume" => "None",
                    "Settings" => "Resume",
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
            OnInitialize();
        }

        /// <summary>
        /// <para> Runs when the menu is initialized. </para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void OnInitialize() 
        {
            lastMenuType.Set(variables.menuType);
            switch (variables.menuType.GetCurrent())
            {
                case "None":
                    resumeMenu.ChangeInputSystem();
                    SRPSpecific.BlurState(false);
                    break;
                case "Resume":
                    resumeMenu.ChangeVisibility();
                    SRPSpecific.BlurState(true);
                    break;
                case "Settings":
                    settingsMenu.ChangeVisibility();
                    break;
                case "Quit":
                    Application.Quit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnApplicationQuit()
        {
            SRPSpecific.Quit();
        }
    }
}
