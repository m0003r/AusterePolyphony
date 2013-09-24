using GeneratorGUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Compositor;
using Compositor.Levels;
using PitchBase;

namespace GeneratorGUI
{
    static class LilyOutput
    {
        static string[] clefNamesList = { "treble", "soprano", "mezzosoprano", "alto", "tenor", "baritone", "bass" };



        public static string Lily(IGenerator Generator)
        {
            var voices = new StringBuilder();
            string ScoreFormat = Resources.ScoreTemplate;
            string VoiceFormat = Resources.VoiceTemplate;

            var Melodies = Generator.GetNotes();
            foreach (var m in Melodies)
            {
                Pitch p = new Pitch(0, m.Modus);

                string key = p.StringForm;
                string modus, clef, time;
                StringBuilder notes = new StringBuilder();

                modus = "\\" + m.Modus.Name;
                clef = clefNamesList[(int)m.Clef + 1];
                time = (m.Time.Beats.ToString()) + "/2";

                foreach (Note n in m.Notes)
                {
                    notes.Append(n);
                    //notes.AppendFormat("^\"{0}\"", n.Reserve);
                    //notes.AppendFormat("_\"{0}\"", n.Uncomp);
                    notes.Append(" ");
                }

                voices.AppendFormat(VoiceFormat, key, modus, clef, time, notes);
            }


            return string.Format(ScoreFormat, Generator.GetSeed(), voices);
        }
    }
}
