using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using PitchBase;

namespace Compositor.Rules.Base
{
    abstract class MelodyRule : ParamRule
    {
        protected Levels.Melody Melody;
        
        protected List<Note> Notes { get { return Melody.notes; } }
        protected Note LastNote { get { return Notes.Last(); } }
        protected Time Time { get { return Melody.Time; } }
        protected Pitch Higher { get { return Melody.Higher; } }
        protected Pitch Lower { get { return Melody.Lower; } }
        protected List<LeapOrSmooth> LeapSmooth { get { return Melody.LeapSmooth; } }

        public override void Init(IDeniable parent)
        {
            Init((Levels.Melody)parent);
        }

        public virtual void Init(Levels.Melody parent)
        {
            Melody = parent;
        }

        protected List<Note> GetLast(int count)
        {
            if (Notes.Count < count)
                throw new ArgumentOutOfRangeException("count", "Requested last-notes count is greater than notes count");

            return Notes.GetRange(Notes.Count - count, count);
        }

        public override bool IsApplicable()
        {
            return Notes.Count != 0 && _IsApplicable();
        }

        public abstract bool _IsApplicable();

        public abstract double Apply(Note nextNotes);

        public override double Apply(IDeniable nextNotes)
        {
            return Apply((Note) nextNotes);
        }
    }
}

