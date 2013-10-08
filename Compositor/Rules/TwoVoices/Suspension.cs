using System.Linq;

using Compositor.Levels;

namespace Compositor.Rules
{
    class SusPassRule1 : TwoVoicesRule
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

        uint lastStrongness;

        public override bool _IsApplicable()
        {
            if (LastNote.Interval.Consonance)
            {
                lastStrongness = LastNote.TimeStart.Strongness;
                return true;
            }

            return false;
        }

        //1578535889

        public override double Apply(TwoNotes n)
        {
            if (n.Simult || n.Interval.Consonance)
                return 1;

            if (n.TimeStart.Strongness > lastStrongness) // suspension
            {
                if (n.Changed.TimeEnd.Position < n.Stayed.TimeEnd.Position) // не может разрешаться, когда другой голос уже ушёл
                    return 0;

                if (n.Changed.TimeEnd.Strongness > n.Changed.TimeStart.Strongness) // не может разрешаться на более сильное время
                    return 0;

                if ((n.Interval.AbsDeg == 1) && (n.Changed == n.Note2)) // секунду задерживаем только верхним
                    return 0;

                if ((n.Interval.AbsDeg == 6) && (n.Changed == n.Note1)) // а септиму — только нижним
                    return 0;

                n.Suspension = true;
                
                return 1;
            }

            if (n.TimeStart.Strongness < lastStrongness) // passing | neighbour
            {
                return ((n.Changed.TimeEnd.Position > n.Stayed.TimeEnd.Position) ||
                        (n.Changed.Duration > 4)) ? 0 : 1;
            }

            return 1;
        }
    }


    class SusPassRule2 : TwoVoicesRule
    {
        private bool upperDissonates;

        public override bool _IsApplicable()
        {
            if (LastNote.Suspension)
            {
                upperDissonates = (LastNote.Note1.TimeStart.Position > LastNote.Note2.TimeStart.Position);
                return true;
            }

            return false;
        }

        public override double Apply(TwoNotes n)
        {
            var checking = upperDissonates ? n.Note2 : n.Note1;

            bool cambiata = ((checking.TimeStart.Position % 4 == 2) && (checking.Duration == 2) && (checking.Leap.Degrees == -2));
            return (checking.Leap.Degrees == -1 || cambiata) ? 1 : 0;
        }
    }
}
