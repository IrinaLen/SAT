
namespace SAT.States
{
    public interface IStateMachine
    {
        void SetError(string msg);
        string GetError();
        void ChangeState(IParserState state);
        void ChangeContext(IStateContext context);
    }

}