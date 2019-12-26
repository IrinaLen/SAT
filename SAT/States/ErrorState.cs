using SAT.States;

namespace SAT.States
{
    class ErrorState : AbstractParserState
    {
        public override void ProcessState(IStateContext context)
        {
            context.SetRoot(null);
            
        }

        public ErrorState(IStateMachine parent) : base(parent)
        {
        }
    }
}