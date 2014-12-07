using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.Melody
{

    class TritoneRule : MelodyRule
    {
        const double MinimumStrengthForApply = 1.6;

        IEnumerable<Note> _lastTritones;

        public override bool _IsApplicable()
        {
            if (Notes.Count <= 1) return false;

            _lastTritones = FindLastTritone();
            return true;
        }

        private IEnumerable<Note> FindLastTritone()
        {
            /*if (NotesList.Count == 6)
            {
                double f = 1;
            }*/

            return Notes.Where(found => (
                found.Pitch != null &&
                found.Pitch.IsTritone &&
                //(f.Pitch.isTritoneLow ^ n.Pitch.isTritoneLow) &&
                ((LastNote.TimeEnd - found.TimeStart).Bar <= 3) &&
                ((LastNote.TimeEnd - found.TimeStart).Position > 6)
                // (f.Strength >= minimumRequiredForApply)
              ));
        }

        public override double Apply(Note nextNote)
        {
            /*if (!LastNote.Pitch.IsTritone)
                return 1;*/

            var lastStrength = Voice.GetStrengthIf(nextNote);
            if (lastStrength < MinimumStrengthForApply)
                return 1;

            double freq = 1;
            foreach (var f in _lastTritones)
            {
                var distance = (nextNote.TimeStart - f.TimeStart).Position;
                if (f.TimeStart.Beat == nextNote.TimeStart.Beat)
                    freq *= 0.7;
                /* if (f.Strength + lastStrength > MinimumStrengthForApply + distance / 16.0)
                    freq *= 0.2; */
            }

            return freq;
        }
    }

    class TritoneRule2 : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return LeapSmooth.Count >= 1 && LeapSmooth.Last().Interval.IsTritone;
        }

        public override double Apply(Note nextNote)
        {
            return LeapSmooth.Last().CanAdd(nextNote) ? 1 : 0;
        }
    }
}