using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    public class Note
    {
        public Pitch Pitch;

        public SubNote[] SubNotes;

        public Time TimeStart;
        public int Duration;

        public NoteState State;

        public Note(Pitch Pitch, Time TimeStart, int Duration, Note Previous = null) : this(Pitch, TimeStart, Duration, CalcState(Pitch, Previous)) { }

        public Note(Pitch Pitch, Time TimeStart, int Duration, NoteState State)
        {
            this.Pitch = Pitch;
            this.TimeStart = TimeStart;
            this.Duration = Duration;
            this.State = State;

            GenSubNotes();
        }

        protected void GenSubNotes()
        {
            SubNotes = new SubNote[Duration];
            for (int i = 0; i < Duration; i++)
            {
                SubNotes[i] = new SubNote(this, i);
            }
        }

        private static NoteState CalcState(Pitch me, Note previous)
        {
            if (previous == null)
                return NoteState.Unknown;

            Interval d = me - previous.Pitch;
            switch (d.Degrees)
            {
                case 0: return NoteState.Unknown; //повторений нет!
                case -1: return NoteState.Down;
                case 1: return NoteState.Up;
                default: return (d.Degrees > 0) ? NoteState.LeapUp : NoteState.LeapDown;
            }
        }
    }
}
