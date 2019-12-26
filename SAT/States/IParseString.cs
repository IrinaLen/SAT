namespace SAT.States
{
    public interface IParseString
    {
        int GetPosition();
        void SetPosition(int pos);
        bool HasChar();
        char GetChar();
        char PeekChar();
        void ClearTerm();
        void CollectTerm();
        int GetVarNum();
    }
}