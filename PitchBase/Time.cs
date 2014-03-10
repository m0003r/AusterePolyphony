using System;

namespace PitchBase
{
    public class Time
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Time) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position*397) ^ Perfectus.GetHashCode();
            }
        }

        public bool Perfectus { get; private set; }

        public int Beats {
            get {
                return Perfectus? 3 : 4;
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
                Position = Bar * BarLength + value;
            }
        }

        public bool AllowEight
        {
            get {
                return (Beat % 4) >= 2;
            }
        }

        public bool StrongTime
        {
            get {
                return Perfectus ? (Beat == 0) : ((Beat % 8) == 0);
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
                var b = Beat;

                if (Perfectus && (b > 8))
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
            Perfectus = perfectus;
        }

        public static Time Create(bool perfectus = false)
        {
            return new Time(perfectus);
        }

        public static Time operator + (Time me, int eights)
        {
            var n = new Time(me.Perfectus) { Position = me.Position + eights };
            return n;
        }

        public static Time operator -(Time me, int eights)
        {
            return me + (-eights);
        }

        public static Time operator +(Time me, Time aux)
        {
            if (aux.Perfectus != me.Perfectus)
                throw new Exception("Can't perform addition on differend-perfected times");

            return me + aux.Position;
        }

        public static Time operator -(Time me, Time aux)
        {
            if (aux.Perfectus != me.Perfectus)
                throw new Exception("Can't perform addition on differend-perfected times");

            return me - aux.Position;
        }

        public static bool operator ==(Time me, Time aux)
        {
            if (aux == null) return false;

            return (me.Perfectus == aux.Perfectus) && (me.Position == aux.Position);
        }

        public static bool operator !=(Time me, Time aux) { return !(me == aux); }

        public static bool operator >(Time me, Time aux) { return me.Position > aux.Position; }
        public static bool operator <(Time me, Time aux) { return me.Position < aux.Position; }

        public static bool operator >=(Time me, Time aux) { return me.Position >= aux.Position; }
        public static bool operator <=(Time me, Time aux) { return me.Position <= aux.Position; }

        protected bool Equals(Time aux)
        {
            return Position == aux.Position && Perfectus.Equals(aux.Perfectus);
        }

        public override string ToString()
        {
            return String.Format("({0}/2) {1}:{2}", Beats, Bar, Beat);
        }
    }
}
