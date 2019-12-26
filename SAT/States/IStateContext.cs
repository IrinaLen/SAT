namespace SAT.States
{
    public interface IStateContext
    {
        IParseString GetParseString();
        IStateContext GetPrevousContext();
        INode GetRoot();
        void SetRoot(INode node);
        INode GetCurrentConj();
        void SetCurrentConj(INode node);
    }
}