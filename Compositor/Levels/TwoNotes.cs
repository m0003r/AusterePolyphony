using System;
using System.Collections.Generic;
using Compositor.Rules.Base;
using PitchBase;


namespace Compositor.Levels
{
    public class TwoNotes : IDeniable, IComparable<TwoNotes>
    {
        public Note Note1;
        public Note Note2;

        public IRule DeniedRule { get; set; }
        public bool IsBanned { get; set; }

        public bool Suspension;

        public TwoNotes(Note note1, Note note2)
        {
            IsBanned = false;
            DeniedRule = null;
            Suspension = false;
            Note1 = note1;
            Note2 = note2;
            Freqs = null;
        }

        public Interval Interval { get { return Note1.Pitch - Note2.Pitch; } }

        public Time TimeStart { get { return UpperChanged ? Note1.TimeStart : Note2.TimeStart; } }
        public Time TimeEnd { get { return (Note1.TimeEnd.Position > Note2.TimeEnd.Position) ? Note1.TimeEnd : Note2.TimeEnd; } }

        public bool UpperChanged { get { return (Note1.TimeStart.Position > Note2.TimeStart.Position); } }

        public Note Changed { get { return UpperChanged ? Note1 : Note2; } }
        public Note Stayed { get { return UpperChanged ? Note2 : Note1; } }

        public bool IsSmooth { get { return Simult ? (Note1.Leap.IsSmooth && Note2.Leap.IsSmooth) : (Changed.Leap.IsSmooth); } }

        public bool Simult { get { return (Note1.TimeStart.Position == Note2.TimeStart.Position); } }
        public bool EndSimult { get { return (Note1.TimeEnd.Position == Note2.TimeEnd.Position); } }

        public int CompareTo(TwoNotes other)
        {
            int c1 = Note1.CompareTo(other.Note1);
            return (c1 == 0) ? Note2.CompareTo(other.Note2) : c1;
        }

        public override string ToString()
        {
            return Note1 + "@" + Note1.TimeStart + "; " + Note2 + "@" + Note2.TimeStart;
        }

        public Dictionary<TwoNotes, double> Freqs;
    }
}