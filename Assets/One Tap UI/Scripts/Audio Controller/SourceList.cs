using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Source = One_Tap_UI.Audio_Controller.VolumeController.Source;

namespace One_Tap_UI.Audio_Controller
{
    /// <summary>
    /// <para> A list of all the audio sources. </para>
    /// </summary>
    [Serializable]
    public class SourceList : IList<Source>
    {
        [SerializeField] private List<Source> sources = new ();
        
        public Source this[int index]
        {
            get => sources[index];
            set => sources[index] = value;
        }

        public int Count => sources?.Count ?? -1;
        public bool IsReadOnly => false;

        public void Clear()
        {
            sources.Clear();
        }
        public bool Contains(Source item)
        {
            return sources.Contains(item);
        }
        public bool Contains(AudioSource item)
        {
            return sources.Any(x => x.audioSource == item);
        }
        public void CopyTo(Source[] array, int arrayIndex)
        {
            sources.CopyTo(array, arrayIndex);
        }
        public IEnumerator<Source> GetEnumerator()
        {
            return sources.GetEnumerator();
        }
        public int IndexOf(Source item)
        {
            return sources.IndexOf(item);
        }
        public int IndexOf(AudioSource item)
        {
            return sources.FindIndex(x => x.audioSource == item);
        }
        public void Insert(int index, Source item)
        {
            sources.Insert(index, item);
        }
        public void Add(Source item)
        {
            sources.Add(item);
        }
        public void Add(AudioSource item)
        {
            sources.Add(new Source(item));
        }
        public bool Remove(Source item)
        {
            return sources.Remove(item);
        }
        public bool Remove(AudioSource item)
        {
            return sources.Remove(sources.Find(x => x.audioSource == item));
        }
        public void RemoveAt(int index)
        {
            sources.RemoveAt(index);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return sources.GetEnumerator();
        }
        public void Set(IEnumerable<AudioSource> array)
        {
            sources.Clear();
            sources.AddRange(array.Select(x => new Source(x)));
        }
    }
}