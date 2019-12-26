using SAT.States;

namespace SAT.States
{
    abstract class AbstractParserState : IParserState
    {

        IStateMachine _parent;

        public IStateMachine GetParent()
        {
            return _parent;
        }

        protected void ChangeState(IParserState state)
        {
            GetParent().ChangeState(state);
        }

        protected AbstractParserState(IStateMachine parent)
        {
            _parent = parent;
        }

        public abstract void ProcessState(IStateContext context);//virtual
    }
}