using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace NotesTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Pitch n, f;
            Interval d;
            List<Modus> modi= new List<Modus>();

            modi.Add(Modus.Ionian(2));
            modi.Add(Modus.Dorian(3));
            modi.Add(Modus.Phrygian(4));
            modi.Add(Modus.Lydian(5));
            modi.Add(Modus.Mixolydian(6));
            modi.Add(Modus.Aeolian(7));

            foreach (Modus m in modi)
            {
                f = new Pitch(0, m);
                for (int i = -3; i < 8; i++)
                {
                    n = new Pitch(i, m);
                    Console.Write("<" + f.ToString() + " " + n.ToString() + "> = ");
                    d = n - f;
                    Console.WriteLine(d.ToString());
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}

