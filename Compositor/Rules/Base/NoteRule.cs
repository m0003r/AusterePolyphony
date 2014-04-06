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
            if (nextNotes.GetType().IsAssignableFrom(typeof (Note)))
                return Apply((Note) nextNotes);

            throw new ArgumentException();
        }

        public override void Init(IDeniable me)
        {
            if (me.GetType().IsAssignableFrom(typeof(Note)))
                Init((Note)me);
            else
                throw new ArgumentException();            
        }
    }
}
