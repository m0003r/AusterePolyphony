using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;
using PitchBase;

namespace Compositor.Rules.TwoVoices
{
    class DistanceRule : TwoVoicesRule
    {

        public override bool _IsApplicable()
        {
            return true;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            if (nextNotes.Interval == null)
                return 1;

            var deg = nextNotes.Interval.AbsDeg;
            double c = 1;
            if (deg > 10)
                c = (12.0 - deg)/3.0;
            return (deg > 12) ? 0 : c;
        }
    }
    
    class DenyCrossing : TwoVoicesRule
    {

        Pitch _lowerBound;
        Pitch _upperBound;

        public override bool _IsApplicable()
        {
            var lastup = LastNote.Note1;
            var lastdown = LastNote.Note2;
            var lb = Notes.Reverse<TwoNotes>().First(n => n.Note1 == lastup);
            var ub = Notes.Reverse<TwoNotes>().First(n => n.Note2 == lastdown);

            _lowerBound = lb.Note2.Pitch;
            _upperBound = ub.Note1.Pitch;

            return true;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            return (nextNotes.Note1.Pitch != null && nextNotes.Note1.Pitch < _lowerBound ||
                    nextNotes.Note2.Pitch != null && nextNotes.Note2.Pitch > _upperBound) ? 0 : 1;
        }
    }
}