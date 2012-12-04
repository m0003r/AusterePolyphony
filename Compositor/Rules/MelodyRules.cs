using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor.Rules
{
    using NotesList = List<Note>;
    using PitchAtTime = KeyValuePair<int, Pitch>;

    class CadenzaRule : MelodyRule
    {
        private int lastBar;

        public override void Init(Melody parent)
        {
            base.Init(parent);
            lastBar = (int)Melody.DesiredLength - Time.Beats * 4;
        }

        public override bool _IsApplicable()
        {
            return (Time.Position >= lastBar - 16); //если мы не в предпоследнем такте
        }

        public override double Apply(Note n)
        {
            if ((Time.Position < lastBar) && (Time.Position + n.Duration > lastBar)) //нельзя залиговку к последнему такту
                return 0;
            if ((Time.Position == lastBar) &&
                (
                    (n.Duration != Time.Beats * 4) ||
                    (n.Pitch.Degree != 0) ||
                    (n.Leap.AbsDeg >= 2) //и плавным ходом 
                )) //нельзя ничего в последнем такте кроме долгой тоники
                return 0;
            return 1;
        }
    }

    /*********************************************************************/

    class TrillRule : MelodyRule
    {
        private NotesList last;

        public override bool _IsApplicable()
        {
            if (Notes.Count < 3)
                return false;

            last = GetLast(3);

            return (last[0].Pitch == last[2].Pitch);
        }

        public override double Apply(Note n)
        {
            return (n.Pitch == last[1].Pitch) ? 0 : 1;
        }
    }

    /*********************************************************************/

    //запрещаем писать половину после нечетверти и двух четвертей с сильной доли
    class SyncopaRule : MelodyRule
    {
        private NotesList last;

        public override bool _IsApplicable()
        {
            if (Notes.Count == 2)
            {
                last = GetLast(2);
                last.Insert(0, new Note(last[1].Pitch, last[1].TimeStart, 4));
            }
            else if (Notes.Count > 2)
            {
                last = GetLast(3);
            }
            else
                return false;

            return ((last[1].TimeStart.strongTime) &&
                (last[1].Duration == 2) &&
                (last[2].Duration == 2) &&
                (last[0].Duration != 2));
        }

        public override double Apply(Note n)
        {
            return (n.Duration == 4) ? 0 : 1;
        }
    }

    /*********************************************************************/

    /*
     * Мелодическое обнаружение тритона:
     * запрещает брать звук тритона на долгим, если 
     * другой звук тритона был мелодической вершиной
     */

    class TritoneRule : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return (Lower.isTritone || Higher.isTritone);
        }

        public override double Apply(Note n)
        {
            if (!n.Pitch.isTritone)
                return 1;

            double koeff = (1 - n.Duration / 16.0);
            return n.TimeStart.strongTime ? koeff / 2 : koeff;
        }
    }

    /*********************************************************************/

    class PeakRule : MelodyRule
    {
        private bool denyUp;
        private bool denyDown;
        Pitch lastPitch;

        public override bool _IsApplicable()
        {
            lastPitch = LastNote.Pitch;

            //запрещаем вниз, если мы на текущей верхней мелодической вершине            
            denyDown = (lastPitch == Higher);
            // или если нижняя — звук тритона, а мы на другом
            denyDown |= ((lastPitch.isTritone) && (Lower.isTritone) && (lastPitch != Lower));

            //и аналогично вверх
            denyUp = (lastPitch == Lower);
            denyUp |= ((lastPitch.isTritone) && (Higher.isTritone) && (lastPitch != Higher));

            return true; //мы всегда можем запретить стоять на прошлой вершине долго
        }

        public override double Apply(Note n)
        {
            if (denyDown && (n.Pitch < lastPitch))
                return 0;
            if (denyUp && (n.Pitch > lastPitch))
                return 0;
            if ((n.Pitch == Lower) || (n.Pitch == Higher))
            {
                double k = (n.Duration > 4) ? Math.Max(0.2, 1 - (Math.Log(n.Duration, 2) - 2) * 0.5) : 1;
                if (n.TimeStart.Beat % 8 == 0)
                    k *= 0.7;
                return k;
            }
            return 1;
        }
    }

    class ManyQuartersRule : MelodyRule
    {
        int quarterCount = 0;
        int lastTime = -1;

        public override bool _IsApplicable()
        {
            if (lastTime != LastNote.TimeStart.Position)
            {
                lastTime = LastNote.TimeStart.Position;
                if (LastNote.Duration > 2)
                    quarterCount = 0;
                else
                    quarterCount++;
            }

            return (quarterCount >= 6);
        }

        public override double Apply(Note n)
        {
            if (n.Duration > 2)
                return 1;

            switch (quarterCount)
            {
                case 6: return 0.4;
                case 7: return 0.1;
                default: return 0;
            }
        }
    }

    //хорошо бы после половинки с точкой и четверти продолжать движение четвертями
    class DottedHalveRestrictionRule : MelodyRule
    {
        public override bool _IsApplicable()
        {
            if (Notes.Count < 2)
                return false;

            List<Note> last = GetLast(2);
            return ((last[0].Duration == 6) && (last[1].Duration == 2));
        }

        public override double Apply(Note n)
        {
            if (n.Duration == 2)
                return 2; //неслыханная рекомендация!

            return (n.Duration == 6) ? 0.5 : 0.7;
        }
    }

    class DenyTwoNoteSequence : MelodyRule
    {
        Pitch deniedPitch;
        int deniedDuration;

        public override bool _IsApplicable()
        {
            int checkedInterval;

            deniedPitch = null;

            if (Notes.Count < 3)
                return false;

            List<Note> last = GetLast(3);

            checkedInterval = (last[1].Pitch - last[0].Pitch).Degrees;
            if ((last[2].Pitch - last[1].Pitch).Degrees != checkedInterval)
            {
                deniedPitch = last[2].Pitch + checkedInterval;
                deniedDuration = last[1].Duration;
                return true;
            }

            return false;
        }

        public override double Apply(Note n)
        {
            double frq = 1;

            if (n.Duration == deniedDuration)
                frq *= 0.5;

            if (deniedPitch == n.Pitch)
                frq *= 0.1;

            return frq;
        }
    }

    class DenyTwoNoteRhytmicSequence : MelodyRule
    {
        int deniedDuration;
        int patternDuration;

        public override bool _IsApplicable()
        {
            if (Notes.Count < 3)
                return false;

            List<Note> last = GetLast(3);

            if ((last[0].Duration) == (last[2].Duration))
            {
                deniedDuration = last[1].Duration;
                patternDuration = last[0].Duration + deniedDuration;

                return true;
            }

            return false;
        }

        public override double Apply(Note n)
        {
            if (n.Duration == deniedDuration)
                return ((patternDuration % 8) == 0) ? 0 : 0.5;

            return 1;
        }
    }

    //TODO: seed = -967466835 ???
    class DenySequence : MelodyRule
    {
        class SequencePattern
        {
            public List<int> Tones { get { return tones.GetRange(startedAt, Length); } }
            public int Length { get; private set; }

            private List<int> tones;

            int startedAt;
            int melodyLength;
            int this[int index] { get { return tones[index]; } }

            int baseShift;
            int previousStartedAt;

            const int ShiftStep = 2;

            public SequencePattern(Melody m, int notes)
            {
                if (notes > m.NoteCount - 3)
                    throw new IndexOutOfRangeException();

                tones = new List<int>();

                melodyLength = m.Time.Position;
                startedAt = m[m.NoteCount - notes].TimeStart.Position;
                previousStartedAt = m[m.NoteCount - notes - 1].TimeStart.Position;
                baseShift = melodyLength - previousStartedAt;

                if (startedAt < baseShift)
                    throw new IndexOutOfRangeException();

                Length = 0;
                foreach (PitchAtTime subNote in (IEnumerable<PitchAtTime>)m)
                {
                    tones.Add(subNote.Value.Value);
                    if (subNote.Key > startedAt)
                        Length++;
                }
            }

            /*public void Extend()
            {
                startedAt -= ShiftStep;
                Length += ShiftStep;
            }*/

            private double Difference()
            {
                int comparedLength = 0;
                double average, diffAccumulator = 0;
                List<int> diffs = new List<int>();
                List<int>.Enumerator myEnum, otherEnum;

                myEnum = Tones.GetEnumerator();
                otherEnum = tones.GetRange(previousStartedAt - Length, Length).GetEnumerator();

                while (myEnum.MoveNext() && otherEnum.MoveNext())
                {
                    diffs.Add(Math.Abs(myEnum.Current - otherEnum.Current));
                    comparedLength++;
                } 

                average = diffs.Average();

                foreach (int diff in diffs)
                    diffAccumulator += Math.Pow((diff - average), 2);

                return Math.Sqrt(diffAccumulator / (double)comparedLength);
            }

            public Dictionary<int, double> undesiredNotes()
            {
                Dictionary<int, double> result = new Dictionary<int, double>();
                int nextPitch;

                /*for (int shift = 1; baseShift + shift < startedAt - 1; shift++)
                {*/
                    nextPitch = tones[startedAt - 1];
                    result[nextPitch] = Difference();
                //}

                return result;
            }
        }

        Dictionary<int, double> undesiredPitches;

        const int maxSequenceLength = 24;

        public override bool _IsApplicable()
        {
            if (Melody.NoteCount < 5)
                return false;

            int currGrabbedNotes = 2;
            bool tooLong = false;

            undesiredPitches = new Dictionary<int,double>();

            if (Melody.Time.Position == 106)
                tooLong = false;

            SequencePattern pattern;
            do
            {
                try
                {
                    pattern = new SequencePattern(Melody, currGrabbedNotes);
                    if (pattern.Length > maxSequenceLength)
                        tooLong = true;
                    else
                        foreach (KeyValuePair<int, double> kv in pattern.undesiredNotes())
                            if (undesiredPitches.ContainsKey(kv.Key))
                                undesiredPitches[kv.Key] = Math.Max(undesiredPitches[kv.Key], kv.Value);
                            else
                                undesiredPitches[kv.Key] = kv.Value;
                    currGrabbedNotes++;
                }
                catch (IndexOutOfRangeException)
                {
                    tooLong = true;
                }
            }
            while (!tooLong);

            return (undesiredPitches.Count > 0);
        }



        public override double Apply(Note n)
        {
            int tone = n.Pitch.Value;
            double calculated;

            if (undesiredPitches.ContainsKey(tone))
            {
                calculated = undesiredPitches[tone];
                return Math.Min(1, calculated);
            }
            else
                return 1;
        }
    }

    class AverageHeight : MelodyRule
    {
        double desiredCenter;
        double gravityPoint;
        double gravityForce;

        public override void  Init(Melody parent)
        {
 	        base.Init(parent);
            desiredCenter = Melody.Diapason.Average(p => p.Value);
        }

        public override bool _IsApplicable()
        {
            double actualCenter = Melody.notes.Sum(n => n.Pitch.Value * n.Duration) / (double)Melody.Time.Position;
            double diff = desiredCenter - actualCenter;
            gravityPoint = desiredCenter + diff;
            gravityForce = diff;

            return Math.Abs(gravityForce) > 2;
        }

        public override double Apply(Note n)
        {
            double distance = n.Pitch.Value - gravityPoint;
            double force = distance * gravityForce;
            // Console.WriteLine(" >>>>> Gravity to {0}: {1}", n.Pitch.Value, force);
            if (force < 0)
                return 1;
            if (force > 40)
                return 0.1;
            return 1 - force / 45.0;
        }
    }

    // 1245604329
    class DenyStrongNotesRepeat : MelodyRule
    {
        const double minimumRequiredForApply = 1.5;

        public override bool _IsApplicable()
        {
            if (Notes.Count < 3)
                return false;

            return true;
        }

        private IEnumerable<Note> findLastEquiv(Note n)
        {
            return Notes.Where(f => (
                (f.Pitch.Degree == n.Pitch.Degree) && 
                ((n.TimeStart - f.TimeStart).Bar <= 3) &&
                ((n.TimeStart - f.TimeStart).Position > 6) &&
                (f.Strength >= minimumRequiredForApply)
              ));
        }

        public override double Apply(Note n)
        {
            double lastStrength = Melody.getStrengthIf(n);
            if (lastStrength < minimumRequiredForApply)
                return 1;

            double freq = 1;
            foreach (Note f in findLastEquiv(n))
            {
                int distance = (n.TimeStart - f.TimeStart).Position;
                if (f.TimeStart.Beat == n.TimeStart.Beat)
                    freq *= 0.7;
                if (f.Strength + lastStrength > minimumRequiredForApply + distance / 8)
                    freq *= 0.2;
            }
            
            return freq;
        }
    }
}

