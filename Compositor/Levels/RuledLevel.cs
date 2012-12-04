using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Helpers;

namespace Compositor.Levels
{
    public abstract class RuledLevel<T> where T: RuledLevel<T>
    {
        internal static List<Rule<T>> Rules;

        public bool isBanned = false;
        protected bool filtered;
        public Dictionary<Note, double> Freqs { get; protected set; }

        private const double MinimumFrequency = 0.01;

        public RuledLevel()
        {
            filtered = false;

            if (Rules == null)
            {
                Rules = new List<Rule<T>>();
                AddRules();
            }

            Freqs = new Dictionary<Note, double>();
        }

        private void AddRules()
        {
            RuleAttribute[] attrs = (RuleAttribute[])Attribute.GetCustomAttributes(this.GetType(), typeof(RuleAttribute));

            Type ruleClass;
            Type[] interfaces;
            Type expectedInterface = typeof(Rule<T>);
            Rule<T> instance;

            foreach (RuleAttribute t in attrs)
            {
                ruleClass = t.Value;
                interfaces = ruleClass.GetInterfaces();

                if (interfaces.Contains(expectedInterface))
                {
                    instance = (Rule<T>)System.Activator.CreateInstance(ruleClass);
                    Rules.Add(instance);
                }
            }
        }

        protected abstract void AddVariants();

        public Dictionary<Note, double> Filter()
        {
            if (filtered)
                return Freqs;

            double freq;

            AddVariants();

            IEnumerable<Note> toFilter =
                from kv in Freqs.ToList()
                where kv.Value > MinimumFrequency
                select kv.Key;

            foreach (Rule<T> r in Rules)
            {
                r.Init((T)this);
                if (r.IsApplicable())
                    foreach (Note n in toFilter)
                    {
                        /*if (Freqs[n] == 0)
                            break;*/

                        freq = r.Apply(n);
                        if (freq != 1)
                            Console.WriteLine("Rule {0} to note {1} (@ {2}) = {3:F}",
                                r.GetType().Name, n.ToString(), n.TimeStart.Position, freq);
                        Freqs[n] *= freq;
                    }
            }
            
            filtered = true;
            return Freqs;
        }

        public void ban(Note what)
        {
            if (Freqs.ContainsKey(what))
            {
                Freqs[what] = 0;
                what.isBanned = true;
            }
        }
    }
}
