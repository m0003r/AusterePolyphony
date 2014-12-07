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
        List<Tuple<IRule, double>> AppliedRules { get; set; }
    }

    public abstract class RuledLevel : IFilterable
    {
        public static List<IRule> Rules;
        internal static HashSet<Type> RulesAdded;

        protected internal bool Filtered;
        public FreqsDict Freqs { get; protected set; }

        public IRule DeniedRule { get; set; }
        public List<Tuple<IRule, double>> AppliedRules { get; set; }
        public bool IsBanned { get; set; }


        private const double MinimumFrequency = 0.01;

        protected RuledLevel()
        {
            Filtered = false;

            if (Rules == null)
            {
                Rules = new List<IRule>();
                RulesAdded = new HashSet<Type>();
            }
            if (!RulesAdded.Contains(GetType()))
            {
                RulesAdded.Add(GetType());
                AddRules();
            }


            AppliedRules = new List<Tuple<IRule, double>>();
            Freqs = new FreqsDict();
        }

        private void AddRules()
        {
            var attrs = (RuleAttribute[])Attribute.GetCustomAttributes(GetType(), typeof(RuleAttribute));

            var expectedInterface = typeof(IRule);

            foreach (var t in attrs)
            {
                var ruleClass = t.Value;
                var interfaces = ruleClass.GetInterfaces();

                if (!interfaces.Contains(expectedInterface)) continue;

                var instance = (IRule)Activator.CreateInstance(ruleClass);
                Rules.Add(instance);
            }
        }

        public abstract void AddVariants();

        public virtual FreqsDict Filter()
        {
            if (Filtered)
                return Freqs;

            AddVariants();

            var toFilter =
                from kv in Freqs.ToList()
                where kv.Value >= MinimumFrequency
                select kv.Key;

            Timer.Start("filter");

            //Rules.Sort((a, b) => b.DeniedTimes.CompareTo(a.DeniedTimes));
            Rules
                //cause strage problems
                //.AsParallel().ForAll(r =>
                .ForEach(r =>
            {
                if (!r.Initiable(this)) return;

                r.Init(this);
                if (r.IsApplicable())
                    toFilter
                        //cause strage problems
                        //.AsParallel().ForAll(n =>
                        .ToList().ForEach(n =>
                        {
                            if (Freqs[n] < MinimumFrequency) return;

                            var freq = r.Apply(n);


                            n.AppliedRules.Add(new Tuple<IRule, double>(r, freq));
#if TRACE_RULES
                        Console.WriteLine("Rule {0} to note {1} = {2:F}",
                            r.GetType().Name, n.ToString(), freq);
#endif
                            Freqs[n] *= freq;

                            if (Math.Abs(freq) >= MinimumFrequency) return;

                            n.DeniedRule = r;
                            r.Denied();
                        });
            });

            Timer.Stop("filter");


            Filtered = true;
            return Freqs;
        }

        public void Ban(IDeniable what)
        {
            if (!Freqs.ContainsKey(what)) return;

            Freqs[what] = 0;
            what.IsBanned = true;
        }
    }
}
