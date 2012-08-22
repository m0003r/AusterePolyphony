using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody
{
    using NotesFreq = Dictionary<Note, double>;
    using NotesList = List<Note>;

    public class MelodyGenerator
    {
        public Clef Clef { get; private set; }
        public Modus Modus { get; private set; }
        public Time Time { get; private set; }

        private NotesList notes;
        private List<Pitch> Diapason;
        private Random rand;

        public NotesList Notes { get { return new NotesList(notes); } }

        private Pitch Higher;
        private Pitch Lower;

        public MelodyGenerator(Clef Clef, Modus Modus, Time Time)
        {
            PitchFactory pf;

            this.Clef = Clef;
            this.Modus = Modus;
            this.Time = Time;

            Time.Position = 0;

            pf = new PitchFactory(Modus, Clef);
            Diapason = pf.Pitches;

            Lower = Diapason.Last();
            Higher = Diapason.First();

            rand = new Random();

        }

        public void Generate(uint Length)
        {
            uint BeatCount = Length * (uint)Time.Beats * 2;

            FirstNote();
            while (Time.Position < BeatCount)
                Step();
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

            if (allowedNext.Sum(kv => kv.Value) > 0.01)
            {
                chooseFrom(allowedNext);
            }
            else
                RemoveLast();
        }

        private NotesFreq ApplyMelodyFilters(NotesFreq freqs)
        {
            freqs = CheckForTrill(freqs);
            freqs = CheckForSyncope(freqs);
            freqs = CheckForManyQuarters(freqs);
            freqs = CheckForTop(freqs);

            return freqs;
        }

        private NotesFreq CheckForTop(NotesFreq freqs)
        {
            if (notes.Count < 4)
                return freqs;

            NotesFreq resFreq = freqs;

            //запрещаем вниз, если мы на текущей верхней мелодической вершине            
            bool denyDown = (notes.Last().Pitch == Higher);
            // или если нижняя — звук тритона, а мы на другом
            denyDown |= ((notes.Last().Pitch.isTritone) && (Lower.isTritone));

            //и аналогично вверх
            bool denyUp = (notes.Last().Pitch == Lower);
            denyUp |= ((notes.Last().Pitch.isTritone) && (Higher.isTritone));

            if (denyDown || denyUp)
            {
                resFreq = freqs;
                foreach (Note key in freqs.Keys.ToList())
                {
                    if (
                        (denyDown && (key.Pitch < Lower))
                        ||
                        (denyUp && (key.Pitch > Higher))
                        )
                        resFreq[key] *= 0.01;
                }

                return resFreq;
            }
            else
                return freqs;
        }

        private NotesFreq CheckForManyQuarters(NotesFreq freqs)
        {
            NotesFreq resFreq;

            if (notes.Count < 6)
                return freqs;

            int quarterCount = 0;
            int curr = notes.Count - 1;
            double coeff = 1;

            while (curr > 0)
            {
                if (notes[curr].Duration == 2)
                    quarterCount++;
                else
                    break;

                curr--;
            }

            if (quarterCount < 6)
                return freqs;

            switch (quarterCount)
            {
                case 6: coeff = 0.4; break;
                case 7: coeff = 0.1; break;
                default: coeff = 0; break;
            }

            resFreq = freqs;

            foreach (Note key in freqs.Keys.ToList())
            {
                if (key.Duration == 2)
                    resFreq[key] *= coeff;
            }

            return resFreq;
        }

        private NotesFreq CheckForTrill(NotesFreq freqs)
        {
            if (notes.Count < 3)
                return freqs;

            NotesList last3 = notes.GetRange(notes.Count - 3, 3);
            NotesFreq resFreq = freqs;

            if (last3[0].Pitch == last3[2].Pitch)
            {
                foreach (Note key in freqs.Keys.ToList())
                {
                    if (key.Pitch == last3[1].Pitch)
                        resFreq[key] *= 0.01; // один раз на сотню можно и трель
                }
            }

            return resFreq;
        }

        // не разрешаем четверть-четверть половина
        private NotesFreq CheckForSyncope(NotesFreq freqs)
        {
            NotesFreq resFreq = freqs;
            NotesList last3;
            if (notes.Count == 2)
            {
                last3 = notes.GetRange(notes.Count - 2, 2);
                last3.Insert(0, new Note(last3[1].Pitch, last3[1].TimeStart, 4));
            }
            else if (notes.Count > 2)
                last3 = notes.GetRange(notes.Count - 3, 3);
            else
                return freqs;


            if (
                (last3[1].TimeStart.Beat == 0) &&
                (last3[1].Duration == 2) &&
                (last3[2].Duration == 2) &&
                (last3[0].Duration != 2)
                )
            {
                // запретить всё что больше половины
                foreach (Note key in freqs.Keys.ToList())
                {
                    if (key.Duration > 2)
                        resFreq[key] *= 0.01; // один раз на сотню можно
                }
            }

            return freqs;
        }

        private void chooseFrom(NotesFreq allowed)
        {
            double freqS = allowed.Sum(kv => kv.Value);
            double r = rand.NextDouble() * freqS;
            double accumulator = 0;

            foreach (KeyValuePair<Note, double> kv in allowed)
            {
                if (kv.Value <= 0.005)
                    continue;
                accumulator += kv.Value;
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
                if ((n.Leap.Degrees > 0) && (notes.Last().Leap.Degrees <= 0))
                    Lower = n.Pitch;
                if ((n.Leap.Degrees < 0) && (notes.Last().Leap.Degrees >= 0))
                    Higher = n.Pitch;
            }
            else
            {
                if (n.Pitch > Higher)
                    Higher = n.Pitch;
                if (n.Pitch < Lower)
                    Lower = n.Pitch;
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
                    notes.Last().NextNotes[n] = 0;
            }
            else
                FirstNote();
        }
    }
}


