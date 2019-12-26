namespace SAT.States
{
    class StateContext : IStateContext
    {
        IParseString _parseStr;
        IStateContext _prevContext;
        INode _currentConj;
        INode _root;

        public StateContext(IParseString parseStr, IStateContext prev)
        {
            _parseStr = parseStr;
            _prevContext = prev;
        }


        public IParseString GetParseString()
        {
            return _parseStr;
        }

        public IStateContext GetPrevousContext()
        {
            return _prevContext;
        }

        public INode GetRoot()
        {
            return _root;
        }

        public void SetRoot(INode node)
        {
            _root = node;
        }

        public INode GetCurrentConj()
        {
            return _currentConj;
        }

        public void SetCurrentConj(INode node)
        {
            _currentConj = node;
        }
    }
}