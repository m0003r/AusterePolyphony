using System;
using System.Collections.Generic;
using Compositor.Rules.Base;
using Compositor.Rules.Melody;
using PitchBase;
using System.Text;

namespace Compositor.Levels
{

    //[Rule(typeof(StableOnDownBeatRule))]
    [Rule(typeof(DenyDoubleBrevesRule))]
    [Rule(typeof(EightRestrictionsAfterRule))]
    [Rule(typeof(EightRestrictionsBeforeRule))]
    [Rule(typeof(AfterSmoothLeapRule))]
    [Rule(typeof(AfterLeapLeapRule))]

    public class Note : RuledLevel, IComparable<Note>, IComparable
    {
        public Pitch Pitch;

        public bool IsHigher = false;
        public bool IsLower = false;

        public double Strength = 0;
        
        public Time TimeStart { get; private set; }        
        public Time TimeEnd { get { return TimeStart + Duration; } }

        public int Duration { get; private set; }
        public Interval Leap { get; private set; }

        public List<Pitch> Diapason;

        public int Reserve, Uncomp;

        public Note(Pitch pitch, Time timeStart, int duration, Note previous = null)
        {
            Pitch = pitch;
            TimeStart = timeStart;
            Duration = duration;
            
            if (previous != null)
            {
                Leap = CalcState(pitch, previous.Pitch);
                Diapason = previous.Diapason;
            }
            else
                Leap = new Interval(IntervalType.Prima);
        }

        protected static Interval CalcState(Pitch me, Pitch previous)
        {
            if (previous == null)
                return new Interval(IntervalType.Prima);

            return me - previous;
        }

        public override void AddVariants(bool dumpResult = false)
        {
            Time newPos = TimeStart + Duration;
            double v;

            foreach (Pitch p in Diapason)
                if ((v = this.AllowPitchAfterAt(p, newPos)) > 0)
                    foreach (Note n in this.GenerateDurations(p, newPos))
                        Freqs[n] = v * this.DurationCoeff(n.Duration);

            if (dumpResult)
            {
                var sb = new StringBuilder();

                foreach (var kv in Freqs)
                    sb.AppendFormat("{0}=>{1}\n", kv.Key, kv.Value);

                Console.WriteLine(sb);
            }
        }
         

        public override string ToString()
        {
            string ps = Pitch.ToString();
            switch (Duration)
            {
                case 1: return ps + "8";
                case 2: return ps + "4";
                case 4: return ps + "2";
                case 6: return ps + "2.";
                case 8: return ps + "1";
                case 10: return ps + "2. ~ " + ps + "2";
                case 12: return ps + "1.";
                case 16: return ps + "\\breve";
                default: return ps + "16";
            }
        }

        public int CompareTo(object obj)
        {
            var note = obj as Note;
            if (note != null)
                return CompareTo(note);

            throw new NotImplementedException();
        }

        internal void UpdateFreqs(FreqsDict freqs)
        {
            Freqs = freqs;
        }

        public int CompareTo(Note obj)
        {
            if (Pitch.Value > obj.Pitch.Value)
                return 1;
            if (Pitch.Value < obj.Pitch.Value)
                return -1;

            return Duration.CompareTo(obj.Duration);
        }

        public bool Equals(Note obj)
        {
            return (TimeStart == obj.TimeStart) && (Pitch == obj.Pitch) && (Duration == obj.Duration);
        }
    }
}