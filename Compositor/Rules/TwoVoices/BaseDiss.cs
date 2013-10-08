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


    class LinearDiss : TwoVoicesRule
    {
        /*
         * Вспомогательный звук
         * Должен быть слабее предыдущего
         * Предыдущий должен быть консонанс
         */
        public override bool _IsApplicable()
        {
            return LastNote.Interval.Consonance;
        }

        public override double Apply(TwoNotes NextNotes)
        {
            if (NextNotes.Interval.Consonance)
                return 1;

            return (NextNotes.Changed.Leap.isSmooth) ? 1 : 0;
        }
    }

    class AfterDiss : TwoVoicesRule
    {
        public override bool _IsApplicable()
        {
            return LastNote.Interval.Dissonance;
        }

        public override double Apply(TwoNotes n)
        {
            if (n.Interval.Dissonance)
                return 0;

            if ((n.TimeStart.Beat % 4 == 2) && (n.Changed.Leap.Degrees == -2))
                return 1;

            return (n.Changed.Leap.AbsDeg == 1) ? 1 : 0;

        }
    }
 
}
