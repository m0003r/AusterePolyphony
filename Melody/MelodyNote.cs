using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    public class MelodyNote
    {
        public Pitch Pitch;

        public SubNote[] SubNotes;

        public Time TimeStart;
        public int Duration;

        public MelodyNote(Pitch Pitch, Time TimeStart, int Duration)
        {
            this.Pitch = Pitch;
            this.TimeStart = TimeStart;
            this.Duration = Duration;

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
    }
}
