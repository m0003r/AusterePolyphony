using Compositor.Levels;
using PitchBase;

namespace Compositor.Rules.Base
{
    abstract class NoteRule : ParamRule<Note, Note>
    {
        protected Note Me;

        protected Time Time { get { return Me.TimeStart; } }
        protected int Duration { get { return Me.Duration; } }
        protected Pitch Pitch { get { return Me.Pitch; } }
        protected Interval Leap { get { return Me.Leap; } }

        public override void Init(Note me)
        {
            Me = me;
        }

        public abstract override bool IsApplicable();
        public abstract override double Apply(Note nextNotes);
    }
}
