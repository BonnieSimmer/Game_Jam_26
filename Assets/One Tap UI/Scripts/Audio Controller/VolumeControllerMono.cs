using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using One_Tap_UI.Audio_Controller.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace One_Tap_UI.Audio_Controller
{
    public class VolumeControllerMono : MonoBehaviour
    {
        [SerializeField] internal List<VolumeController> list = new ();
        [SerializeField] internal AudioData audioData;

        public void Init(float[] maxVolume)
        {
            for (int i = 0, count = list.Count; i < count; i++)
            {
                list[i].Init(maxVolume[i]);
            }
        }
        
        /// <summary>
        /// <para> Sets the default volume of a specific audio source. </para>
        /// </summary>
        /// <param name="audioSource"> The audio source to set the volume. </param>
        /// <param name="volume"> The volume to set. </param>
        /// <returns> Returns true if the audio source is found in the list. </returns>
        public bool SetDefaultVolume(AudioSource audioSource, float volume)
        {
            if (audioSource == null) return false;
            var vc = list.Find(x => x.sources.Contains(audioSource));
            if (vc == null) return false;
            vc.sources[vc.sources.IndexOf(audioSource)].SetDefaultVolume(volume);
            return true;
        }
            
        /// <summary>
        /// <para> Sets the volume multiplier of all the audio sources. </para>
        /// </summary>
        /// <param name="tag"> The tag of the audio sources to set the volume. </param>
        /// <param name="multiplier"> The volume multiplier to set. </param>
        public void SetVolumeMultipliers(string tag, float multiplier)
        {
            var vc = list.Find(x => x.tag == tag);
            vc?.SetVolumeMultiplier(multiplier);
        }

        public List<AudioSource> GetAudioSources()
        {
            return FindObjectsByType<AudioSource>(FindObjectsSortMode.None).ToList();
        }
    }
}