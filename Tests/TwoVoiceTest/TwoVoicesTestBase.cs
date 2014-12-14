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

            var t = new TwoVoices(c1, c2, m, v1.Time);

            t.SetLength(length);

            // voice1 — base iterator
            // voice2 — aux

            var i1 = v1.Notes.GetEnumerator();
            var i2 = v2.Notes.GetEnumerator();

            Note last1 = null;
            Note last2 = null;

            while (i1.Current != null && i2.Current != null)
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
                        tn = new TwoNotes(i1.Current, last2);
                        i1.MoveNext();
                    }
                    if (last.Note1.TimeEnd > last.Note2.TimeEnd)
                    {
                        tn = new TwoNotes(last1, i2.Current);
                        i2.MoveNext();
                    }
                }

                var f = t.Freqs;
                IsAllowed(tn, f);
                t.AddTwoNotes(tn);
                t.Filter();
            }

            return t;
        }
    }
}
