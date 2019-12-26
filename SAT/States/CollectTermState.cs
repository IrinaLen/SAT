using SAT.States;
using System;

namespace SAT.States
{
    class CollectTermState : AbstractParserState
    {
        public CollectTermState(IStateMachine parent) : base(parent)
        {
        }

        public override void ProcessState(IStateContext context)
        {
            IParseString ps = context.GetParseString();
            if (ps.HasChar())
            {
                if (Char.IsDigit(ps.PeekChar()))
                {
                    ps.CollectTerm();
                    ChangeState(new CollectTermState(GetParent()));
                    return;
                }
            }
            ChangeState(new AddTermState(GetParent()));
        }
    }
}