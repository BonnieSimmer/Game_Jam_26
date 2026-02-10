using System.Collections.Generic;
using UnityEngine;

namespace One_Tap_UI.Utilities
{
    [CreateAssetMenu(fileName = "Sound Lister", menuName = "One Tap UI/Audio/Sound Lister")]
    public class SoundLister : ScriptableObject
    {
        [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

        public AudioClip Get() => audioClips[Random.Range(0, audioClips.Count)];
        public AudioClip Get(int index) => audioClips[index];
    }
}
