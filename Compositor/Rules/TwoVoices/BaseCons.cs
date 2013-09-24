using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;

namespace Compositor.Rules
{
    class ConsonantesSimult : TwoVoicesRule
    {
        public override bool _IsApplicable()
        {
            return LastNote.EndSimult;
        }

        public override double Apply(TwoNotes NextNotes)
        {
            var i = NextNotes.Interval;
            return ((i.Degrees != 3) && i.Consonance) ? 1 : 0;
        }
    }

    class DenyParallelConsonantes : TwoVoicesRule
    {
        private uint degrees;

        public override bool _IsApplicable()
        {
            var i = LastNote.Interval;
            if (i.PerfectConsonance)
            {
                degrees = i.ModDeg;
                return true;
            }
            else
                return false;
        }

        public override double Apply(TwoNotes NextNotes)
        {
            var i = NextNotes.Interval.ModDeg;
            return (degrees == i) ? 0 : 1;
        }
    }

    class DenyStraightToConsonans : TwoVoicesRule
    {

        public override bool _IsApplicable()
        {
            return LastNote.EndSimult;
        }

        public override double Apply(TwoNotes NextNotes)
        {
            return ((NextNotes.Interval.PerfectConsonance) &&
                (NextNotes.Note1.Leap.Upwards == NextNotes.Note2.Leap.Upwards) &&
                (NextNotes.Note1.Leap.isLeap)) ? 0 : 1;
        }
    }

 
}
