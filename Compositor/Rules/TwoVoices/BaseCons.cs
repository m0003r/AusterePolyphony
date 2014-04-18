using System;
using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.TwoVoices
{
    class ConsonantesSimult : TwoVoicesRule
    {
        public override bool _IsApplicable()
        {
            return LastNote.EndSimult;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            var i = nextNotes.Interval;
            return i == null || i.Degrees != 3 && i.Consonance ? 1 : 0;
        }
    }

    class DenyParallelConsonantes : TwoVoicesRule
    {
        private uint _degrees;

        public override bool _IsApplicable()
        {
            var i = LastNote.Interval;
            if (i == null || !i.PerfectConsonance && i.ModDeg != 4) return false;

            _degrees = i.ModDeg;
            return true;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            var i = nextNotes.Interval;
            return i == null || _degrees != i.ModDeg ? 1 : 0;
        }
    }

    class DenyHiddenParallelConsonantes : TwoVoicesRule
    {
        private uint _degrees;

        public override bool _IsApplicable()
        {
            var stayedStart = LastNote.Stayed.TimeStart;
            var takedAtEnumerator = Notes.Reverse<TwoNotes>().TakeWhile<TwoNotes>(n => n.TimeStart >= stayedStart);

            try
            {
                var i = takedAtEnumerator.Last().Interval;
                if (i != null && (i.PerfectConsonance || i.ModDeg == 4))
                {
                    _degrees = i.ModDeg;
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return false;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            var i = nextNotes.Interval;
            if (i == null)
                return 1;

            return (_degrees == i.ModDeg) ? 0 : 1;
        }
    }

    class DenyStraightToConsonans : TwoVoicesRule
    {

        public override bool _IsApplicable()
        {
            return LastNote.EndSimult;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            return (nextNotes.Interval != null &&
                    nextNotes.Interval.PerfectConsonance &&
                    nextNotes.Note1.Leap.Upwards == nextNotes.Note2.Leap.Upwards &&
                    nextNotes.Note1.Leap.IsLeap) ? 0 : 1;
        }
    }

 
}
