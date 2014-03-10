using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor.Rules
{
    interface Rule<T, N> : Rule
    {
        void Init(T me);
        double Apply(N n);
    }

    public interface Rule
    {
        bool IsApplicable();
    }

    public interface ParamRule
    {
        ParamAttribute[] GetParams();
        string GetName();

        void SetParam(string n, object v);
    }

    abstract class ParamRule<T, N> : Rule<T, N>, ParamRule
    {
        public abstract void Init(T me);
        public abstract double Apply(N n);
        public abstract bool IsApplicable();

        public bool enabled = true;

        protected Dictionary<string, object> settings;
        private Dictionary<string, Type> types;
        private Dictionary<string, object> defaults;

        public ParamRule()
        {
            settings = new Dictionary<string, object>();
            types = new Dictionary<string, Type>();
            defaults = new Dictionary<string, object>();

            ParamAttribute[] attrs = GetParams();

            foreach (ParamAttribute t in attrs)
            {
                defaults[t.ParamName] = t.Default;
                types[t.ParamName] = t.Type;
                settings[t.ParamName] = t.Default;
            }
        }

        public ParamAttribute[] GetParams()
        {
            return (ParamAttribute[])Attribute.GetCustomAttributes(this.GetType(), typeof(ParamAttribute));
        }    
    
        public void SetParam(string name, object value)
        {
            if (!value.GetType().IsSubclassOf(types[name]))
                throw new ArgumentException();

            settings[name] = value;
        }

        public string GetName()
        {
            var rn = (RuleNameAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(RuleNameAttribute));

            if (rn == null)
                return null;

            return rn.ParamName;
        }
    }

}
