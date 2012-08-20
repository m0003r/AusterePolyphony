using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melody
{
    class Time
    {
        private bool perfectus;

        public int Beats {
            get {
                return perfectus? 3 : 4;
            } 
        }

        private int Beats8
        {
            get
            {
                return Beats * 2;
            }
        }

        public int Position;

        public int Bar
        {
            get
            {
                return Position / Beats8;
            }
            set
            {
                Bar = value;
                Position = value * Beats8 + Beat;
            }
        }

        public int Beat
        {
            get
            {
                return Position % Beats8;
            }
            set
            {
                Beat = value;
                Position = Bar * Beats8 + value;
            }
        }

        private Time(bool perfectus)
        {
            this.perfectus = perfectus;
        }

        public static Time Create(bool perfectus)
        {
            return new Time(perfectus);
        }
    }
}
