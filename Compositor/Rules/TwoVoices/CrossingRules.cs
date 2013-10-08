using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;
using PitchBase;

namespace Compositor.Rules
{
    class DistanceRule : TwoVoicesRule
    {

        public override bool _IsApplicable()
        {
            return true;
        }

        public override double Apply(TwoNotes NextNotes)
        {
            int deg = NextNotes.Interval.AbsDeg;
            double c = 1;
            if (deg > 10)
                c = (12.0 - deg)/3.0;
            return (deg > 12) ? 0 : c;
        }
    }
    
    class DenyCrossing : TwoVoicesRule
    {

        Pitch LowerBound;
        Pitch UpperBound;

        public override bool _IsApplicable()
        {
            var lastup = LastNote.Note1;
            var lastdown = LastNote.Note2;
            var lb = Notes.Reverse<TwoNotes>().First(n => n.Note1 == lastup);
            var ub = Notes.Reverse<TwoNotes>().First(n => n.Note2 == lastdown);

            LowerBound = lb.Note2.Pitch;
            UpperBound = ub.Note1.Pitch;

            return true;
        }

        public override double Apply(TwoNotes NextNotes)
        {
            return ((NextNotes.Note1.Pitch < LowerBound) || (NextNotes.Note2.Pitch > UpperBound)) ? 0 : 1;
        }
    }
}