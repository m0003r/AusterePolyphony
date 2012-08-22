using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    public class Note
    {
        public Pitch Pitch;

        public SubNote[] SubNotes;

        public Time TimeStart;
        public int Duration;
        public Interval State;

        public List<Pitch> Diapason;
        public Dictionary<Note, double> NextNotes;

        public Note(Pitch Pitch, Time TimeStart, int Duration, Note Previous = null)
        {
            this.Pitch = Pitch;
            this.TimeStart = TimeStart;
            this.Duration = Duration;
            if (Previous != null)
                this.State = CalcState(Pitch, Previous.Pitch);
            else
                this.State = new Interval(IntervalType.Prima);

            GenSubNotes();   
        }

        public Note(Pitch Pitch, Time TimeStart, int Duration, Interval State)
        {
            this.Pitch = Pitch;
            this.TimeStart = TimeStart;
            this.Duration = Duration;
            this.State = State;

            GenSubNotes();            
        }

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

                if (!p.isTritone) //а больше можно, если не звук тритона
                {
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

            NextNotes = new Dictionary<Note,double>();

            foreach (Pitch p in Diapason)
                if (allowPitchAfterAt(p, newPos))
                    foreach (Note n in GenerateDurations(p, newPos))
                        NextNotes[n] = 1;
        }

        private bool allowPitchAfterAt(Pitch p, Time t)
        {
            if (p.equalTo(Pitch))
                return false;

            Interval diff = p - Pitch;

            if (t.Beat % 4 != 0) // нельзя неплавные ходы внутри метрической доли
                return (diff.Type == IntervalType.Secunda);

            switch (diff.Type)
            {
                case IntervalType.Tertia: return true;
                case IntervalType.Quarta: return (diff.Alteration == IntervalAlt.Natural);
                case IntervalType.Quinta: return (diff.Alteration == IntervalAlt.Natural);
                case IntervalType.Octava: return true;
            }

            if (diff.Semitones == 8) // малая секста вверх
                return true;

            return false;
        }

        public void FilterVariants()
        {
            AddVariants();

            Note n;
            double v;
            foreach (KeyValuePair<Note, double> kv in NextNotes)
            {
                n = kv.Key;
                v = kv.Value;

                NextNotes[n] = CheckNext(n) ? 0 : 1;
            }
        }

        private bool CheckNext(Note n)
        {
            Interval leap = n.State;

            if (n.Duration == 1)
            {
                if (Duration > 2) // запрещаем восьмые после больше-чем-четвертей
                    return false;
                if (leap.isLeap) // запрещаем неплавные ходы восьмыми
                    return false;
            }

            // TODO: может быть, больше-чем-половинки?
            if ((Duration == 1) && (n.Duration > 2)) // запрещаем больше-чем-четверти после восьмых
                return false;

            if (State.Degrees == 0) //в начале можно всё ??
                return true;

            if (Math.Abs(leap.Degrees) < 2)
                return true; // ходить плавно мы можем ВСЕГ-ДА :)))

            return ApplyLeapRules(n, leap);
        }

        private bool ApplyLeapRules(Note n, Interval leap)
        {
            Interval s = State;
            bool coDir = (s.Upwards == leap.Upwards);

            if (!coDir) //если в разные стороны
            {
                if (s.isCont) // после плавного хода скачок в другом направлении МОЖНО
                    return true;
                else
                    return (leap.CompareTo(s) < 0); //иначе, если только меньше
            }
            else
            {
                if (s.isCont)
                    return (Math.Abs(leap.Degrees) <= 4); // в ту же сторону — не больше чем на квинту
                else
                    return ((Math.Abs(leap.Degrees) <= 4) && (Math.Abs(s.Degrees) <= 4) && ((leap + s).Consonance));
            }
        }
    }
}
