using System;
using System.Collections.Generic;
using Compositor.Levels;

namespace Compositor.Rules.Base
{
    abstract class ParamRule : IRule, IParamRule
    {
        public abstract void Init(IDeniable me);
        public abstract double Apply(IDeniable nextNotes);
        public abstract bool IsApplicable();
        public abstract bool Initiable(IDeniable level);

        public bool Enabled = true;

        protected Dictionary<string, object> Settings;
        private readonly Dictionary<string, Type> _types;

        protected ParamRule()
        {
            /*Settings = new Dictionary<string, object>();
            _types = new Dictionary<string, Type>();
            var defaults = new Dictionary<string, object>();

            var attrs = GetParams();

            foreach (var t in attrs)
            {
                defaults[t.ParamName] = t.Default;
                _types[t.ParamName] = t.Type;
                Settings[t.ParamName] = t.Default;
            }*/
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

            return rn == null ? null : rn.ParamName;
        }
    }
}