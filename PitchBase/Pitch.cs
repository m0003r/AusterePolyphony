using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchBase
{
    public class Pitch
    {
        public int Value { set; get; }
        public Modus Modus { protected set; get; }

        private String _osh;

        private static String[,] StringForms =
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
                return (int)Math.Floor(((double)Value) / 7.0) - 1;
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

                if (minK <= Math.Abs(Modus.Keys))
                    return Math.Sign(Modus.Keys);
                else
                    return 0;
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


        public Pitch(int PitchValue, Modus PitchModus)
        {
            this.Value = PitchValue;
            this.Modus = PitchModus;

            _osh = OctaveShift(RealOctave);
        }

        public static Interval operator -(Pitch a, Pitch b)
        {
            if (a.Modus != b.Modus)
            {
                throw new Exception("Different modi in substraction!");
            }

            int octaves = a.ModusOctave - b.ModusOctave;
            Interval r = a.FromBase - b.FromBase;
            Interval o = new Interval(IntervalType.Octava, IntervalAlt.Natural);
            
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
            return new Pitch(a.Value + b, a.Modus);
        }

        public static Pitch operator -(Pitch a, int b)
        {
            return new Pitch(a.Value - b, a.Modus);
        }

        public override String ToString()
        {
            return StringForm + _osh;
        }

        public static bool operator <(Pitch a, Pitch b)
        {
            return !((a - b).Upwards);
        }

        public static bool operator >(Pitch a, Pitch b)
        {
            return (a - b).Degrees > 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(Pitch))
                return false;

            return (GetHashCode() == obj.GetHashCode());
        }

        public static bool operator == (Pitch A, Pitch B)
        {
            return A.Equals(B);
        }

        public static bool operator !=(Pitch A, Pitch B)
        {
            return !(A == B);
        }

        private Interval FromBase
        {
            get
            {
                return Modus.baseToDegree(Degree);
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

        public bool isTritoneHigh
        {
            get
            {
                return ((int)Degree - Modus.NotesDelta + 3) % 7 == 0;
            }
        }

        public bool isTritoneLow
        {
            get
            {
                return ((int)Degree - Modus.NotesDelta - 1) % 7 == 0;
            }
        }

        public bool isTritone { get { return (isTritoneHigh || isTritoneLow); } }
    
    }
}
;