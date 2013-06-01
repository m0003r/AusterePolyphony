using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    abstract class MelodyRule
    {
        protected MelodyGenerator _p;

        public MelodyRule(MelodyGenerator parent)
        {
            _p = parent;
        }

        protected List<Note> Notes { get { return _p.notes; } }
        protected Note LastNote { get { return Notes.Last(); } }
        protected Time Time { get { return _p.Time; } }
        protected Pitch Higher { get { return _p.Higher; } }
        protected Pitch Lower { get { return _p.Lower; } }

        protected List<Note> GetLast(int count)
        {
            if (Notes.Count < count)
                throw new ArgumentOutOfRangeException("count", "Requested last-notes count is greater than notes count");

            return Notes.GetRange(Notes.Count - count, count);
        }

        public abstract bool IsApplicable();
        public abstract double Apply(Note n);
    }
}

