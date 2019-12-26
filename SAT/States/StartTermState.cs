using SAT.States;
using System;

namespace SAT.States
{
    class StartTermState : AbstractParserState
    {
        public StartTermState(IStateMachine parent) : base(parent)
        {
        }
        public override void ProcessState(IStateContext context)
        {
            IParseString ps = context.GetParseString();
            if (!ps.HasChar())
            {
                GetParent().SetError("Missed number of variable at } of expression'");
                ChangeState(new ErrorState(GetParent()));
                return;
            }
            if (Char.IsDigit(ps.PeekChar()))
            {
                ps.ClearTerm();
                ps.CollectTerm();
                ChangeState(new CollectTermState(GetParent()));
            }
            else
            {
                GetParent().SetError("Unexpected char at position " + ps.GetPosition().ToString());
                ChangeState(new ErrorState(GetParent()));
            }
        }
    }
    
}
