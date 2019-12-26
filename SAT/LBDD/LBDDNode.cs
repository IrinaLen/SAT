namespace SAT.LBDD
{
    class LBDDNode
    {
        public Labeling Label { get; private set; }
        public enum Labeling
        {
            Non,
            Negate,
            Double
        }

        private int id;//уникальный для каждого узла
        private int varId;
        private LBDDNode low = null;
        private LBDDNode high = null;
        public bool IsTerminal { get; private set; }
        

        //public override string ToString()
        //{
        //    if (low == null) return id.ToString();
        //    string lb = Label == LBDDNode.Labeling.Double ? "& " : Label == LBDDNode.Labeling.Negate ? "$ " : " ";

        //    return "ite (" +  lb + varId.ToString() + ", "
        //           + high.ToString() + ", " + low.ToString() + " )";
        //}

        public LBDDNode(int v, int id, bool term = false)
        {
            this.id = id;
            varId = v;
            IsTerminal = term;
        }

        public LBDDNode(int v, int i, LBDDNode low, LBDDNode high, Labeling lab = Labeling.Non)
        {
            varId = v;
            id = i;
            this.low = low;
            this.high = high;
            Label = lab;
        }

        public bool IsNegate()
        {
            return Label == Labeling.Negate;
        }

        public bool IsDouble()
        {
            return Label == Labeling.Double;
        }

        public bool IsNormal()
        {
            return Label == Labeling.Non;
        }

        public LBDDNode GetLow()
        {
            return low;
        }

        public void SetLow(LBDDNode l)
        {
            low = l;
        }

        public LBDDNode GetHigh()
        {
            return high;
        }

        public void SetHigh(LBDDNode h)
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


        public int InnerEdgesCount = 0;
    }
}
