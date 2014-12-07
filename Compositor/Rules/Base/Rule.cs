using Compositor.Levels;

namespace Compositor.Rules.Base
{
    public interface IRule
    {
        bool IsApplicable();
        void Init(IDeniable me);
        double Apply(IDeniable nextNotes);
        bool Initiable(IDeniable level);
        void Denied();
        int DeniedTimes { get; }
        int ResetDenied();
    }

    public interface IParamRule
    {
        RuleParamAttribute[] GetParams();
        string GetName();

        void SetParam(string n, object v);
    }
}
