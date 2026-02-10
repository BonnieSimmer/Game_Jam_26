using System.Collections.Generic;
using UnityEngine;

namespace One_Tap_UI.Audio_Controller.Data
{
    [CreateAssetMenu(fileName = "Audio Data", menuName = "One Tap UI/UI/Tabs/Audio Data")]
    public class AudioData : ScriptableObject
    {
        [SerializeField] public List<string> audioTags;
        private VolumeControllerMono volumeControllers;
        
        internal void OnGenerate(VolumeControllerMono volumeControllers)
        {
            this.volumeControllers = volumeControllers;
        }
        
        public void GetMicrophoneOptions(List<string> options)
        {
            options.AddRange(Microphone.devices);
        }
        
        public void SetMasterVolume(float value) => SetVolumeByTagIndex(0, value);
        
        public void SetMusicVolume(float value) => SetVolumeByTagIndex(1, value);
        
        public void SetEffectVolume(float value) => SetVolumeByTagIndex(2, value);

        public void SetVolumeByTagIndex(int index, float value)
        {
            volumeControllers.SetVolumeMultipliers(audioTags[index], value);
        }
        
        public void SetMicrophone(int level)
        {
            // implement the microphone to your system.
        }
    }
}