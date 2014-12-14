using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Compositor.Levels;
using PitchBase;

namespace NoteRulesTest
{
    public class MelodyTestBase : TestBase<Note>
    {
        protected static int[] ParseNotes(List<Pitch> diapason, string notesString, bool perfectTime)
        {
            var result = new List<int>();
            var parserState = 0; //0: whitespace, 1: note, 2: octave modifier, 3: duration, 4: dotted duration
            var name = new StringBuilder(3);
            var durationString = new StringBuilder(2);

            var pitchIndex = 0;
            var duration = 4;
            var modifier = 0;

            var t = Time.Create(perfectTime);


            for (var index = 0; index <= notesString.Count(); index++)
            {
                char ch;
                ch = (index == notesString.Count()) ? ' ' : notesString[index];

                if (parserState == 0) // if whitespace 
                {
                    if (ch == ' ')
                        continue;
                    if ("abcdefghr".Contains(ch))
                    {
                        parserState = 1;
                        modifier = 0;
                        name.Clear();
                        name.Append(ch);
                        continue;
                    }
                }
                if (parserState == 1)
                {
                    if ("ies".Contains(ch))
                    {
                        name.Append(ch);
                        continue;
                    }
                    if ("',".Contains(ch))
                    {
                        parserState = 2;
                        if (ch == ',')
                            modifier -= 7;
                        else
                            modifier += 7;
                        continue;
                    }
                }
                if (parserState > 0 && parserState < 4)
                {
                    if ("01248".Contains(ch))
                    {
                        if (parserState != 3)
                            durationString.Clear();
                        parserState = 3;
                        durationString.Append(ch);
                        continue;
                    }
                    if (ch == '.')
                    {
                        parserState = 4;
                        continue;
                    }
                }

                if (parserState > 0) {
                    if (ch == ' ')
                    {
                        if (durationString.Length > 0)
                        {
                            int.TryParse(durationString.ToString(), out duration);
                            duration = (duration == 0) ? 16 : 8/duration;
                            if (parserState == 4)
                                duration += duration/2;
                        }
                        var minNoteIndex = Math.Max(0, pitchIndex + modifier - 3);
                        var maxNoteIndex = Math.Min(diapason.Count, pitchIndex + modifier + 4);
                        var nameS = name.ToString();
                        if (nameS.Equals("r"))
                        {
                            result.Add(-1); //rest
                            result.Add(duration);
                            continue;
                        }
                        var found = false;
                        for (var i = minNoteIndex; i < maxNoteIndex; i++)
                        {
                            if (diapason[i].StringForm.Equals(nameS))
                            {
                                pitchIndex = i;
                                t += duration;
                                result.Add(i);
                                result.Add(duration);
                                parserState = 0;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            continue;

                        throw new ArgumentException(String.Format("Note specified as {0}{1}{2} is out of diapason (base: {3})",
                            nameS, (modifier == 7) ? "'" : ((modifier == -7) ? "," : ""), durationString, diapason[pitchIndex]));
                    }
                }

                throw new ArgumentException(String.Format("Invalid notesString at {0}", index));
            }

            return result.ToArray();
        }

        protected static List<Note> CreateNotes(Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, string noteString)
        {
            var pf = new PitchFactory(m, c);
            diapason = pf.Pitches;

            return CreateNotes(m, c, perfectTime, out diapason, ParseNotes(diapason, noteString, perfectTime));
        }


        protected static List<Note> CreateNotes(Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, params int[] infoList)
        {
            var result = new List<Note>();
            var pf = new PitchFactory(m, c);
            diapason = pf.Pitches;
            Note prev = null;
            var t = Time.Create(perfectTime);
            var pos = 0;
            var isPos = true;
            
            foreach (var ni in infoList)
            {
                if (isPos)
                {
                    pos = ni;
                    isPos = false;
                    continue;
                }

                var n = new Note((pos == -1) ? null : diapason[pos], t, ni, prev);
                if (prev != null)
                    IsAllowed(n, prev.Freqs);

                Console.WriteLine("Adding {0} on {1}\n", n, n.TimeStart);
                t += ni;
                n.Diapason = diapason;
                n.Filter();
                prev = n;
                result.Add(n);
                isPos = true;
            }

            return result;
        }

        public static Voice CreateMelody(uint length, Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, string noteString, VoiceType type = VoiceType.Single)
        {
            var pf = new PitchFactory(m, c);
            diapason = pf.Pitches;

            return CreateMelody(length, m, c, perfectTime, out diapason, type, ParseNotes(diapason, noteString, perfectTime));
        }

        public static Voice CreateMelody(uint length, Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, params int[] infoList)
        {
            return CreateMelody(length, m, c, perfectTime, out diapason, VoiceType.Single, infoList);
        }

        public static Voice CreateMelody(uint length, Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, VoiceType type, params int[] infoList)
        {
            var notes = CreateNotes(m, c, perfectTime, out diapason, infoList);

            var mel = new Voice(c, m, Time.Create(perfectTime), type);
            mel.SetLength(length);

            var first = true;

            Console.WriteLine("\n\nChecking melody");
            foreach (var n in notes)
            {
                var freqs = mel.Filter();
                Console.WriteLine("Checking {0} on {1} in melody...", n, n.TimeStart);

                if (!first)
                    IsAllowed(n, freqs);

                mel.AddNote(n);
                Console.WriteLine(" Reserve: {0}, Uncomp: {1}\n", n.Reserve, n.Uncomp);

                first = false;
            }

            return mel;
        }

        protected static void AppendNote(Voice m, Pitch pitch, int duration)
        {
            var prev = m.Notes.Last();
            var n = new Note(pitch, m.Time, duration, prev);

            IsAllowed(n, prev.Freqs);
            Console.WriteLine("Adding {0} on {1}\n", n, n.TimeStart);

            var freqs = m.Filter();
            Console.WriteLine("Checking {0} on {1} in melody...", n, n.TimeStart);
            IsAllowed(n, freqs);
            
            m.AddNote(n);
        }
    }
}