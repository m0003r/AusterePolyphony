﻿namespace PitchBase
{
    public enum IntervalAlt
    {
        Natural = 0,
        Major = 1,
        Minor = -1,
        Augmented = 2,
        Diminished = -2
    }

    public static class IntervalAltExtensionMethods
    {
        public static double ToSemitones(this IntervalAlt a)
        {
            return (double)a / 2.0;
        }
    }
}
