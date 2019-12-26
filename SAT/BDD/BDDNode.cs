namespace SAT.BDD
{
    class BDDNode
    {
        private int id;//уникальный для каждого узла
        private int varId;
        private BDDNode low = null;
        private BDDNode high = null;
        public bool IsTerminal { get; private set; }

        public override string ToString()
        {
            if (low == null) return id.ToString();
            return "L( " + low.ToString() + " ) " +varId.ToString() + " R( " + high.ToString() + " )";
        }

        public BDDNode() { }

        public BDDNode(int i)
        {
            id = i;
            varId = i;
            IsTerminal = false;
        }

       
        public BDDNode(int v, int i, bool term = false)
        {
            id = i;
            varId = v;
            IsTerminal = term;
        }

        public BDDNode(int v, int i, BDDNode left, BDDNode right)
        {
            varId = v;
            id = i;
            low = left;
            high = right;
        }


        public BDDNode GetLow()
        {
            return low;
        }

        public void SetLow(BDDNode l)
        {
            low = l;
        }

        public BDDNode GetHigh()
        {
            return high;
        }

        public void SetHigh(BDDNode h)
        {
            high = h;
        }

        public int GetId()
        {
            return id;
        }

        public int GetVarId()
        {
            return varId;
        }

        public void SetVarId(int n)
        {
            varId = n;
        }

        public static bool Eq(BDDNode t1, BDDNode t2)
        {
            if (t1 == null && t2 == null) return true;
            if (t1 == null || t2 == null) return false;
            return t1.varId == t2.varId && t1.high == t2.high && t1.low == t2.low;
        }
      
    }
}
