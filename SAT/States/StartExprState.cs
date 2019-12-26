using SAT.States;

namespace SAT.States
{
    class StartExprState : AbstractParserState
    {
        public StartExprState(IStateMachine parent) : base(parent)
        {
        }

        public override void ProcessState(IStateContext context)
        {
            IStateContext newcontext = new StateContext(context.GetParseString(), context);
            GetParent().ChangeContext(newcontext);
            ChangeState(new InitState(GetParent()));
        }
    }
}