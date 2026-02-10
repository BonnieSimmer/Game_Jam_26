using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One_Tap_UI.EditorExperience {
    [CreateAssetMenu(fileName = "Display Objects", menuName = "One Tap UI/Editor Experience/Display Objects")]
    public class DisplayObjects : ScriptableObject {
        public List<ScriptableObject> list = new ();

        public void AddObject(ScriptableObject obj) {
            list.Add(obj);
        }

        public void RemoveObject(ScriptableObject obj) {
            list.Remove(obj);
        }

        public void RemoveObjectAt(int i)
        {
            list.RemoveAt(i);
        }

        public void ClearAllObjects()
        {
            list.Clear();
        }
    }
}
