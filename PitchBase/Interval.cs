using System;

namespace PitchBase
{
    public class Interval : IComparable
    {
        public Interval(IntervalType type, IntervalAlt alteration = IntervalAlt.Natural, bool upwards = true)
        {
            if (!CheckAlterable(type, alteration))
                throw new ArgumentException();
            Type = type;
            Alteration = alteration;
            Upwards = upwards;
        }

        public IntervalType Type { get; protected set; }
        public IntervalAlt Alteration { get; protected set; }
        public bool Upwards { get; protected set; }

        public int Semitones
        {
            get
            {
                var semitones = (int) Math.Truncate(Type.ToSemitones() + Alteration.ToSemitones());
                return Upwards ? semitones : -semitones;
            }
        }

        public int Degrees
        {
            get
            {
                var deg = (int) Type;
                return Upwards ? deg : -deg;
            }
        }

        public int AbsDeg
        {
            get { return (int) Type; }
        }

        public Interval Abs
        {
            get { return new Interval(Type, Alteration); }
        }

        public bool IsLeap
        {
            get { return Type.IsLeap(); }
        }

        public bool IsSmooth
        {
            get { return Type.IsCont(); }
        }

        public bool Consonance
        {
            get
            {
                uint d = ModDeg;
                if (d == 4)
                    return Alteration == IntervalAlt.Natural;
                if (d > 3)
                    d = 7 - d;
                return ((d == 0) || (d == 2));
            }
        }

        public bool Dissonance
        {
            get { return !Consonance; }
        }

        public bool PerfectConsonance
        {
            get
            {
                if (Dissonance)
                    return false;
                return ((ModDeg == 4) || (ModDeg == 0));
            }
        }


        public bool IsTritone
        {
            get { return IsDimQuinta || IsAugQuarta; }
        }

        public bool IsDimQuinta
        {
            get { return (((uint) Type%7 == 4) && (Alteration == IntervalAlt.Diminished)); }
        }


        public bool IsAugQuarta
        {
            get { return (((uint) Type%7 == 3) && (Alteration == IntervalAlt.Augmented)); }
        }

        public uint ModDeg
        {
            get { return (uint) Type%7; }
        }

        public int CompareTo(object target)
        {
            var t = (Interval) target;
            if (Type < t.Type)
                return -1;
            if (Type > t.Type)
                return 1;
            if (Alteration < t.Alteration)
                return -1;
            if (Alteration > t.Alteration)
                return 1;
            return 0;
        }

        private static bool CheckAlterable(IntervalType type, IntervalAlt alteration)
        {
            double interval = type.ToSemitones() + alteration.ToSemitones();
            return (Math.Abs(Math.Truncate(interval) - interval) < 0.1);
        }

        public static Interval operator +(Interval a, Interval b)
        {
            int resSemitones = a.Semitones + b.Semitones;
            bool resUp = true;

            if (resSemitones < 0)
            {
                resUp = false;
                resSemitones = -resSemitones;
                a = a.Invert();
                b = b.Invert();
            }

            var resType = (IntervalType) (a.Degrees + b.Degrees);
            var resAlt = resType.CalcAlteration(resSemitones);

            return new Interval(resType, resAlt, resUp);
        }

        public static Interval operator -(Interval a, Interval b)
        {
            return a + b.Invert();
        }

        public Interval Invert()
        {
            return new Interval(Type, Alteration, !Upwards);
        }


        public static Interval operator -(Interval a)
        {
            return a.Invert();
        }

        public override String ToString()
        {
            return Alteration + " " + Type + (Upwards ? "" : ", down");
        }
    }
}