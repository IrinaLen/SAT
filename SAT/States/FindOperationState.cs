using SAT.States;

namespace SAT.States
{
    class FindOperationState : AbstractParserState
    {
        public FindOperationState(IStateMachine parent) : base(parent)
        {
        }

        public override void ProcessState(IStateContext context)
        {
            IParseString ps = context.GetParseString();

            if (ps.HasChar())
            {
                switch (ps.GetChar())
                {
                    case 'X':
                        ChangeState(new StartTermState(GetParent()));
                        break;
                    case '+':
                        if (!ps.HasChar() || ps.PeekChar() != 'X' && ps.PeekChar() != '(')
                        {
                            GetParent().SetError("second term isn't found at position " + ps.GetPosition().ToString());
                            ChangeState(new ErrorState(GetParent()));
                        }
                        else ChangeState(new AddConjunctionState(GetParent()));
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
            else ChangeState(new AddConjunctionState(GetParent()));
        }
    }
}