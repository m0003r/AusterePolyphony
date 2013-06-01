using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Melody;

namespace NotesTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MelodyGenerator mg = new MelodyGenerator(Clef.Treble, Modus.Dorian(2), Time.Create());

            mg.Generate(20);

            foreach (Note n in mg.Notes)
            {
                Console.Write(n.ToString() + " ");
            }

            Console.ReadKey();

            /*Pitch n, f;
            Interval d;
            List<Modus> modi= new List<Modus>();

            modi.Add(Modus.Dorian(2));
            modi.Add(Modus.Phrygian(-3));
            modi.Add(Modus.Lydian(-4));
            modi.Add(Modus.Mixolydian(-5));
            modi.Add(Modus.Aeolian(-6));

            foreach (Modus m in modi)
            {
                f = new Pitch(14, m);
                for (int i = 14; i < 29; i++)
                {
                    n = new Pitch(i, m);
                    Console.Write("<" + f.ToString() + " " + n.ToString() + "> = ");
                    d = n - f;
                    Console.WriteLine(d.ToString());
                }
                Console.WriteLine();
            }

            Console.ReadKey();

            PitchFactory pf;

            pf = new PitchFactory(Modus.Phrygian(4), Clef.Tenor);

            foreach (Pitch p in pf.Pitches)
            {
                Console.Write(p.ToString() + " ");
            }

            Console.ReadKey();*/
        }
    }
}

