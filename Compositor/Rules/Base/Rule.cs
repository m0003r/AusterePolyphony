using System;
using System.Collections.Generic;

namespace Compositor.Rules.Base
{
    interface IRule<in T, in TN> : IRule
    {
        void Init(T me);
        double Apply(TN nextNotes);
    }

    public interface IRule
    {
        bool IsApplicable();
    }

    public interface IParamRule
    {
        ParamAttribute[] GetParams();
        string GetName();

        void SetParam(string n, object v);
    }

    abstract class ParamRule<T, TN> : IRule<T, TN>, IParamRule
    {
        public abstract void Init(T me);
        public abstract double Apply(TN nextNotes);
        public abstract bool IsApplicable();

        public bool Enabled = true;

        protected Dictionary<string, object> Settings;
        private readonly Dictionary<string, Type> _types;

        protected ParamRule()
        {
            Settings = new Dictionary<string, object>();
            _types = new Dictionary<string, Type>();
            var defaults = new Dictionary<string, object>();

            ParamAttribute[] attrs = GetParams();

            foreach (ParamAttribute t in attrs)
            {
                defaults[t.ParamName] = t.Default;
                _types[t.ParamName] = t.Type;
                Settings[t.ParamName] = t.Default;
            }
        }

        public ParamAttribute[] GetParams()
        {
            return (ParamAttribute[])Attribute.GetCustomAttributes(GetType(), typeof(ParamAttribute));
        }    
    
        public void SetParam(string name, object value)
        {
            if (!value.GetType().IsSubclassOf(_types[name]))
                throw new ArgumentException();

            Settings[name] = value;
        }

        public string GetName()
        {
            var rn = (RuleNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(RuleNameAttribute));

            if (rn == null)
                return null;

            return rn.ParamName;
        }
    }

}
