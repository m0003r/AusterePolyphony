using System;
using System.Collections.Generic;
using Compositor.Rules.Base;
using PitchBase;


namespace Compositor.Levels
{
    public class TwoNotes : IDeniable, IComparable<TwoNotes>, IComparable, ITemporal
    {
        public Note Note1;
        public Note Note2;

        public IRule DeniedRule { get; set; }
        public bool IsBanned { get; set; }

        public List<Tuple<IRule, double>> AppliedRules { get; set; }

        public bool Suspension;

        public TwoNotes(Note note1, Note note2)
        {
            IsBanned = false;
            DeniedRule = null;
            Suspension = false;
            Note1 = note1;
            Note2 = note2;
            Freqs = null;
            AppliedRules = new List<Tuple<IRule, double>>();
        }

        public Interval Interval
        {
            get
            {
                if (Note1.Pitch == null || Note2.Pitch == null) return null;

                return Note1.Pitch - Note2.Pitch;
            }
        }

        /// <summary>
        /// Возвращает момент начала совместного звучания
        /// </summary>
        public Time TimeStart { get { return UpperChanged ? Note1.TimeStart : Note2.TimeStart; } }

        /// <summary>
        /// Возвращает момент начала звучания первой из нот
        /// </summary>
        public Time HalfStart { get { return UpperChanged ? Note2.TimeStart : Note1.TimeStart;  } }
        /// <summary>
        /// Возвращает момент снятия последней из двух нот
        /// </summary>
        public Time TimeEnd { get { return (Note1.TimeEnd.Position > Note2.TimeEnd.Position) ? Note1.TimeEnd : Note2.TimeEnd; } }

        /// <summary>
        /// Возвращает момент конца совместного звучания
        /// </summary>
        public Time HalfEnd { get { return (Note1.TimeEnd.Position > Note2.TimeEnd.Position) ? Note2.TimeEnd : Note1.TimeEnd; } }

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
            return String.Format("{0} <{1}@{2} {3}@{4}>", Note1.TimeStart.MeasureString(), Note1, Note1.TimeStart.PositionString(), Note2, Note2.TimeStart.PositionString());
        }

        public FreqsDict Freqs;

        public int CompareTo(object obj)
        {
            var notes = obj as TwoNotes;

            if (notes != null)
                return CompareTo(notes);

            throw new NotImplementedException();
        }

        public bool Equals(ITemporal obj)
        {
            return (obj is TwoNotes) && Equals((TwoNotes) obj);
        }

        public bool Equals(TwoNotes obj)
        {
            return obj.Note1.Equals(Note1) && obj.Note2.Equals(Note2);
        }
    }
}