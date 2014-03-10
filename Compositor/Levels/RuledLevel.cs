using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Rules.Base;

namespace Compositor.Levels
{
    public interface IDeniable
    {
        IRule DeniedRule { get; set; }
        bool IsBanned { get; set; }
    }

    public abstract class RuledLevel<T, TN> where T: RuledLevel<T, TN> where TN: IDeniable
    {
        internal static List<IRule<T, TN>> Rules;

        //public bool isBanned = false;
        protected bool Filtered;
        public Dictionary<TN, double> Freqs { get; protected set; }

        private const double MinimumFrequency = 0.01;

        protected RuledLevel()
        {
            Filtered = false;

            if (Rules == null)
            {
                Rules = new List<IRule<T, TN>>();
                AddRules();
            }

            Freqs = new Dictionary<TN, double>();
        }

        private void AddRules()
        {
            var attrs = (RuleAttribute[])Attribute.GetCustomAttributes(GetType(), typeof(RuleAttribute));

            var expectedInterface = typeof(IRule<T, TN>);

            foreach (var t in attrs)
            {
                var ruleClass = t.Value;
                var interfaces = ruleClass.GetInterfaces();

                if (!interfaces.Contains(expectedInterface)) continue;

                var instance = (IRule<T, TN>)Activator.CreateInstance(ruleClass);
                Rules.Add(instance);
            }
        }

        protected abstract void AddVariants(bool dumpResult = false);

        public Dictionary<TN, double> Filter(bool dumpResult = false)
        {
            if (Filtered)
                return Freqs;

            AddVariants(dumpResult);

            var toFilter =
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
                            double freq = r.Apply(n);
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


            Filtered = true;
            return Freqs;
        }

        public void Ban(TN what)
        {
            if (!Freqs.ContainsKey(what)) return;

            Freqs[what] = 0;
            what.IsBanned = true;
        }
    }
}
