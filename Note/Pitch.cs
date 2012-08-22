using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes
{
    public class Pitch
    {
        public int PitchValue { set; get; }
        public Modus PitchModus { protected set; get; }

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
                int d = PitchValue % 7;
                if (d < 0)
                    d += 7;
                return (uint)d;
            }
        }

        public int ModusOctave
        {
            get
            {
                return (int)Math.Floor(((double)PitchValue) / 7.0) - 1;
            }
        }

        public int InDiatonic
        {
            get
            {
                int r = ((int)Degree + PitchModus.DiatonicStart) % 7;
                return (r < 0) ? r + 7 : r;
            }
        }

        public int RealAlteration
        {
            get
            {
                //MinKeysForAlteration
                int minK = 6;
                int inMajorDegree = (int)Degree + PitchModus.NotesDelta; 
                minK = (minK - 2 * inMajorDegree) % 7;
                if (minK <= 0)
                    minK += 7;

                if (PitchModus.Keys < 0)
                    minK = 8 - minK;

                if (minK <= Math.Abs(PitchModus.Keys))
                    return Math.Sign(PitchModus.Keys);
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
                double tor = (Degree + PitchModus.DiatonicStart) / 7.0;
                return (int)Math.Floor(tor) + ModusOctave;
            }
        }


        public Pitch(int PitchValue, Modus PitchModus)
        {
            this.PitchValue = PitchValue;
            this.PitchModus = PitchModus;

            _osh = OctaveShift(RealOctave);
        }

        public static Interval operator -(Pitch a, Pitch b)
        {
            if (a.PitchModus != b.PitchModus)
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

        public override String ToString()
        {
            return StringForm + _osh;
        }

        private Interval FromBase
        {
            get
            {
                return PitchModus.baseToDegree(Degree);
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

        //TODO: isTritone
    }
}
