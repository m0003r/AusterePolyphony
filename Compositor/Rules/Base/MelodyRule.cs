﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using PitchBase;

namespace Compositor.Rules.Base
{
    abstract class MelodyRule : ParamRule
    {
        protected Voice Voice;
        
        protected List<Note> Notes { get { return Voice.NotesList; } }
        protected Note LastNote { get { return Notes.Last(); } }
        protected Time Time { get { return Voice.Time; } }
        protected Pitch Higher { get { return Voice.Higher; } }
        protected Pitch Lower { get { return Voice.Lower; } }
        protected List<LeapOrSmooth> LeapSmooth { get { return Voice.LeapSmooth; } }

        protected virtual bool ApplyToRests { get { return false; } }

        public override void Init(IDeniable parent)
        {
            if (!Initiable(parent))
                throw new ArgumentException();
            
            Init((Voice) parent);
        }

        public override bool Initiable(IDeniable level)
        {
            return (level is Voice);
        }

        public virtual void Init(Voice parent)
        {
            Voice = parent;
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

        public abstract double Apply(Note nextNote);

        public override double Apply(IDeniable nextNotes)
        {
            var note = nextNotes as Note;
            if (note != null)
                return (note.Pitch == null && !ApplyToRests) ? 1 : Apply(note);

            throw new ArgumentException();
        }
    }
}

