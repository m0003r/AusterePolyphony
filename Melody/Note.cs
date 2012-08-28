using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;
using Melody.Rules;

namespace Melody
{
    using NotesFreq = Dictionary<Note, double>;

    public static class LeapsData
    {
        public static readonly double Tertia = 0.8;
        public static readonly double Quarta = 0.7;
        public static readonly double Quinta = 0.6;
        public static readonly double OctavaUp = 0.5;
        public static readonly double OctavaDown = 0.2;
        public static readonly double SextaUp = 0.1;
    }

    public class Note
    {
        public Pitch Pitch;

        public bool isBanned = false;
        public bool isHigher = false;
        public bool isLower = false;

        public SubNote[] SubNotes;

        public Time TimeStart;
        public int Duration;
        public Interval Leap;

        public List<Pitch> Diapason;
        public NotesFreq NextNotes;

        internal static List<NoteRule> Rules;

        private bool filtered;

        public Note(Pitch Pitch, Time TimeStart, int Duration, Note Previous = null)
        {
            SetupRules();

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

        public static void SetupRules()
        {
            if (Rules != null)
                return;

            Rules = new List<NoteRule>();

            Rules.Add(new StableOnDownBeatRule());
            Rules.Add(new DenyDoubleBrevesRule());
            Rules.Add(new EightRestrictionsAfterRule());
            Rules.Add(new EightRestrictionsBeforeRule());
            Rules.Add(new AfterSmoothLeapRule());
            Rules.Add(new AfterLeapLeapRule());
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
                durVar.Add(2); //четверти можно везде

                if (t.Beat % 4 == 0) //каждую половину
                {
                    durVar.Add(4); // половины
                    durVar.Add(6); // половины с точкой
                    if (Duration > 1) //если не восьмая
                        durVar.Add(8); // и целые
                }

                if (Duration > 1) //после восьмушек всю это белоту нельзя
                {
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
            double v;

            NextNotes = new Dictionary<Note, double>();

            foreach (Pitch p in Diapason)
                if ((v = allowPitchAfterAt(p, newPos)) > 0)
                    foreach (Note n in GenerateDurations(p, newPos))
                        NextNotes[n] = v * DurationCoeff(n.Duration);
        }

        private double DurationCoeff(int Duration)
        {
            if (Duration < 2)
                return 0.5;  //восьмые
            if (Duration > 6)
                return 1 - (Math.Log(Duration, 2) - 3) * 0.8;
            return 1;
        }

        private double allowPitchAfterAt(Pitch p, Time t)
        {
            if (p.equalTo(Pitch))
                return 0; //не бывает никада!

            Interval diff = p - Pitch;

            if ((t.Beat % 4 != 0) || (Duration == 1)) // нельзя неплавные ходы внутри метрической доли или после восьмых
                return (diff.Type == IntervalType.Secunda) ? 1 : 0;

            switch (diff.Type)
            {
                case IntervalType.Secunda: return 1;
                case IntervalType.Tertia: return LeapsData.Tertia;
                case IntervalType.Quarta: return (diff.Alteration == IntervalAlt.Natural) ? LeapsData.Quarta : 0;
                case IntervalType.Quinta: return (diff.Alteration == IntervalAlt.Natural) ? LeapsData.Quinta : 0;
                case IntervalType.Octava: return diff.Upwards ? LeapsData.OctavaUp : LeapsData.OctavaDown;
            }

            if ((diff.Semitones == 8) && (diff.Upwards)) // малая секста вверх
                return LeapsData.SextaUp;

            return 0;
        }

        public Dictionary<Note, double> FilterVariants()
        {
            if (filtered)
                return NextNotes;

            AddVariants();

            foreach (NoteRule r in Rules)
                if (r.IsApplicable(this))
                    foreach (Note n in NextNotes.Keys.ToList())
                        if (NextNotes[n] > 0.01)
                            NextNotes[n] *= r.Apply(n);


            filtered = true;
            return NextNotes;
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