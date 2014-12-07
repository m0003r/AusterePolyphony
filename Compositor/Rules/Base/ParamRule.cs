using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;

namespace Compositor.Rules.Base
{
    abstract public class ParamRule : IRule, IParamRule
    {
        public static ISettingsProvider SettingsProvider;

        public abstract double Apply(IDeniable nextNotes);
        public abstract bool IsApplicable();
        public abstract void Init(IDeniable me);
        public abstract bool Initiable(IDeniable level);

        public bool Enabled = true;

        public int DeniedTimes { get; private set; }
        public int ResetDenied()
        {
            var deniedTimes = DeniedTimes;
            DeniedTimes = 0;
            return deniedTimes;
        }


        public void Denied()
        {
            DeniedTimes++;
        }

        protected Dictionary<String, RuleParameter> Settings;
        
        protected ParamRule()
        {
            DeniedTimes = 0;
            Settings = new Dictionary<string, RuleParameter>();

            foreach (var param in GetParams().Select(p => new RuleParameter(p)))
                Settings[param.Name] = param;

            if (SettingsProvider == null) return;

            if (SettingsProvider.HasSetting(GetName(), "enabled"))
                Enabled = (bool) SettingsProvider.GetSetting(GetName(), "enabled");
            else
                SettingsProvider.SetSetting(GetName(), "enabled", true);

            foreach (var key in Settings.Keys)
            {
                if (SettingsProvider.HasSetting(GetName(), key))
                    Settings[key].Value = SettingsProvider.GetSetting(GetName(), key);
                else
                    SettingsProvider.SetSetting(GetName(), key, Settings[key].Value);
            }
        }

        public RuleParamAttribute[] GetParams()
        {
            return (RuleParamAttribute[])Attribute.GetCustomAttributes(GetType(), typeof(RuleParamAttribute));
        }    
    
        public void SetParam(string name, object value)
        {
            Settings[name].Value = value;
        }

        public string GetName()
        {
            return GetType().Name;
        }

        public string GetDescription()
        {
            var rn = (RuleDescriptionAttribute)Attribute.GetCustomAttribute(GetType(), typeof(RuleDescriptionAttribute));

            return rn == null ? null : rn.Description;
        }
    }
}