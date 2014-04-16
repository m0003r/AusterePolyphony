using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PitchBase;

namespace Compositor.Levels
{
    public class LeapOrSmooth
    {
        public bool IsLeap;
        public bool IsSmooth { get { return !IsLeap; } }
        public Time TimeStart;
        public int Duration;
        public Time TimeEnd { get { return TimeStart + Duration; } }
        public int NotesCount { get { return _notes.Count; } }
        public Interval Interval;

        public bool Upwards { get { return Interval.Upwards; } }

        private readonly List<Note> _notes;

        public LeapOrSmooth(Note a, Note b)
        {
            TimeStart = a.TimeStart;
            Duration = (b.TimeEnd - TimeStart).Beats;
            IsLeap = b.Leap.IsLeap;
            Interval = b.Pitch - a.Pitch;

            _notes = new List<Note> {a, b};
        }

        public bool CanAdd(Note n)
        {
            return ((n.Leap.IsLeap == IsLeap) && (n.Leap.Upwards == Upwards));
        }

        public void Add(Note n)
        {
            if (!CanAdd(n))
                throw new Exception("Invalid adding to LeapOrSmooth");

            Interval = Interval + n.Leap;
            Duration += n.Duration;
            _notes.Add(n);
        }

        public void Delete()
        {
            if (NotesCount <= 2)
                throw new Exception("Cannot delete from two-note LeapOrSmooth");

            Note n = _notes.Last();
            Interval = Interval - n.Leap;
            Duration -= n.Duration;

            _notes.RemoveAt(NotesCount - 1);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(IsLeap ? "Leap(" : "Smooth(");
            sb.Append(NotesCount);
            sb.Append("/");
            sb.Append(Duration);
            sb.Append(") ");
            sb.Append(Interval);

            return sb.ToString();
        }
    }
}