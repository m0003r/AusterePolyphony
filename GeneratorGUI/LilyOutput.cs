using Compositor.Generators;
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

                string key = p.StringForm;
                string modus, clef, time;
                var notes = new StringBuilder();

                modus = "\\" + m.Modus.Name;
                clef = ClefNamesList[(int)m.Clef + 1];
                time = (m.Time.Beats) + "/2";

                foreach (Note n in m.Notes)
                {
                    notes.Append(n);
                        
                    notes.Append(" ");
                }

                voices.AppendFormat(voiceFormat, key, modus, clef, time, notes);
            }


            return string.Format(scoreFormat, generator.GetSeed(), voices);
        }
    }
}
