using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.TwoVoices
{
    // Neighbors
    // Anticipation
    // Suspension
    // Cambiata


    class LinearDiss : TwoVoicesRule
    {
        /*
         * диссонансы берём только плавно
         */
        public override bool _IsApplicable()
        {
            return LastNote.Interval.Consonance;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            if (nextNotes.Interval.Consonance)
                return 1;

            return nextNotes.Simult
                ? ((nextNotes.Note1.Leap.IsSmooth && nextNotes.Note2.Leap.IsSmooth) ? 1 : 0)
                : ((nextNotes.Changed.Leap.IsSmooth) ? 1 : 0);
        }
    }

    class AfterDiss : TwoVoicesRule
    {
        public override bool _IsApplicable()
        {
            return LastNote.Interval.Dissonance;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            if (nextNotes.Interval.Dissonance)
                return 0;

            if ((nextNotes.TimeStart.Beat % 4 == 2) && (nextNotes.Changed.Leap.Degrees == -2))
                return 1;

            return nextNotes.Simult
                ? ((nextNotes.Note1.Leap.IsSmooth && nextNotes.Note2.Leap.IsSmooth) ? 1 : 0)
                : (nextNotes.Changed.Leap.IsSmooth ? 1 : 0);
        }
    }

    class SecondaSeptimaResolution : TwoVoicesRule
    {
        public override bool _IsApplicable()
        {
            return (LastNote.Interval.Degrees == 1 || LastNote.Interval.Degrees == 6);
        }

        public override double Apply(TwoNotes nextNotes)
        {
            return (nextNotes.Interval.ModDeg == 0) ? 0 : 1;
        }
    }
 
}
