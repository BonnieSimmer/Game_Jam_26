using System;
using System.Collections.Generic;
using UnityEngine;

namespace One_Tap_UI.Utilities
{
    [Serializable]
    public class FakeEnum : ICloneable
    {
        public List<string> states;
        public int index;

        public string GetCurrent() => states[index];

        public FakeEnum(List<string> states, int index = 0)
        {
            this.states = states;
            this.index = index;
        }
        
        public bool Add(string state)
        {
            if (states.Contains(state)) return false;
            states.Add(state);
            return true;
        }
        
        public bool Remove(string state)
        {
            if (!states.Contains(state)) return false;
            states.Remove(state);
            return true;
        }
        
        public bool Is(string state)
        {
            return GetCurrent() == state;
        }
        
        public bool Is(FakeEnum fakeEnum)
        {
            return GetCurrent() == fakeEnum.GetCurrent();
        }
        
        public bool Set(string state)
        {
            if (!states.Contains(state)) return false;
            index = states.IndexOf(state);
            return true;
        }
        
        public bool Set(FakeEnum fakeEnum)
        {
            if (!states.Contains(fakeEnum.GetCurrent())) return false;
            index = states.IndexOf(fakeEnum.GetCurrent());
            return true;
        }

        public object Clone()
        {
            return new FakeEnum(states, 0);
        }
    }
}