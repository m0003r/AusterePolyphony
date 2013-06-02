using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor.Rules
{
    abstract class NoteRule : Rule<Note, Note>
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

        public abstract bool IsApplicable();
        public abstract double Apply(Note NextNote);
    }
}
