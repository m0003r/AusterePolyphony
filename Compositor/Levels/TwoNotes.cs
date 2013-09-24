using System;
using System.Collections.Generic;

using PitchBase;
using Compositor.Rules;


namespace Compositor.Levels
{
    public class TwoNotes : IDeniable, IComparable<TwoNotes>
    {
        public Note Note1;
        public Note Note2;

        public Rule DeniedRule { get; set; }
        public bool isBanned { get; set; }

        public TwoNotes(Note Note1, Note Note2)
        {
            isBanned = false;
            DeniedRule = null;
            this.Note1 = Note1;
            this.Note2 = Note2;
            Freqs = null;
        }

        public Interval Interval { get { return Note1.Pitch - Note2.Pitch; } }

        public Time Time { get { return (Note1.TimeEnd.Beats > Note2.TimeEnd.Beats) ? Note1.TimeEnd : Note2.TimeEnd; } }

        public int CompareTo(TwoNotes other)
        {
            int c1 = Note1.CompareTo(other.Note1);
            return (c1 == 0) ? Note2.CompareTo(other.Note2) : c1;
        }

        public override string ToString()
        {
            return Note1.ToString() + "@" + Note1.TimeStart.ToString() + "; " + Note2.ToString() + "@" + Note2.TimeStart.ToString();
        }

        public Dictionary<TwoNotes, double> Freqs;
    }
}