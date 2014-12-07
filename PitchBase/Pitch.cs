using System;
using System.Runtime.Remoting.Messaging;

namespace PitchBase
{
    public class Pitch
    {
        protected bool Equals(Pitch other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return Value == other.Value && Equals(Modus, other.Modus);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value*397) ^ (Modus != null ? Modus.GetHashCode() : 0);
            }
        }

        public int Value { set; get; }
        public Modus Modus { protected set; get; }

        private readonly String _octaveShift;

        private static readonly String[,] StringForms =
        {
            { "ceses", "deses", "eses", "feses", "geses", "ases", "beses" },
            { "ces", "des", "es", "fes", "ges", "as", "b" },
            { "c", "d", "e", "f", "g", "a", "h" },
            { "cis", "dis", "eis", "fis", "gis", "ais", "his" },
            { "cisis", "disis", "eisis", "fisis", "gisis", "aisis", "bisis" }
        };

        public uint Degree
        {
            get
            {
                int d = Value % 7;
                if (d < 0)
                    d += 7;
                return (uint)d;
            }
        }

        public int ModusOctave
        {
            get
            {
                return (int)Math.Floor(Value / 7.0) - 1;
            }
        }

        public int InDiatonic
        {
            get
            {
                int r = ((int)Degree + Modus.DiatonicStart) % 7;
                return (r < 0) ? r + 7 : r;
            }
        }

        public int RealAlteration
        {
            get
            {
                //MinKeysForAlteration
                int minK = 6;
                int inMajorDegree = (int)Degree + Modus.NotesDelta; 
                minK = (minK - 2 * inMajorDegree) % 7;
                if (minK <= 0)
                    minK += 7;

                if (Modus.Keys < 0)
                    minK = 8 - minK;

                return minK <= Math.Abs(Modus.Keys) ? Math.Sign(Modus.Keys) : 0;
            }
        }

        public String StringForm
        {
            get
            {
                return StringForms[RealAlteration + 2, InDiatonic];
            }
        }

        public int RealOctave
        {
            get
            {
                double tor = (Degree + Modus.DiatonicStart) / 7.0;
                return (int)Math.Floor(tor) + ModusOctave;
            }
        }


        public Pitch(int value, Modus modus)
        {
            Value = value;
            Modus = modus;

            _octaveShift = OctaveShift(RealOctave);
        }

        public static Interval operator -(Pitch a, Pitch b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return new Interval(IntervalType.Prima);

            if (a.Modus != b.Modus)
            {
                throw new Exception("Different modi in substraction!");
            }

            int octaves = a.ModusOctave - b.ModusOctave;
            var r = a.FromBase - b.FromBase;
            var o = new Interval(IntervalType.Octava);
            
            if (octaves < 0)
            {
                octaves = -octaves;
                o = -o;
            }

            for (int i = 0; i < octaves; i++)
                r += o;

            return r;
        }

        public static Pitch operator +(Pitch a, IntervalType b)
        {
            return a + (int)b;
        }

        public static Pitch operator -(Pitch a, IntervalType b)
        {
            return a - (int)b;
        }

        public static Pitch operator +(Pitch a, int b)
        {
            return ReferenceEquals(a, null) ? null : new Pitch(a.Value + b, a.Modus);
        }

        public static Pitch operator -(Pitch a, int b)
        {
            return new Pitch(a.Value - b, a.Modus);
        }

        public override String ToString()
        {
            return StringForm + _octaveShift;
        }

        public static bool operator <(Pitch a, Pitch b)
        {
            return !((a - b).Upwards);
        }

        public static bool operator >(Pitch a, Pitch b)
        {
            return (a - b).Degrees > 0;
        }

        public bool Equals(String str)
        {
            return ToString().Equals(str);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Pitch) obj);
        }

        public static bool operator == (Pitch a, Pitch b)
        {
            return ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);
        }

        public static bool operator !=(Pitch a, Pitch b)
        {
            return !(a == b);
        }

        private Interval FromBase
        {
            get
            {
                return Modus.BaseToDegree(Degree);
            }
        }

        public static String OctaveShift(int o)
        {
            if (o > 0)
                return new String('\'', o);
            if (o < 0)
                return new String(',', -o);
            return "";
        }

        public bool IsTritoneHigh
        {
            get
            {
                return ((int)Degree - Modus.NotesDelta + 3) % 7 == 0;
            }
        }

        public bool IsTritoneLow
        {
            get
            {
                return ((int)Degree - Modus.NotesDelta - 1) % 7 == 0;
            }
        }

        public bool IsTritone { get { return (IsTritoneHigh || IsTritoneLow); } }
    
    }
}
;