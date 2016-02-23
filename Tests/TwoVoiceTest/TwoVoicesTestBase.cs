using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoteRulesTest;
using PitchBase;

namespace TwoVoiceTest
{
    public class TwoVoicesTestBase : TestBase<TwoNotes>
    {
        internal static TwoVoices CreateVoices(uint length, Modus m, Clef c1, Clef c2, bool perfectTime, out List<Pitch> diapason1,
            out List<Pitch> diapason2, string voice1, string voice2)
        {
            var v1 = MelodyTestBase.CreateMelody(length, m, c1, perfectTime, out diapason1, voice1, VoiceType.Top);
            var v2 = MelodyTestBase.CreateMelody(length, m, c2, perfectTime, out diapason2, voice2, VoiceType.Bass);

            var t = new TwoVoices(c1, c2, m, Time.Create(perfectTime));
            t.SetFragment(t.Time);

            t.SetLength(length);

            // voice1 — base iterator
            // voice2 — aux

            var i1 = v1.Notes.GetEnumerator();
            var i2 = v2.Notes.GetEnumerator();

            i1.MoveNext();
            i2.MoveNext();

            Note last1 = null;
            Note last2 = null;

            while (i1.Current != null || i2.Current != null)
            {
                TwoNotes tn = null;

                if (t.TwoNotes.Count == 0 || t.TwoNotes.Last().EndSimult)
                {
                    tn = new TwoNotes(i1.Current, i2.Current);

                    last1 = i1.Current;
                    last2 = i2.Current;
                    i1.MoveNext();
                    i2.MoveNext();
                }
                else
                {
                    var last = t.TwoNotes.Last();
                    if (last.Note1.TimeEnd < last.Note2.TimeEnd)
                    {
                        if (i1.Current == null)
                            break;

                        tn = new TwoNotes(i1.Current, last2);
                        i1.MoveNext();
                    }
                    if (last.Note1.TimeEnd > last.Note2.TimeEnd)
                    {
                        if (i2.Current == null)
                            break;

                        tn = new TwoNotes(last1, i2.Current);
                        i2.MoveNext();
                    }
                }

                var f = t.Freqs;
                Console.WriteLine("\n Checking {0} in TwoVoices...", tn);
                IsAllowed(tn, f);
                t.AddTwoNotes(tn);
                t.Filter();
            }

            return t;
        }

        internal static TwoVoices CreateVoices(uint length, Modus m, Clef c1, Clef c2, bool perfectTime, string v1,
            string v2)
        {
            List<Pitch> diapason1, diapason2;
            return CreateVoices(length, m, c1, c2, perfectTime, out diapason1, out diapason2, v1, v2);
        }
    }
}
