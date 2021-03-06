﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using PitchBase;

namespace Compositor.Rules.Base
{
    public abstract class TwoVoicesRule : ParamRule
    {
        protected Levels.TwoVoices Melody;

        
        protected List<TwoNotes> Notes { get { return Melody.Twonotes; } }
        protected int NotesCount { get { return Melody.Twonotes.Count; } }
        protected TwoNotes LastNote { get { return Notes.Last(); } }
        protected Time Time { get { return Melody.Time; } }

        public void Init(Levels.TwoVoices parent)
        {
            Melody = parent;
        }

        public override void Init(IDeniable parent)
        {
            if (!Initiable(parent))
                throw new ArgumentException();

            Melody = (Levels.TwoVoices)parent;
        }

        public override bool Initiable(IDeniable level)
        {
            return level is Levels.TwoVoices;
        }

        protected List<TwoNotes> GetLast(int count)
        {
            if (Notes.Count < count)
                throw new ArgumentOutOfRangeException("count", "Requested last-notesList count is greater than notesList count");

            return Notes.GetRange(Notes.Count - count, count);
        }

        public override bool IsApplicable()
        {
            return Notes.Count != 0 && _IsApplicable();
        }

        public abstract bool _IsApplicable();

        public abstract double Apply(TwoNotes nextNotes);

        public override double Apply(IDeniable nextNotes)
        {
            var notes = nextNotes as TwoNotes;
            if (notes != null)
                return Apply(notes);

            throw new ArgumentException();
        }
    }
}

