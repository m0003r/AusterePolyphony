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

        public Time TimeStart { get { return (Note1.TimeStart.Position > Note2.TimeStart.Position) ? Note1.TimeStart : Note2.TimeStart; } }
        public Time TimeEnd { get { return (Note1.TimeEnd.Position > Note2.TimeEnd.Position) ? Note1.TimeEnd : Note2.TimeEnd; } }

        public Note Changed { get { return (Note1.TimeStart.Position > Note2.TimeStart.Position) ? Note1 : Note2; } }
        public Note Stayed { get { return (Note1.TimeStart.Position > Note2.TimeStart.Position) ? Note2 : Note1; } }

        public bool Simult { get { return (Note1.TimeStart.Position == Note2.TimeStart.Position); } }
        public bool EndSimult { get { return (Note1.TimeEnd.Position == Note2.TimeEnd.Position); } }

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