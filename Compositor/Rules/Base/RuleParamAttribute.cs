using System;

namespace Compositor.Rules.Base
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RuleParamAttribute : Attribute
    {
        public string ParamName;
        public Type Type;
        public object Default;
        public string Desc;

        public RuleParamAttribute(string name, Type type, object def, string desc)
        {
            if ( !type.IsInstanceOfType(def))
                throw new ArgumentException();

            ParamName = name;
            Type = type;
            Default = def;
            Desc = desc;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    class RuleDescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public RuleDescriptionAttribute(string desc)
        {
            Description = desc;
        }
    }


}
