using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Rules;

namespace Compositor.Levels
{
    public interface IDeniable
    {
        Rule DeniedRule { get; set; }
        bool isBanned { get; set; }
    }

    public abstract class RuledLevel<T, N> where T: RuledLevel<T, N> where N: IDeniable
    {
        internal static List<Rule<T, N>> Rules;

        //public bool isBanned = false;
        protected bool filtered;
        public Dictionary<N, double> Freqs { get; protected set; }

        private const double MinimumFrequency = 0.01;

        public RuledLevel()
        {
            filtered = false;

            if (Rules == null)
            {
                Rules = new List<Rule<T, N>>();
                AddRules();
            }

            Freqs = new Dictionary<N, double>();
        }

        private void AddRules()
        {
            RuleAttribute[] attrs = (RuleAttribute[])Attribute.GetCustomAttributes(this.GetType(), typeof(RuleAttribute));

            Type ruleClass;
            Type[] interfaces;
            Type expectedInterface = typeof(Rule<T, N>);
            Rule<T, N> instance;

            foreach (RuleAttribute t in attrs)
            {
                ruleClass = t.Value;
                interfaces = ruleClass.GetInterfaces();

                if (interfaces.Contains(expectedInterface))
                {
                    instance = (Rule<T, N>)System.Activator.CreateInstance(ruleClass);
                    Rules.Add(instance);
                }
            }
        }

        protected abstract void AddVariants(bool dumpResult = false);

        public Dictionary<N, double> Filter(bool dumpResult = false)
        {
            if (filtered)
                return Freqs;

            double freq;

            AddVariants(dumpResult);

            IEnumerable<N> toFilter =
                from kv in Freqs.ToList()
                where kv.Value >= MinimumFrequency
                select kv.Key;

            Timer.Start("filter");

            Rules
                //cause strage problems
                //.AsParallel().ForAll(r =>
                .ForEach(r =>
            {
                r.Init((T)this);
                if (r.IsApplicable())
                    toFilter
                        //cause strage problems
                        //.AsParallel().ForAll(n =>
                        .ToList().ForEach(n =>
                    {
                        if (Freqs[n] >= MinimumFrequency)
                        {
                            freq = r.Apply(n);
                            /*if (dumpResult)
                                Console.WriteLine("Rule {0} to note {1} (@ {2}) = {3:F}",
                                    r.GetType().Name, n.ToString(), n.TimeStart.Position, freq);*/

                            Freqs[n] *= freq;

                            if (freq == 0)
                            {
                                n.DeniedRule = r;
                            }
                        }
                    });
            });

            Timer.Stop("filter");


            filtered = true;
            return Freqs;
        }

        public void ban(N what)
        {
            if (Freqs.ContainsKey(what))
            {
                Freqs[what] = 0;
                what.isBanned = true;
            }
        }
    }
}
