using SAT.States;

namespace SAT.States
{
    class AddConjunctionState : AbstractParserState
    {
        public AddConjunctionState(IStateMachine parent) : base(parent)
        {
        }

        public override void ProcessState(IStateContext context)
        {
            IParseString ps = context.GetParseString();

            if (context.GetRoot() != null)
            {
                if (context.GetCurrentConj() != null)
                    context.SetRoot(new Tree.OrNode((Tree.Node) context.GetRoot(), (Tree.Node) context.GetCurrentConj()));
            }
            else
            {
                context.SetRoot(context.GetCurrentConj());
            }
            context.SetCurrentConj(null);
            if (ps.HasChar()) ChangeState(new InitState(GetParent()));
        }
    }
}