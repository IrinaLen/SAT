using System;

namespace SAT.States
{
    class ParseString : IParseString
    {
        private string _str;
        private int _pos;
        private string _term;

        public ParseString(string a)
        {
            _str = a;
            _pos = 0;
        }

        public int GetPosition()
        {
            return _pos;
        }

        public void SetPosition(int pos)
        {
            if (pos < _str.Length) _pos = pos;
        }

        public bool HasChar()
        {
            return _pos < _str.Length;
        }

        public char GetChar()
        {
            _pos++;
            return _str[_pos - 1];
        }

        public char PeekChar()
        {
            return _str[_pos];
        }

        public void ClearTerm()
        {
            _term = "";
        }

        public void CollectTerm()
        {
            _term = _term + GetChar();
        }

        public int GetVarNum()
        {
            if (_term != "") return Convert.ToInt32(_term);
            else return 0;
        }
    }
}