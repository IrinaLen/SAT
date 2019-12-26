using SAT.States;

namespace SAT.States
{
    class EndExprState : AbstractParserState
    {
        public EndExprState(IStateMachine parent) : base(parent)
        {
        }

        public override void ProcessState(IStateContext context)
        { 
            IParseString ps = context.GetParseString();
            IStateContext oldcontext = context.GetPrevousContext();
            if (oldcontext == null)
            {
                GetParent().SetError("Unexpected closing parenthesis at pos  " + (ps.GetPosition() - 1).ToString());
                ChangeState(new ErrorState(GetParent()));
                return;
            }
            if (context.GetRoot() == null)
            {
                if (context.GetCurrentConj() != null)
                    context.SetRoot(context.GetCurrentConj()); // êîíåö êîíúþíêöèè
                else
                {
                    GetParent()
                        .SetError("Empty expression between parenthesis at pos " + ((ps.GetPosition() - 1).ToString()));
                    ChangeState(new ErrorState(GetParent()));
                    return;
                }
            }
            else
            {
                if (context.GetCurrentConj() != null)
                    context.SetRoot(
                        new Tree.OrNode((Tree.Node) context.GetRoot(), (Tree.Node) context.GetCurrentConj()));
            }
            context.SetCurrentConj(null);
            if (ps.HasChar() && ps.PeekChar() == '"' )
            {
                ps.GetChar();
                if (context.GetRoot() != null ) context.SetRoot(new Tree.NotNode((Tree.Node)context.GetRoot()));
            }
            if (oldcontext.GetCurrentConj() != null)
                oldcontext.SetCurrentConj(new Tree.AndNode((Tree.Node) oldcontext.GetCurrentConj(),
                    (Tree.Node) context.GetRoot()));
            else oldcontext.SetCurrentConj(context.GetRoot());

            GetParent().ChangeContext(oldcontext);
            if (!ps.HasChar() && oldcontext.GetPrevousContext() != null)
            {
                GetParent().SetError("Not found close bracket at pos " + ((ps.GetPosition() - 1).ToString()));
                ChangeState(new ErrorState(GetParent()));
            }
            else ChangeState(new InitState(GetParent()));
            
        }

    }
}