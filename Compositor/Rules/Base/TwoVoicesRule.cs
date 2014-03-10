using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor.Rules
{
    abstract class TwoVoicesRule : ParamRule<TwoVoices, TwoNotes>
    {
        protected TwoVoices Melody;

        
        protected List<TwoNotes> Notes { get { return Melody.twonotes; } }
        protected int NotesCount { get { return Melody.twonotes.Count; } }
        protected TwoNotes LastNote { get { return Notes.Last(); } }
        protected Time Time { get { return Melody.Time; } }

        public override void Init(TwoVoices parent)
        {
            Melody = parent;
        }

        protected List<TwoNotes> GetLast(int count)
        {
            if (Notes.Count < count)
                throw new ArgumentOutOfRangeException("count", "Requested last-notes count is greater than notes count");

            return Notes.GetRange(Notes.Count - count, count);
        }

        public override bool IsApplicable()
        {
            return (Notes.Count == 0) ? false : _IsApplicable();
        }

        public abstract bool _IsApplicable();

        public abstract override double Apply(TwoNotes n);
    }
}

