using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Object = System.Object;

namespace One_Tap_UI.Audio_Controller
{
    /// <summary>
    /// <para> A class that holds the data of a volume controller. </para>
    /// </summary>
    [Serializable]
    public class VolumeController
    {
        public VolumeController(string tag)
        {
            this.tag = tag;
        }

        [Serializable]
        public class Source
        {
            public AudioSource audioSource;
            public float defaultVolume;
            
            public Source(AudioSource audioSource)
            {
                this.audioSource = audioSource;
                defaultVolume = audioSource.volume;
            }
            
            public void GetDefaultVolume()
            {
                defaultVolume = audioSource.volume;
            }
            public void SetVolume()
            {
                audioSource.volume *= defaultVolume;
            }
            public void SetDefaultVolume(float volume)
            {
                defaultVolume = volume;
            }
        }
        public SourceList sources = new ();
        private float maxVolume = 1f;
        private float multiplier = 1f;
        public string tag = "";
        
        public void Init(float maxVolume)
        {
            this.maxVolume = maxVolume;
            for (int i = 0, count = sources.Count; i < count; i++)
            {
                sources[i].GetDefaultVolume();
                sources[i].SetVolume();
            }
        }
        
        /// <summary>
        /// <para> Sets the volume multiplier of all the audio sources. </para>
        /// </summary>
        /// <param name="value"> The volume multiplier to set. </param>
        public void SetVolumeMultiplier(float value)
        {
            multiplier = value / maxVolume;
            if (!Application.isPlaying) return;
            for (int i = 0, count = sources.Count; i < count; i++)
            {
                sources[i].audioSource.volume = sources[i].defaultVolume * multiplier;
            }
        }

        public bool Add(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            sources.Add(gameObject.TryGetComponent(out AudioSource audioSource)
                ? audioSource
                : gameObject.AddComponent<AudioSource>());
            return true;
        }
        
        public bool Add(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                return false;
            }
            sources.Add(audioSource);
            return true;
        }
        
        public bool Remove(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent(out AudioSource audioSource)) return false;
            sources.Remove(audioSource);
            return true;

        }
        
        public bool Remove(AudioSource audioSource)
        {
            return sources.Remove(audioSource);
        }
        
        /// <summary>
        /// <para> Gets all the audio sources with the tag. </para>
        /// </summary>
        public void Get(List<AudioSource> audioSources)
        {
            if (tag != "")
            {
                sources.Set(audioSources.FindAll(x => x.CompareTag(tag)));
                return;
            }
            sources.Set(audioSources);
        }
    }
}