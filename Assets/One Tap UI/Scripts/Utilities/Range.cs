using System;
using System.Collections.Generic;
using UnityEngine;

namespace One_Tap_UI.Utilities
{
    [Serializable]
    public struct Range
    {
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        public Range(float value)
        {
            min = value;
            max = value;
        }
        
        
        public float min, max;
        
        public List<float> List()
        {
            return new List<float> {min, max};
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, min, max);
        }
        
        public Range Add(float value)
        {
            var temp = this;
            temp.min += value;
            temp.max += value;
            return temp;
        }
        
        public Range Multiply(float value)
        {
            var temp = this;
            temp.min *= value;
            temp.max *= value;
            return temp;
        }

        public Range Divide(float value)
        {
            var temp = this;
            temp.min /= value;
            temp.max /= value;
            return temp;
        }
        
        public Range Subtract(float value)
        {
            var temp = this;
            temp.min -= value;
            temp.max -= value;
            return temp;
        }
        
        public bool Contains(float value)
        {
            return value >= min && value <= max;
        }
    }
}