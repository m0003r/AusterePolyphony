﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melody
{
    public class Time
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
                return Beats * 4;
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

        public bool allowEight
        {
            get {
                return (Beat % 4) > 2;
            }
        }

        public bool strongTime
        {
            get {
                return (Beat % 8) == 0;
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

        public static Time operator + (Time me, int beats)
        {
            Time n = new Time(me.perfectus);
            n.Position = me.Position + beats;
            return n;
        }

        public static Time operator -(Time me, int beats)
        {
            return me + (-beats);
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
    }
}
