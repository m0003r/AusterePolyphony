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

        protected bool filtered;
        protected Dictionary<Note, double> Freqs;

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
            RulesAttribute[] attrs = (RulesAttribute[])Attribute.GetCustomAttributes(this.GetType(), typeof(RulesAttribute));

            Type ruleClass;
            Type[] interfaces;
            Type expectedInterface = typeof(Rule<T>);
            Rule<T> instance;

            foreach (RulesAttribute t in attrs)
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

        public Dictionary<Note, double> FilterVariants()
        {
            if (filtered)
                return Freqs;

            AddVariants();

            IEnumerable<Note> toFilter =
                from kv in Freqs.ToList()
                where kv.Value > 0.01
                select kv.Key;

            foreach (Rule<T> r in Rules)
            {
                r.Init((T)this);
                if (r.IsApplicable())
                    foreach (Note n in toFilter)
                        Freqs[n] *= r.Apply(n);
            }


            filtered = true;
            return Freqs;
        }
    }
}
