using SAT.States;

namespace SAT.States
{
    class AddTermState : AbstractParserState
    {
        public AddTermState(IStateMachine parent) : base(parent)
        {
        }

        public override void ProcessState(IStateContext context)
        {
            IParseString ps = context.GetParseString();
            Tree v = new Tree(ps.GetVarNum());

            if (ps.HasChar() && ps.PeekChar() == '"')
            {
                ps.GetChar();
                v.AddNot();
            }
            if (context.GetCurrentConj() == null) context.SetCurrentConj(v.ReturnRoot());
            else context.SetCurrentConj(new Tree.AndNode((Tree.Node) context.GetCurrentConj(),
                (Tree.Node) v.ReturnRoot()));
            ps.ClearTerm();
            ChangeState(new FindOperationState(GetParent()));

        }
    }
}