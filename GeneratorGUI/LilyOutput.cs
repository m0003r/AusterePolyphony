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

        public static string Lily(this MelodyGenerator Generator)
        {
            Pitch p = new Pitch(0, Generator.Melody.Modus);

            string key = p.StringForm;
            string modus, clef, time;
            StringBuilder notes = new StringBuilder();

            modus = "\\" + Generator.Melody.Modus.Name;
            clef = clefNamesList[(int)Generator.Melody.Clef + 1];
            time = (Generator.Melody.Time.Beats.ToString()) + "/2";

            foreach (Note n in Generator.Melody.Notes)
            {
                notes.Append(n);
                notes.AppendFormat("^\"{0}\"", n.Reserve);
                notes.AppendFormat("_\"{0}\"", n.Uncomp);
                notes.Append(" ");
            }

            string format = Resources.ScoreTemplate;
            return string.Format(format, Generator.Seed, key, modus, clef, time, notes);

        }
    }
}
