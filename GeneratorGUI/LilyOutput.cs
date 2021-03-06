﻿using Compositor.Generators;
using GeneratorGUI.Properties;
using System.Text;
using Compositor.Levels;
using PitchBase;

namespace GeneratorGUI
{
    static class LilyOutput
    {
        static readonly string[] ClefNamesList = { "treble", "soprano", "mezzosoprano", "alto", "tenor", "baritone", "bass" };



        public static string Lily(IGenerator generator)
        {
            var voices = new StringBuilder();
            string scoreFormat = Resources.ScoreTemplate;
            string voiceFormat = Resources.VoiceTemplate;

            var melodies = generator.GetNotes();
            foreach (var m in melodies)
            {
                var p = new Pitch(0, m.Modus);

                var key = p.StringForm;
                var notes = new StringBuilder();

                var modus = "\\" + m.Modus.Name;
                var clef = ClefNamesList[(int)m.Clef + 1];
                var time = (m.Time.Beats) + "/2";

                foreach (var n in m.Notes)
                {
                    notes.Append(n);
                    notes.AppendFormat("^\"{0}\"_\"{1}\"", n.Uncomp, n.Reserve);
                        
                    notes.Append(" ");
                }

                voices.AppendFormat(voiceFormat, key, modus, clef, time, notes);
            }


            return string.Format(scoreFormat, generator.GetSeed(), voices);
        }
    }
}
