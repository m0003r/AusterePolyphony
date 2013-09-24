using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchBase
{
    public class Time
    {
        public bool perfectus { get; private set; }

        public int Beats {
            get {
                return perfectus? 3 : 4;
            } 
        }

        public int BarLength
        {
            get
            {
                return Beats * 4;
            }
        }

        public int Position;

        public int Bar
        {
            get
            {
                return Position / BarLength;
            }
            set
            {
                Bar = value;
                Position = value * BarLength + Beat;
            }
        }

        public int Beat
        {
            get
            {
                return Position % BarLength;
            }
            set
            {
                Beat = value;
                Position = Bar * BarLength + value;
            }
        }

        public bool allowEight
        {
            get {
                return (Beat % 4) >= 2;
            }
        }

        public bool strongTime
        {
            get {
                return perfectus ? (Beat == 0) : ((Beat % 8) == 0);
            }
        }

        /*
         * Returns:
         *   Strongness of time
         */ 
        public uint Strongness
        {
            get
            {
                if (Beat == 0)
                    return 4;

                uint n = 0;
                int b = Beat;

                if (perfectus && (b > 8))
                    b -= 4;

                while (b % 2 == 0)
                {
                    b /= 2;
                    n++;
                }
                return n;
            }
        }

        private Time(bool perfectus)
        {
            this.perfectus = perfectus;
        }

        public static Time Create(bool perfectus = false)
        {
            return new Time(perfectus);
        }

        public static Time operator + (Time me, int eights)
        {
            Time n = new Time(me.perfectus);
            n.Position = me.Position + eights;
            return n;
        }

        public static Time operator -(Time me, int eights)
        {
            return me + (-eights);
        }

        public static Time operator +(Time me, Time aux)
        {
            if (aux.perfectus != me.perfectus)
                throw new Exception("Can't perform addition on differend-perfected times");

            return me + aux.Position;
        }

        public static Time operator -(Time me, Time aux)
        {
            if (aux.perfectus != me.perfectus)
                throw new Exception("Can't perform addition on differend-perfected times");

            return me - aux.Position;
        }

        public static bool operator ==(Time me, Time aux)
        {
            return (me.perfectus == aux.perfectus) && (me.Position == aux.Position);
        }

        public static bool operator !=(Time me, Time aux)
        {
            return !(me == aux);
        }

        public bool Equals(Time aux)
        {
            return (this == aux);
        }

        public override string ToString()
        {
            return String.Format("({0}/2) {1}:{2}", Beats, Bar, Beat);
        }
    }
}
