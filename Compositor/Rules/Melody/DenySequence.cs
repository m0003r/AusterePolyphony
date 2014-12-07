using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;
using PitchBase;

namespace Compositor.Rules.Melody
{
    class DenySequence : MelodyRule
    {
        class SequencePattern
        {
            private List<int> Tones { get { return _tones.GetRange(_startedAt, Length); } }
            public int Length { get; private set; }

            private List<int> _tones;

            int _startedAt;
            int _previousStartedAt;

            public bool Init(Voice voice, int lengthInNotes)
            {
                if (lengthInNotes > voice.NoteCount - 3)
                    return false;

                _tones = new List<int>();

                var melodyLength = voice.Time.Position;
                _startedAt = voice[voice.NoteCount - lengthInNotes].TimeStart.Position;
                _previousStartedAt = voice[voice.NoteCount - lengthInNotes - 1].TimeStart.Position;
                var baseShift = melodyLength - _previousStartedAt;

                if (_startedAt < baseShift)
                    return false;

                Length = 0;
                foreach (var subNote in (IEnumerable<KeyValuePair<int, Pitch>>) voice)
                {
                    var subPitch = subNote.Value;
                    _tones.Add(subPitch != null ? subPitch.Value : _tones.Count > 0 ? _tones.Last() : 0);

                    if (subNote.Key > _startedAt)
                        Length++;
                }

                return true;
            }

            /*public void Extend()
            {
                startedAt -= ShiftStep;
                Length += ShiftStep;
            }*/

            private double Difference()
            {
                int comparedLength = 0;
                double diffAccumulator = 0;
                var diffs = new List<int>();

                var myEnum = Tones.GetEnumerator();
                var otherEnum = _tones.GetRange(_previousStartedAt - Length, Length).GetEnumerator();

                while (myEnum.MoveNext() && otherEnum.MoveNext())
                {
                    diffs.Add(Math.Abs(myEnum.Current - otherEnum.Current));
                    comparedLength++;
                } 

                double average = diffs.Average();

                diffAccumulator += diffs.Sum(diff => Math.Pow((diff - average), 2));

                double rawSpread = Math.Sqrt(diffAccumulator / comparedLength);
                double spread = rawSpread;

                if (average > 5) //это уже линеарная имитация...
                    spread *= 1.5;

                return spread;
            }

            public Dictionary<int, double> UndesiredNotes()
            {
                var result = new Dictionary<int, double>();

                /*for (int shift = 1; baseShift + shift < startedAt - 1; shift++)
                {*/
                int nextPitch = _tones[_startedAt - 1];
                result[nextPitch] = Difference();
                //}

                return result;
            }
        }

        Dictionary<int, double> _undesiredPitches;

        const int MaxSequenceLength = 24;

        public override bool _IsApplicable()
        {
            if (Voice.NoteCount < 5)
                return false;

            int currGrabbedNotes = 2;

            _undesiredPitches = new Dictionary<int,double>();

            do
            {
                var pattern = new SequencePattern();
                bool tooLong = !pattern.Init(Voice, currGrabbedNotes);

                if (pattern.Length > MaxSequenceLength || tooLong)
                    break;

                foreach (var kv in pattern.UndesiredNotes())
                    if (_undesiredPitches.ContainsKey(kv.Key))
                        _undesiredPitches[kv.Key] = Math.Min(_undesiredPitches[kv.Key], kv.Value);
                    else
                        _undesiredPitches[kv.Key] = kv.Value;

                currGrabbedNotes++;
            }
            while (true);

            return (_undesiredPitches.Count > 0);
        }



        public override double Apply(Note nextNote)
        {
            int tone = nextNote.Pitch.Value;

            if (!_undesiredPitches.ContainsKey(tone)) return 1;

            var calculated = _undesiredPitches[tone];
            return Math.Min(1, calculated);
        }
    }
}