using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes
{
    public class Pitch
    {
        public int Pitch { set; get; }
        public Modus PitchModus { protected set; get; }

        private String _str;

        public uint Degree
        {
            get
            {
                int d = Pitch % 7;
                if (d < 0)
                    d += 7;
                return (uint)d;
            }
        }

        public int RealOctave
        {
            get
            {
                double tor = (FromBase.Semitones + (double)PitchModus.noteStart) / 12.0;
                return (int)Math.Floor(tor) + ModusOctave;
            }
        }

        public int ModusOctave
        {
            get
            {
                return (int)Math.Floor(((double)Pitch) / 7.0) - 1;
            }
        }

        public Pitch(int Pitch, Modus PitchModus)
        {
            this.Pitch = Pitch;
            this.PitchModus = PitchModus;

            _str = new Degree(this).resolve() + Notes.Degree.OctaveShift(RealOctave);
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
            return _str;
        }

        private Interval FromBase
        {
            get
            {
                return PitchModus.baseToDegree(Degree);
            }
        }

        //TODO: isTritone
    }
}
