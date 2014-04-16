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

        public override void AddVariants()
        {
            var newPos = TimeStart + Duration;

            foreach (var p in Diapason)
                GenerateAtPitch(p, newPos);

#if TRACE
            var sb = new StringBuilder();

            foreach (var kv in Freqs)
                sb.AppendFormat("{0}=>{1}\n", kv.Key, kv.Value);

            Console.WriteLine(sb);

#endif
        }

        private void GenerateAtPitch(Pitch p, Time newPos)
        {
            var pitchFreq = this.GetPitchFreqAfter(p, newPos);
            if (pitchFreq <= 0) return;

            var generatedNotes = this.GenerateNotes(p, newPos);

            foreach (var note in generatedNotes)
                Freqs[note] = pitchFreq * this.DurationCoeff(note.Duration);
        }


        public override string ToString()
        {
            var ps = (Pitch == null) ? "r" : Pitch.ToString();
            var sb = new StringBuilder(ps);
            switch (Duration)
            {
                case 1: return sb.Append("8").ToString();
                case 2:
                    return sb.Append("4").ToString();
                case 4:
                    return sb.Append("2").ToString();
                case 6:
                    return sb.Append("2.").ToString();
                case 8:
                    return sb.Append("1").ToString();
                case 10:
                    return sb.Append("2. ~ ").Append(ps).Append(2).ToString();
                case 12:
                    return sb.Append("1.").ToString();
                case 16:
                    return sb.Append("\\breve").ToString();
                default:
                    return sb.Append("16").ToString();
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