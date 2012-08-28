using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    abstract class NoteRule
    {
        protected Note Me;

        protected Time Time { get { return Me.TimeStart; } }
        protected int Duration { get { return Me.Duration; } }
        protected Pitch Pitch { get { return Me.Pitch; } }
        protected Interval Leap { get { return Me.Leap; } }

        public bool IsApplicable(Note me)
        {
            Me = me;
            return _IsApplicable();
        }

        protected abstract bool _IsApplicable();
        public abstract double Apply(Note NextNote);
    }
}
