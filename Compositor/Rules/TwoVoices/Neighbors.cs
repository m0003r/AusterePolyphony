using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;

namespace Compositor.Rules
{
    // Neighbors
    // Anticipation
    // Suspension
    // Cambiata

    class NeighborsRules : TwoVoicesRule
    {
        public override bool _IsApplicable()
        {
            if (NotesCount < 2)
                return false;

            var last2 = GetLast(2);
            return false;
            //if (last2[0].Interval.Consonance)
        }

        public override double Apply(TwoNotes NextNotes)
        {
            return 1;
        }
    }
 
}
