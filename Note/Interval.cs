using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes
{
    public class Interval : IComparable
    {
        public IntervalType Type { get; protected set; }
        public IntervalAlt Alteration { get; protected set; }
        public bool Upwards { get; protected set; }

        public int Semitones
        {
            get
            {
                int semitones = (int)Math.Truncate(Type.ToSemitones() + Alteration.ToSemitones());
                return Upwards ? semitones : -semitones;
            }
        }

        public int Degrees
        {
            get
            {
                int deg = (int)Type;
                return Upwards ? deg : -deg;
            }
        }

        public int AbsDeg { get { return (int)Type; } }

        public Interval Abs
        {
            get
            {
                return new Interval(Type, Alteration);
            }
        }

        public Interval(IntervalType Type, IntervalAlt Alteration = IntervalAlt.Natural, bool Upwards = true)
        {
            if (!checkAlterable(Type, Alteration))
                throw new ArgumentException();
            this.Type = Type;
            this.Alteration = Alteration;
            this.Upwards = Upwards;
        }

        private static bool checkAlterable(IntervalType Type, IntervalAlt Alteration)
        {
            double interval = Type.ToSemitones() + Alteration.ToSemitones();
            return (Math.Truncate(interval) == interval);
        }

        public static Interval operator +(Interval a, Interval b)
        {
            int resSemitones = a.Semitones + b.Semitones;
            bool resUp = true;
            IntervalType resType;
            IntervalAlt resAlt;

            if (resSemitones < 0)
            {
                resUp = false;
                resSemitones = -resSemitones;
                a = a.Invert();
                b = b.Invert();
            }

            resType = (IntervalType)(a.Degrees + b.Degrees);
            resAlt = resType.CalcAlteration(resSemitones);

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

        public int CompareTo(object target)
        {
            Interval t = (Interval)target;
            if (this.Type < t.Type)
                return -1;
            if (this.Type > t.Type)
                return 1;
            if (this.Alteration < t.Alteration)
                return -1;
            if (this.Alteration > t.Alteration)
                return 1;
            return 0;
        }

        public override String ToString()
        {
            return Alteration.ToString() + " " + Type.ToString() + (Upwards ? "" : ", down");
        }

        public bool isLeap
        {
            get
            {
                return Type.isLeap();
            }
        }

        public bool isSmooth
        {
            get
            {
                return Type.isCont();
            }
        }

        public bool Consonance
        {
            get
            {
                uint d = (uint)Type % 7;
                if (d == 4)
                    return Alteration == IntervalAlt.Natural;
                if (d > 3)
                    d = 7 - d;
                if ((d == 0) || (d == 2))
                    return true;
                return false;
            }
        }
    }
}
