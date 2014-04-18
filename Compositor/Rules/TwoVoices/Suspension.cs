using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.TwoVoices
{
    class SusPassTakeRule : TwoVoicesRule
    {

        /* Необходимость задержания:
         * 
         * Если диссонанс взят на более сильное время, чем предшествующий консонанс, 
         * 
         * Взявший диссонанс голос
         * 
         * Должен кончаться не позже второго
         * 
         * 
         * 
         * Проходящий: наоборот
         */

        uint _lastStrongness;

        public override bool _IsApplicable()
        {
            if (LastNote.Interval == null || LastNote.Interval.Consonance)
            {
                _lastStrongness = LastNote.TimeStart.Strongness;
                return true;
            }

            return false;
        }

        //--1578535889
        //1545806974

        public override double Apply(TwoNotes nextNotes)
        {
            if (nextNotes.Simult || nextNotes.Interval == null || nextNotes.Interval.Consonance)
                return 1;

            if (nextNotes.TimeStart.Strongness > _lastStrongness) // задержание
            {
                if (nextNotes.Changed.TimeEnd.Position < nextNotes.Stayed.TimeEnd.Position) // не может разрешаться, когда другой голос уже ушёл
                    return 0;

                if (nextNotes.Stayed.TimeEnd.Strongness > nextNotes.Changed.TimeStart.Strongness) // не может разрешаться на более сильное время
                    return 0;

                if ((nextNotes.Interval.AbsDeg == 1) && (nextNotes.Changed == nextNotes.Note2)) // секунду задерживаем только верхним
                    return 0;

                if ((nextNotes.Interval.AbsDeg == 6) && (nextNotes.Changed == nextNotes.Note1)) // а септиму — только нижним
                    return 0;

                nextNotes.Suspension = true;
                
                return 1;
            }
            //проходящий

            //не можно в том случае если:
            return ((nextNotes.Changed.TimeEnd.Position > nextNotes.Stayed.TimeEnd.Position) || // проходящий уходит позже, чем стоячий
                    (nextNotes.Changed.Duration > 4) || 
                    (nextNotes.TimeStart - LastNote.Changed.TimeStart).Position > 4)
                    ? 0 : 1; 
        }
    }


    class SusPassResolutionRule : TwoVoicesRule
    {
        private bool _upperDissonates;

        public override bool _IsApplicable()
        {
            if (!LastNote.Suspension) return false;

            _upperDissonates = (LastNote.Note1.TimeStart.Position > LastNote.Note2.TimeStart.Position);
            return true;
        }

        public override double Apply(TwoNotes nextNotes)
        {
            if (nextNotes.Interval == null) //нельзя повиснуть
                return 0;

            var checking = _upperDissonates ? nextNotes.Note2 : nextNotes.Note1;

            var cambiata = ((checking.TimeStart.Position % 4 == 2) && (checking.Duration == 2) && (checking.Leap.Degrees == -2));
            return (checking.Leap.Degrees == -1 || cambiata) ? 1 : 0;
        }
    }
}
