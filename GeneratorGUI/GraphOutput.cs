using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Compositor.Generators;
using Compositor.Levels;

namespace GeneratorGUI
{
    public static class GraphOutputExtension
    {
        private static string _graph = "";
        private const string Prelude = "digraph \"Seed {0}\" {{\r\naspect=\"1.0\"\r\n";
        private const string Postlude = "}";
        private static Dictionary<string, bool> _drawed;

        private static string MakeNodeLabel(Note n)
        {
            string r = "<" + n + "<br /><font point-size=\"10\">";
            r += "t: " + n.TimeStart.Position + "<br />";
            r += "d: " + (n.DeniedRule);
            return r + "</font>>";
        }

        private static string MakeNoteName(Note n)
        {
            return (n.Pitch == null ? "r" : (n.Pitch.StringForm + n.Pitch.ModusOctave)) + n.Duration + n.TimeStart.Position;
        }

        private static string ChanceToColor(double chance)
        {
            if (chance < 0)
                return " style=\"filled\" color=\"1,0,0.6\"";

            double hue;
            if (chance < 1)
                hue = chance * 0.5;
            else
                hue = 0.6 - 0.1 / chance;

            string sc = hue.ToString((CultureInfo.CreateSpecificCulture("en-GB")));
            return " style=\"filled\" color=\"" + sc + ",1,1\"";
        }

        private static string MakeNode(Note n, Note prev = null, double val = 1)
        {
            string res = "  " + MakeNoteName(n) + "[label=" + MakeNodeLabel(n);
            res += ChanceToColor(n.IsBanned ? -1 : val);
            if (n.IsLower)
                res += " shape=\"invhouse\"";
            if (n.IsHigher)
                res += " shape=\"house\"";
            res += "]";

            if (prev != null)
            {
                res += "\r\n";
                res += "  " + (MakeNoteName(prev) + " -> " + MakeNoteName(n));
            }

            _drawed[MakeNoteName(n)] = true;

            return res;
        }


        private static string MakeNote(Note n)
        {
            string res = "";

            if (!_drawed.ContainsKey(MakeNoteName(n))) //need note node
                res = MakeNode(n) + "\r\n";

            if (n.Freqs != null)
            {
                foreach (var kv in
                    n.Freqs.Where(kv => (kv.Value > 0.03) || (kv.Key.IsBanned))   
                    )
                    {
                    res += (MakeNode((Note)kv.Key, n, kv.Value) + "\r\n");
                    if (kv.Key.IsBanned)
                        MakeNote((Note)kv.Key);
                    }
            }

            return res;
        }

        public static string GenerationGraph(this MelodyGenerator me)
        {
            _graph = string.Format(Prelude, me.Seed);

            _drawed = new Dictionary<string, bool>();

            foreach (Note n in me.Voice.Notes)
            {
                _graph += MakeNote(n);
                _graph += "\r\n";
            }

            return _graph + Postlude;
        }
    }
}
