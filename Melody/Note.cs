using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    using NotesFreq = Dictionary<Note, double>;

    public class Note
    {
        public Pitch Pitch;

        public SubNote[] SubNotes;

        public Time TimeStart;
        public int Duration;
        public Interval Leap;

        public List<Pitch> Diapason;
        public NotesFreq NextNotes;

        private bool filtered;

        public Note(Pitch Pitch, Time TimeStart, int Duration, Note Previous = null)
        {
            this.Pitch = Pitch;
            this.TimeStart = TimeStart;
            this.Duration = Duration;
            if (Previous != null)
            {
                this.Leap = CalcState(Pitch, Previous.Pitch);
                this.Diapason = Previous.Diapason;
            }
            else
                this.Leap = new Interval(IntervalType.Prima);

            this.filtered = false;

            GenSubNotes();   
        }

        /*
        public Note(Pitch Pitch, Time TimeStart, int Duration, Interval State)
        {
            this.Pitch = Pitch;
            this.TimeStart = TimeStart;
            this.Duration = Duration;
            this.State = State;
            this.filtered = false;
            GenSubNotes();            
        }
        */

        protected void GenSubNotes()
        {
            SubNotes = new SubNote[Duration];
            for (int i = 0; i < Duration; i++)
            {
                SubNotes[i] = new SubNote(this, i);
            }
        }

        private static Interval CalcState(Pitch me, Pitch previous)
        {
            if (previous == null)
                return new Interval(IntervalType.Prima);

            return me - previous;
        }

        public Note[] GenerateDurations(Pitch p, Time t)
        {
            List<Note> v = new List<Note>();
            List<int> durVar = new List<int>();

            if (t.Beat % 2 == 1)
                durVar.Add(1);
            else
            {
                if (t.allowEight)
                    durVar.Add(1);
                durVar.Add(2); //четверти 
                durVar.Add(4); //и половины можно на любой доле

                if (t.Beat % 4 == 0) //каждую полвину
                {
                    durVar.Add(6); // половины с точкой
                    durVar.Add(8); // и целые
                }
                if (t.Beat == 0) // на первую долю
                {
                    durVar.Add(12); // целые с точкой
                    durVar.Add(16); // и бревисы
                }
                if (t.Beats == 3) // в трёхдольном размере
                {
                    if (t.Beat == 0)
                        durVar.Add(10); // половинку с точкой + половинку на сильную долю
                    if (t.Beat == 4)
                        durVar.Add(12); // целую с точкой на вторую долю
                }
                else
                {
                    if (t.Beat == 8) // в четырёхдольном в середине такта
                    {
                        durVar.Add(12); // целую с точкой
                        durVar.Add(16); // и бревис
                    }
                }
            }

            foreach (int dur in durVar)
            {
                v.Add(new Note(p, t, dur, this));
            }

            return v.ToArray();
        }

        private void AddVariants()
        {
            Time newPos = TimeStart + Duration;
            double v;

            NextNotes = new Dictionary<Note,double>();

            foreach (Pitch p in Diapason)
                if ((v = allowPitchAfterAt(p, newPos)) > 0)
                    foreach (Note n in GenerateDurations(p, newPos))
                        NextNotes[n] = v * DurationCoeff(n.Duration);
        }

        private double DurationCoeff(int Duration)
        {
            if (Duration < 2)
                return 0.5;  //восьмые
            if (Duration > 8)
                return 1 - (Math.Log(Duration, 2) - 3)*0.8;
            return 1;
        }

        private double allowPitchAfterAt(Pitch p, Time t)
        {
            if (p.equalTo(Pitch))
                return 0; //не бывает никада!

            Interval diff = p - Pitch;

            if (t.Beat % 4 != 0) // нельзя неплавные ходы внутри метрической доли
                return (diff.Type == IntervalType.Secunda) ? 1 : 0.01;

            switch (diff.Type)
            {
                case IntervalType.Secunda: return 1;
                case IntervalType.Tertia: return 0.8;
                case IntervalType.Quarta: return (diff.Alteration == IntervalAlt.Natural) ? 0.7 : 0;
                case IntervalType.Quinta: return (diff.Alteration == IntervalAlt.Natural) ? 0.6 : 0;
                case IntervalType.Octava: return diff.Upwards ? 0.5 : 0.2;
            }

            if ((diff.Semitones == 8) && (diff.Upwards)) // малая секста вверх
                return 0.1;

            return 0;
        }

        public Dictionary<Note,double> FilterVariants()
        {
            if (filtered)
                return NextNotes;

            AddVariants();

            foreach (Note n in NextNotes.Keys.ToList())
                NextNotes[n] *= CheckNext(n);

            filtered = true;
            return NextNotes;
        }

        private double CheckNext(Note n)
        {
            Interval leap = n.Leap;

            if (n.Duration == 1)
            {
                if (Duration > 2) // запрещаем восьмые после больше-чем-четвертей
                    return 0;
                if (leap.isLeap) // запрещаем неплавные ходы восьмыми
                    return 0;
            }

            // TODO: может быть, больше-чем-половинки?
            if ((Duration == 1) && (n.Duration > 2)) // запрещаем больше-чем-четверти после восьмых
                return 0;

            if (Leap.Degrees == 0) //в начале можно всё ??
                return 1;

            if (Math.Abs(leap.Degrees) < 2)
                return 1; // ходить плавно мы можем ВСЕГ-ДА :)))

            return ApplyLeapRules(n, leap);
        }

        private double ApplyLeapRules(Note n, Interval leap)
        {
            Interval s = Leap;
            bool coDir = (s.Upwards == leap.Upwards);

            if (!coDir) //если в разные стороны
            {
                if (s.isCont) // после плавного хода скачок в другом направлении МОЖНО
                    return 1;
                else
                    switch (leap.CompareTo(s))
                    {
                       case -1: return 1;
                        case 0: return 0;
                       default: return 0.01;
                    }
            }
            else
            {
                if (s.isCont)
                    return (Math.Abs(leap.Degrees) <= 4) ? 0.6 : 0; // в ту же сторону — не больше чем на квинту
                else
                    return ((Math.Abs(leap.Degrees) <= 4) && (Math.Abs(s.Degrees) <= 4) && ((leap + s).Consonance)) ? 0.4 : 0;
            }
        }

        public override string ToString()
        {
            string ps = Pitch.ToString();
            switch (Duration)
            {
                case 1: return ps + "8";
                case 2: return ps + "4";
                case 4: return ps + "2";
                case 6: return ps + "2.";
                case 8: return ps + "1";
                case 10: return ps + "2. ~ " + ps + "2";
                case 12: return ps + "1.";
                case 16: return ps + "\\breve";
                default: return ps + "16";
            }
        }
    }
}
