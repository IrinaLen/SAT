
namespace SAT.States
{
    class InitState : AbstractParserState
    {
        public InitState(IStateMachine parent) : base(parent)
        {
        }
        public override void ProcessState(IStateContext context)
        {
            IParseString ps = context.GetParseString();
            if (!ps.HasChar())
            {
                if (context.GetCurrentConj() != null)
                {
                    ChangeState(new AddConjunctionState(GetParent()));
                    return;
                }
                else if (context.GetRoot() == null)
                {
                    GetParent().SetError("Unexpected } of line at position" + ps.GetPosition().ToString());
                    ChangeState(new ErrorState(GetParent()));
                }
                return;
            }
            switch (ps.GetChar())
            {
                case 'X':
                    ChangeState(new StartTermState(GetParent()));
                    break;
                case '+':
                    if (context.GetCurrentConj() != null)
                    {
                        ChangeState(new AddConjunctionState(GetParent()));
                        return;
                    }
                    else
                    {
                        GetParent().SetError("Unexpected char position " + ps.GetPosition().ToString());
                        ChangeState(new ErrorState(GetParent()));
                    }
                    break;
                case '(':
                    ChangeState(new StartExprState(GetParent()));
                    break;
                case ')':
                    ChangeState(new EndExprState(GetParent()));
                    break;
                default:
                    GetParent().SetError("Unexpected char at position " + (ps.GetPosition() - 1).ToString());
                    ChangeState(new ErrorState(GetParent()));
                    break;
            }
        }

    }
}