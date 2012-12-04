using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor
{
    abstract class MelodyRule : Rule<Melody>
    {
        protected Melody Melody;

        
        protected List<Note> Notes { get { return Melody.notes; } }
        protected Note LastNote { get { return Notes.Last(); } }
        protected Time Time { get { return Melody.Time; } }
        protected Pitch Higher { get { return Melody.Higher; } }
        protected Pitch Lower { get { return Melody.Lower; } }

        public virtual void Init(Melody parent)
        {
            Melody = parent;
        }

        protected List<Note> GetLast(int count)
        {
            if (Notes.Count < count)
                throw new ArgumentOutOfRangeException("count", "Requested last-notes count is greater than notes count");

            return Notes.GetRange(Notes.Count - count, count);
        }

        public bool IsApplicable()
        {
            return (Notes.Count == 0) ? false : _IsApplicable();
        }

        public abstract bool _IsApplicable();

        public abstract double Apply(Note n);
    }
}

