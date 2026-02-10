using System;
using UnityEngine.UIElements;
using Range = One_Tap_UI.Utilities.Range;

namespace One_Tap_UI.UI.Others
{
    /// <summary>
    /// <para> The type of the slider. </para>
    /// </summary>
    public enum SliderType
    {
        Float,
        Int,
    }
    
    /// <summary>
    /// <para> Used to call the same class even if the slider is float or int. </para>
    /// </summary>
    public struct GeneralizedSlider
    {
        private Slider slider;
        private SliderInt sliderInt;
        private SliderType sliderType;
        
        /// <summary>
        /// <para> A class that holds the data of a slider. </para>
        /// </summary>
        /// <param name="sliderType"> The type of the slider (Float, Int). </param>
        /// <param name="slider"> The floating slider. </param>
        /// <param name="sliderInt"> The integer slider. </param>
        /// <exception cref="NullReferenceException"> If both sliders are null. </exception>
        public GeneralizedSlider(SliderType sliderType, Slider slider = null, SliderInt sliderInt = null)
        {
            if (sliderInt == null && slider == null) throw new NullReferenceException("You at least need to pass one of the sliders!");
            
            this.slider = slider;
            this.sliderInt = sliderInt;
            this.sliderType = sliderType;
        }
        
        /// <summary>
        /// <para> Sets the value of the slider. </para>
        /// </summary>
        /// <param name="value"> The value to set. </param>
        /// <exception cref="ArgumentOutOfRangeException"> If the slider type is not float or int (Probably null). </exception>
        public void SetValue(float value)
        {
            switch (sliderType)
            {
                case SliderType.Float:
                    slider.SetValueWithoutNotify(value);
                    break;
                case SliderType.Int:
                    sliderInt.SetValueWithoutNotify((int) value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        /// <summary>
        /// <para> Sets the range of the slider. </para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> If the slider type is not float or int (Probably null). </exception>
        public float value => sliderType switch
        {
            SliderType.Float => slider.value,
            SliderType.Int => sliderInt.value,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        /// <summary>
        /// <para> Sets the range of the slider. </para>
        /// </summary>
        /// <param name="range"> The range to set. </param>
        public void SetRange(Range range)
        {
            switch (sliderType)
            {
                case SliderType.Float:
                    slider.lowValue = range.min;
                    slider.highValue = range.max;
                    break;
                case SliderType.Int:
                    sliderInt.lowValue = (int) range.min;
                    sliderInt.highValue = (int) range.max;
                    break;
            }
        }
        
        /// <summary>
        /// <para> Sets the range of the slider. </para>
        /// </summary>
        /// <param name="min"> The minimum value of the range. </param>
        /// <param name="max"> The maximum value of the range. </param>
        public void SetRange(float min, float max)
        {
            switch (sliderType)
            {
                case SliderType.Float:
                    slider.lowValue = min;
                    slider.highValue = max;
                    break;
                case SliderType.Int:
                    sliderInt.lowValue = (int) min;
                    sliderInt.highValue = (int) max;
                    break;
            }
        }
        
        /// <summary>
        /// <para> Sets the label of the slider. </para>
        /// </summary>
        /// <param name="label"> The label to set. </param>
        public void SetLabel(string label)
        {
            switch (sliderType)
            {
                case SliderType.Float:
                    slider.label = label;
                    break;
                case SliderType.Int:
                    sliderInt.label = label;
                    break;
            }
        }
        
        /// <summary>
        /// <para> Sets the label of the slider. </para>
        /// </summary>
        /// <param name="callback"> The callback to call when the value of the slider changes. </param>
        public void RegisterValueChangedCallback(Action<float> callback)
        {
            switch (sliderType)
            {
                case SliderType.Float:
                    slider.RegisterValueChangedCallback(e => callback(e.newValue));
                    break;
                case SliderType.Int:
                    sliderInt.RegisterValueChangedCallback(e => callback(e.newValue));
                    break;
            }
        }
    }
}