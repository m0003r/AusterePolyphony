using System;
using Compositor.Levels;
using PitchBase;

namespace Compositor.Rules.Base
{
    abstract class NoteRule : ParamRule
    {
        protected Note Me;

        protected Time Time { get { return Me.TimeStart; } }
        protected int Duration { get { return Me.Duration; } }
        protected Pitch Pitch { get { return Me.Pitch; } }
        protected Interval Leap { get { return Me.Leap; } }

        public void Init(Note me)
        {
            Me = me;
        }

        public abstract override bool IsApplicable();
        public abstract double Apply(Note nextNotes);

        public override double Apply(IDeniable nextNotes)
        {
            var note = nextNotes as Note;
            if (note != null)
                return Apply(note);

            throw new ArgumentException();
        }

        public override void Init(IDeniable me)
        {
            var note = me as Note;
            if (note != null)
                Init(note);
            else
                throw new ArgumentException();
        }
    }
}
