using Compositor.Levels;

namespace Compositor.Rules.Base
{
    public interface IRule
    {
        bool IsApplicable();
        void Init(IDeniable me);
        double Apply(IDeniable nextNotes);
        bool Initiable(IDeniable level);
    }

    public interface IParamRule
    {
        ParamAttribute[] GetParams();
        string GetName();

        void SetParam(string n, object v);
    }
}
