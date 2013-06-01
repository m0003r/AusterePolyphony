using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;

namespace Compositor.Levels
{
    public class SubNotes
    {
        public List<Pitch> Pitches { get; private set; }
        public List<bool> Beginnings { get; private set; }

        public SubNotes()
        {
            Pitches = new List<Pitch>();
            Beginnings = new List<bool>();
        }

        public SubNotes(Note note) : this()
        {
            Add(note);
        }

        public void Add(Note note)
        {
            for (int i = note.TimeStart.Beats; i < note.TimeEnd.Beats; i++)
            {
                Pitches.Insert(i, note.Pitch);
                Beginnings.Insert(i, false);
            }

            Beginnings[note.TimeStart.Beats] = true;
        }
    }
}
