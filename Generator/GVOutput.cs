using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Notes;
using Melody;

namespace Generator
{
    public static class GVOutput
    {
        private static string graph = "";
        private static readonly string prelude = "digraph \"Seed {0}\" {{\r\naspect=\"1.0\"\r\n";
        private static readonly string postlude = "}";
        private static Dictionary<string, bool> drawed;

        private static string MakeNodeLabel(Note n)
        {
            string r = "<" + n.ToString() + "<br /><font point-size=\"10\">";
            r += "l: " + n.Leap.Degrees.ToString() + "<br />";
            r += "t: " + n.TimeStart.Position.ToString() + "<br />";
            r += "d: " + n.Duration.ToString() + "<br />";
            r += "6: " + (n.Pitch.isTritone?"yes":"no");
            return r + "</font>>";
        }

        private static string MakeNoteName(Note n)
        {
            return n.Pitch.Degree.ToString() + n.Pitch.ModusOctave.ToString() + n.Duration.ToString() + n.TimeStart.Position.ToString();
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
            string res;

            res = "  " + MakeNoteName(n) + "[label=" + MakeNodeLabel(n);
            res += ChanceToColor(n.isBanned ? -1 : val);
            if (n.isLower)
                res += " shape=\"invhouse\"";
            if (n.isHigher)
                res += " shape=\"house\"";
            res += "]";

            if (prev != null)
            {
                res += "\r\n";
                res += "  " + (MakeNoteName(prev) + " -> " + MakeNoteName(n));
            }

            drawed[MakeNoteName(n)] = true;

            return res;
        }


        private static string MakeNote(Note n)
        {
            string res = "";

            if (!drawed.ContainsKey(MakeNoteName(n))) //need note node
                res = MakeNode(n) + "\r\n";

            if (n.NextNotes != null)
            {
                foreach (KeyValuePair<Note, double> kv in n.NextNotes)
                    if ((kv.Value > 0.001) || (kv.Key.isBanned)) // отрицательные значения — тупики
                    {
                        res += (MakeNode(kv.Key, n, kv.Value) + "\r\n");
                        if (kv.Key.isBanned)
                            MakeNote(kv.Key);
                    }
            }

            return res;
        }

        public static string GenerationGraph(this MelodyGenerator me)
        {
            graph = string.Format(prelude, me.seed);

            drawed = new Dictionary<string, bool>();

            foreach (Note n in me.Notes)
            {
                graph += MakeNote(n);
                graph += "\r\n";
            }

            return graph + postlude;
        }
    }
}
