using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    using NotesList = List<Note>;

    class CadenzaRule : MelodyRule
    {
        private int lastBar;

        public CadenzaRule(MelodyGenerator parent)
            : base(parent)
        {
            lastBar = (int)_p.LengthInBeats - Time.Beats * 4;
        }

        public override bool IsApplicable()
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

        public TrillRule (MelodyGenerator parent): base(parent) { }
        
        public override bool IsApplicable()
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

        public SyncopaRule(MelodyGenerator parent)
            : base(parent) { }

        public override bool IsApplicable()
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
        public TritoneRule(MelodyGenerator parent)
            : base(parent) { }

        public override bool IsApplicable()
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

        public PeakRule(MelodyGenerator parent)
            : base(parent) { }

        public override bool IsApplicable()
        {
            lastPitch = LastNote.Pitch;

            //запрещаем вниз, если мы на текущей верхней мелодической вершине            
            denyDown = (lastPitch == Higher);
            // или если нижняя — звук тритона, а мы на другом
            denyDown |= ((lastPitch.isTritone) && (Lower.isTritone));

            //и аналогично вверх
            denyUp = (lastPitch == Lower);
            denyUp |= ((lastPitch.isTritone) && (Higher.isTritone));

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
                double k = (n.Duration > 4) ? Math.Max(0.2, 1 - (Math.Log(n.Duration, 2) - 2)*0.5) : 1;
                if (n.TimeStart.Beat % 8 == 0)
                    k *= 0.7;
                return k;
            }
            return 1;
        }
    }

    class ManyQuartersRule : MelodyRule
    {
        public ManyQuartersRule(MelodyGenerator parent)
            : base(parent) { }

        int quarterCount = 0;
        int lastTime = -1;

        public override bool IsApplicable()
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
        public DottedHalveRestrictionRule(MelodyGenerator parent)
            : base(parent) { }

        public override bool IsApplicable()
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
}
