using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;
using Melody.Rules;

namespace Melody
{
    using NotesFreq = Dictionary<Note, double>;
    using NotesList = List<Note>;

    public class MelodyGenerator
    {
        public Clef Clef { get; private set; }
        public Modus Modus { get; private set; }
        public Time Time { get; private set; }

        internal List<MelodyRule> Rules;

        internal NotesList notes;
        private List<Pitch> Diapason;
        private Random rand;

        internal uint LengthInBeats;

        public NotesList Notes { get { return new NotesList(notes); } }

        internal Pitch Higher;
        internal Pitch Lower;

        public int seed;

        public MelodyGenerator(Clef Clef, Modus Modus, Time Time, int seed = 0)
        {
            this.Clef = Clef;
            this.Modus = Modus;
            this.Time = Time;

            Time.Position = 0;
            SetupDiapason(Clef, Modus);
            SetupRand(seed);
        }

        private void SetupDiapason(Clef Clef, Modus Modus)
        {
            PitchFactory pf = new PitchFactory(Modus, Clef);
            Diapason = pf.Pitches;
            Lower = Diapason.Last();
            Higher = Diapason.First();
        }

        private void SetupRules()
        {
            Rules = new List<MelodyRule>();

            Rules.Add(new CadenzaRule(this));
            Rules.Add(new TrillRule(this));
            Rules.Add(new SyncopaRule(this));
            Rules.Add(new TritoneRule(this));
            Rules.Add(new PeakRule(this));
            Rules.Add(new ManyQuartersRule(this));
            Rules.Add(new DottedHalveRestrictionRule(this));
        }

        private void SetupRand(int seed)
        {
            if (seed == 0)
                this.seed = (int)DateTime.Now.Ticks;
            else
                this.seed = seed;

            rand = new Random(this.seed);
        }

        public void Generate(uint Length)
        {
            LengthInBeats = Length * (uint)Time.Beats * 4;
            int steps = 0;

            SetupRules(); // правила могут зависеть от длины

            FirstNote();
            while ((Time.Position < LengthInBeats) && (steps < 50000))
            {
                Step();
                steps++;
            }
        }

        private void FirstNote()
        {
            notes = new NotesList();

            List<Pitch> pi = Diapason.FindAll(
                delegate(Pitch p)
                {
                    return ((p.Degree % 7 == 4) || (p.Degree % 7 == 0));
                }
            );

            NotesFreq allowed = new NotesFreq();

            foreach (Pitch p in pi)
            {
                allowed[new Note(p, Time, 4)] = 0.5;
                allowed[new Note(p, Time, 6)] = 0.7;
                allowed[new Note(p, Time, 8)] = 1;
                allowed[new Note(p, Time, 12)] = 1;
            }

            chooseFrom(allowed);
            
        }

        private void Step()
        {
            if (Notes.Last().Diapason == null)
                Notes.Last().Diapason = Diapason;
            NotesFreq allowedNext = notes.Last().FilterVariants();

            allowedNext = ApplyMelodyFilters(allowedNext);

            double max = allowedNext.Max(kv => (kv.Value > 0.01) ? kv.Value : 0);

            if (max > 0.3) //должно быть что-то приличное!
            {
                chooseFrom(allowedNext);
            }
            else
                RemoveLast();
        }

        private NotesFreq ApplyMelodyFilters(NotesFreq freqs)
        {
            foreach (Note note in freqs.Keys.ToList())
                if (freqs[note] > 0.01)
                    foreach (MelodyRule r in Rules)
                        if (r.IsApplicable())
                            freqs[note] *=  r.Apply(note);

            return freqs;
        }


        private void chooseFrom(NotesFreq allowed)
        {
            double freqS = allowed.Sum(kv => kv.Value);
            double r = rand.NextDouble() * freqS;
            double accumulator = 0;

            foreach (KeyValuePair<Note, double> kv in allowed)
            {
                accumulator += kv.Value;
                if (kv.Value < 0.02) //более строги будем здесь
                    continue;
                if (accumulator > r)
                {
                    AddNote(kv.Key);
                    break;
                }
            }
        }

        /*
         * TODO:
         * "Сила" мелодических вершин. Должна влиять доля, длительность и экстремальность
         */

        private void AddNote(Note n)
        {
            if (notes.Count > 4)
            {
                if ((n.Leap.Degrees > 0) && (notes.Last().Leap.Degrees < 0))
                {
                    notes.Last().isLower = true;
                    Lower = notes.Last().Pitch;
                }
                if ((n.Leap.Degrees < 0) && (notes.Last().Leap.Degrees > 0))
                {
                    notes.Last().isHigher = true;
                    Higher = notes.Last().Pitch;
                }
            }
            else
            {
                if (n.Pitch > Higher)
                {
                    n.isHigher = true;
                    Higher = n.Pitch;
                }
                if (n.Pitch < Lower)
                {
                    n.isLower = true;
                    Lower = n.Pitch;
                }
            }

            notes.Add(n);
            Time += n.Duration;
        }

        private void RemoveLast(bool ban = true)
        {
            Note n = notes.Last();
            Time -= n.Duration;
            notes.Remove(n);

            if (notes.Count > 0)
            {
                if (ban)
                {
                    notes.Last().NextNotes[n] = 0;
                    n.isBanned = true;
                }
            }
            else
                FirstNote();
        }
    }
}


