﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using PitchBase;

namespace Compositor.Rules.Base
{
    abstract class TwoVoicesRule : ParamRule<Levels.TwoVoices, TwoNotes>
    {
        protected Levels.TwoVoices Melody;

        
        protected List<TwoNotes> Notes { get { return Melody.Twonotes; } }
        protected int NotesCount { get { return Melody.Twonotes.Count; } }
        protected TwoNotes LastNote { get { return Notes.Last(); } }
        protected Time Time { get { return Melody.Time; } }

        public override void Init(Levels.TwoVoices parent)
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
            return Notes.Count != 0 && _IsApplicable();
        }

        public abstract bool _IsApplicable();

        public abstract override double Apply(TwoNotes nextNotes);
    }
}
