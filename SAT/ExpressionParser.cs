using SAT.States;
using System.Collections.Generic;

namespace SAT
{
    class ExpressionParser : IExpressionParser, IStateMachine
    {
        private Stack<IParserState> _states;
        private IStateContext _context;
        private string _error;

        public void ChangeContext(IStateContext context)
        {
            _context = context;
        }

        public void ChangeState(IParserState state)
        {
            _states.Push(state);
        }

        public ExpressionParser()
        {
            _states = new Stack<IParserState>();
            _context = null;
        }

        public string GetError()
        {
            return _error;
        }

        public INode Parse(string a)
        {
            IParserState state;
            _context = new StateContext(new ParseString(a.ToUpper()), null);
            ChangeState(new InitState(this));
            while (_states.Count > 0)
            {
                state = _states.Pop();
                state.ProcessState(_context);
            }
            return _context.GetRoot();
        }

        public void SetError(string msg)
        {
            _error = msg;
        }
    }

}
