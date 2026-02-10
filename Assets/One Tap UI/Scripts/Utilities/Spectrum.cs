using System;

namespace One_Tap_UI.Utilities
{
public class Spectrum
{
    public float Start { get; private set; }
    public float End { get; private set; }

    public Spectrum(float start, float end)
    {
        if (end < start)
            throw new ArgumentException("End point must be greater than or equal to start point.");

        Start = start;
        End = end;
    }

    /// <summary>
    /// Stretches or contracts the spectrum by a scaling factor.
    /// A factor >1 stretches the spectrum, while a factor <1 contracts it.
    /// </summary>
    /// <param name="scaleFactor">Scaling factor.</param>
    public void Stretch(float scaleFactor)
    {
        if (scaleFactor <= 0)
            throw new ArgumentException("Scale factor must be positive.");

        float center = (Start + End) / 2;
        float halfLength = (End - Start) / 2 * scaleFactor;

        Start = center - halfLength;
        End = center + halfLength;
    }

    /// <summary>
    /// Offsets (translates) the spectrum by a delta value.
    /// Positive delta moves the spectrum to the right, negative to the left.
    /// </summary>
    /// <param name="delta">Offset value.</param>
    public void Offset(float delta)
    {
        Start += delta;
        End += delta;
    }

    /// <summary>
    /// Contracts the spectrum by a scaling factor less than 1.
    /// Equivalent to stretching with scaleFactor <1.
    /// </summary>
    /// <param name="scaleFactor">Scaling factor.</param>
    public void Contract(float scaleFactor)
    {
        Stretch(scaleFactor); // Same as stretching with factor <1
    }

    /// <summary>
    /// Returns the length of the spectrum.
    /// </summary>
    public float Length => End - Start;

    /// <summary>
    /// Maps a value from this spectrum to another spectrum.
    /// </summary>
    /// <param name="value">Value within this spectrum to map.</param>
    /// <param name="targetSpectrum">The spectrum to map the value to.</param>
    /// <returns>Mapped value in the target spectrum.</returns>
    public float MapValueTo(float value, Spectrum targetSpectrum)
    {
        if (value < Start || value > End)
            throw new ArgumentOutOfRangeException(nameof(value), "Value is outside the spectrum range.");

        float proportion = (value - Start) / Length;
        return targetSpectrum.Start + proportion * targetSpectrum.Length;
    }

    /// <summary>
    /// Finds the corresponding value in the target spectrum given a value in this spectrum.
    /// </summary>
    /// <param name="value">Value in this spectrum.</param>
    /// <param name="targetSpectrum">Target spectrum to map to.</param>
    /// <returns>Mapped value in the target spectrum.</returns>
    public float FindCorrespondingValue(float value, Spectrum targetSpectrum)
    {
        return MapValueTo(value, targetSpectrum);
    }

    public override string ToString()
    {
        return $"Spectrum(Start: {Start}, End: {End})";
    }
}
}