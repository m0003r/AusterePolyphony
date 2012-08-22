using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    public class MelodyGenerator
    {
        public Clef Clef { get; private set; }
        public Modus Modus { get; private set; }
        public Time Time { get; private set; }

        private List<Note> notes;
        private List<Pitch> Diapason;
        private Random rand;

        public List<Note> Notes
        {
            get
            {
                return new List<Note>(notes);
            }
        }

        public MelodyGenerator(Clef Clef, Modus Modus, Time Time)
        {
            PitchFactory pf;

            this.Clef = Clef;
            this.Modus = Modus;
            this.Time = Time;

            Time.Position = 0;

            pf = new PitchFactory(Modus, Clef);
            Diapason = pf.Pitches;

            rand = new Random();

        }

        public void Generate(uint Length)
        {
            uint BeatCount = Length * (uint)Time.Beats;

            FirstNote();
            while (Time.Position < BeatCount)
                Step();
        }

        private void FirstNote()
        {
            notes = new List<Note>();

            List<Pitch> pi = Diapason.FindAll(
                delegate(Pitch p)
                {
                    return ((p.Degree % 7 == 4) || (p.Degree % 7 == 0));
                }
            );

            List<Note> allowed = new List<Note>();

            foreach (Pitch p in pi)
            {
                allowed.Add(new Note(p, Time, 4));
                allowed.Add(new Note(p, Time, 6));
                allowed.Add(new Note(p, Time, 8));
                allowed.Add(new Note(p, Time, 12));
            }

            chooseFrom(allowed);
            
        }

        private void Step()
        {
            List<Note> allowedNext = new List<Note>();
            if (Notes.Last().Diapason == null)
                Notes.Last().Diapason = Diapason;
            Dictionary<Note, double> d = notes.Last().FilterVariants();


            foreach (KeyValuePair<Note, double> kv in d)
            {
                if (kv.Value > 0)
                    allowedNext.Add(kv.Key);
            }

            if (allowedNext.Count > 0)
                chooseFrom(allowedNext);
            else
                RemoveLast();
        }

        private void chooseFrom(List<Note> allowed)
        {
            int i = rand.Next(allowed.Count);
            AddNote(allowed.ElementAt(i));
        }

        private void AddNote(Note n)
        {
            notes.Add(n);
            Time += n.Duration;
        }

        private void RemoveLast()
        {
            Note n = notes.Last();
            Time -= n.Duration;
            notes.Remove(n);

            if (notes.Count > 0)
                notes.Last().NextNotes[n] = 0;
            else
                FirstNote();
        }
    }
}

