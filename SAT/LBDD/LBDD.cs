using System;
using System.Collections.Generic;

namespace SAT.LBDD
{
    class LBDD
    {
        protected Dictionary<Tuple<int, LBDDNode.Labeling, int, int>, LBDDNode> _hashTable; // var, lab, high, low

        public LBDDNode Zero { get; protected set; }
        public LBDDNode One { get; protected set; }
        protected int _id = 2;
        protected int[] _vars;

        public LBDDNode Root { get; set; }
        public LBDD(int[] vals)
        {
            _vars = vals;
            Zero = new LBDDNode(-10, 0, true);
            One = new LBDDNode(-11, 1, true);
            SetCash();
        }

        protected LBDD(LBDDNode root)
        {
            Root = root;
            SetCash();
        }


        //Построение элементов
        public virtual LBDDNode CreateVar(int n)
        {
            return MakeOrReturn(n, LBDDNode.Labeling.Non, One, Zero);
        }

       
        //Операции над lbdd
        protected Dictionary<Tuple<int, int>, LBDDNode> _andCache ;

        public virtual LBDDNode AND(LBDDNode H, LBDDNode K)
        {
            // h & 0 = 0 & k = 0
            if (H.GetId() == Zero.GetId() || K.GetId() == Zero.GetId()) return Zero;
            // 1 & k = k
            if (H.GetId() == One.GetId()) return K;
            // h & 1 = h
            if (K.GetId() == One.GetId()) return H;
            //h==k
            if (K.GetId() == H.GetId()) return K;
            //f&g == g&f, сначала с меньшим номером, потом с большим
            var t = H.GetId() > K.GetId()
                ? Tuple.Create(K.GetId(), H.GetId())
                : Tuple.Create(H.GetId(), K.GetId());

            // была ли такая операция раньше
            if (_andCache.ContainsKey(t)) return _andCache[t];

            int top = TopVar(H, K);
            LBDDNode low = null, high = null;
            LBDDNode.Labeling lab = LBDDNode.Labeling.Non;

            var h1 = Contraction(H, top, true);
            var h0 = Contraction(H, top, false);
            var k1 = Contraction(K, top, true);
            var k0 = Contraction(K, top, false);
 // постро построение bdd без гнс
            
                //Different variables, any label
                if (H.GetVarId() != K.GetVarId())
                {
                    if (H.GetVarId() == top)
                    {
                        low = AND(h0, K);
                        high = AND(h1, K);
                        lab = H.Label;
                    }
                    else //if (k.GetVarId() == top)
                    {
                        low = AND(H, k0);
                        high = AND(H, k1);
                        lab = K.Label;
                    }
                }
                else
                {
                    //Same variable, same label
                    if (H.Label == K.Label)
                    {
                        high = AND(h1, k1);
                        low = AND(h0, k0);
                        lab = H.Label;
                    }
                    //Same variable, diff labels
                    // H = ite(&x, H1 , H0) and K = ite(x, K1 , K0) =>
                    // H and K = ite(&x, (H1 and K1), (H0 and K0)) 
                    else if (H.IsNormal() && K.IsDouble() || H.IsDouble() && K.IsNormal())
                    {
                        lab = LBDDNode.Labeling.Double;
                        high = AND(h1, k1);
                        low = AND(h0, k0);
                    }
                    else
                    {
                        lab = LBDDNode.Labeling.Double;
                        // H = ite (x, H1, H0) and K = ite($x, K1 , K0) ||
                        // H = ite (&x, H1, H0) and K = ite($x, K1 , K0) =>
                        // H and K = ite(&x, (H1 and K0), (H0 and K1)) 
                        if (K.IsNegate())
                        {
                            high = AND(h1, k0);
                            low = AND(h0, k1);
                        }
                        else
                        {
                            high = AND(h0, k1);
                            low = AND(h1, k0);
                        }
                    }
                }
            
            var node = MakeOrReturn(top, lab, high, low);
            _andCache.Add(t, node);
            return node;
        }

        protected Dictionary<Tuple<int, int>, LBDDNode> _orCache;

        public virtual LBDDNode OR(LBDDNode H, LBDDNode K)
        {
            // (h||1) = (1||k) = 1
            if (H.GetId() == One.GetId() || K.GetId() == One.GetId()) return One;
            // (0||k) = k
            if (H.GetId() == Zero.GetId()) return K;
            // (h||0) = h
            if (K.GetId() == Zero.GetId()) return H;


            //f||g == g||f, сначала с меньшим номером, потом с большим
            var t = H.GetId() > K.GetId()
                ? Tuple.Create(K.GetId(), H.GetId())
                : Tuple.Create(H.GetId(), K.GetId());

            if (_orCache.ContainsKey(t)) return _orCache[t];

            int top = TopVar(H, K);
            LBDDNode low = null, high = null;
            LBDDNode.Labeling lab = LBDDNode.Labeling.Non;

            var h1 = Contraction(H, top, true);
            var h0 = Contraction(H, top, false);
            var k1 = Contraction(K, top, true);
            var k0 = Contraction(K, top, false);

            //Different variables, any label
            if (H.GetVarId() != K.GetVarId())
            {
                if (H.GetVarId() == top)
                {
                    low = OR(h0, K);
                    high = OR(h1, K);
                    lab = H.Label;
                }
                else //if (k.GetVarId() == top)
                {
                    low = OR(H, k0);
                    high = OR(H, k1);
                    lab = K.Label;
                }
            }
            else
            {
                //Same variable, same label
                if (H.Label == K.Label)
                {
                    high = OR(h1, k1);
                    low = OR(h0, k0);
                    lab = H.Label;
                }
                //Same variable, diff labels
                // H = ite(&x, H1 , H0) OR K = ite(x, K1 , K0) =>
                // H OR K = ite(&x, (H1 OR K1), (H0 OR K0)) 
                else if (H.IsNormal() && K.IsDouble() || H.IsDouble() && K.IsNormal())
                {
                    lab = LBDDNode.Labeling.Double;
                    high = OR(h1, k1);
                    low = OR(h0, k0);
                }
                else
                {
                    lab = LBDDNode.Labeling.Double;
                    // H = ite (x, H1, H0) OR K = ite($x, K1 , K0) ||
                    // H = ite (&x, H1, H0) OR K = ite($x, K1 , K0) =>
                    // H OR K = ite(&x, (H1 OR K0), (H0 OR K1)) 
                    if (K.IsNegate())
                    {
                        high = OR(h1, k0);
                        low = OR(h0, k1);
                    }
                    else
                    {
                        high = OR(h0, k1);
                        low = OR(h1, k0);
                    }
                }
            }


            var node = MakeOrReturn(top, lab, high, low);
            _orCache.Add(t, node);
            return node;
        }

        protected Dictionary<int, LBDDNode> _notCache;
        public virtual LBDDNode NOT(LBDDNode f)
        {
            if (f.GetId() == Zero.GetId()) return One;
            if (f.GetId() == One.GetId()) return Zero;

            if (_notCache.ContainsKey(f.GetId())) return _notCache[f.GetId()];

            int v = f.GetVarId();
            var invLow = NOT(f.GetLow());
            var invHigh = NOT(f.GetHigh());
            LBDDNode node = null;

            if (f.IsDouble())
            {
                node = MakeOrReturn(v, LBDDNode.Labeling.Double, invHigh, invLow);
            }

            else if (f.IsNegate())
            {
                node = MakeOrReturn(v, LBDDNode.Labeling.Non, invLow, invHigh);
            }

            else /*f.IsNormal()*/
            {
                node = MakeOrReturn(v, LBDDNode.Labeling.Negate, invLow, invHigh);
            }

            _notCache.Add(f.GetId(), node);
            return node;

        }

      
        protected virtual LBDDNode MakeOrReturn(int var, LBDDNode.Labeling lab, LBDDNode high, LBDDNode low)
        {
            var t = Tuple.Create(var, lab, high.GetId(), low.GetId());
            if (high.GetId() == low.GetId()) return low;
            if (_hashTable.ContainsKey(t)) return _hashTable[t];
            LBDDNode node;
            if (lab == LBDDNode.Labeling.Double)
            {
                // ite(&x, 0, 1)=ite($x, 1, 0)
                // ite(&x, 0, K)=ite($x, K, 0)
                if (high.GetId() == Zero.GetId())
                {
                    t = Tuple.Create(var, LBDDNode.Labeling.Negate, low.GetId(), Zero.GetId());
                    node = new LBDDNode(var, _id, Zero, low, LBDDNode.Labeling.Negate);
                }
                // ite(&x, 1, 0)=ite(x, 1, 0)
                // ite(&x, 1, K)=ite(x, 1, K)
                else if (high.GetId() == One.GetId())
                {
                    t = Tuple.Create(var, LBDDNode.Labeling.Non, high.GetId(), low.GetId());
                    node = new LBDDNode(var, _id, low, high, LBDDNode.Labeling.Non);
                }
                // ite(&x, H, 0)=ite(x, H, 0)
                else if (low.GetId() == Zero.GetId())
                {
                    t = Tuple.Create(var, LBDDNode.Labeling.Non, low.GetId(), Zero.GetId());
                    node = new LBDDNode(var, _id, Zero, high, LBDDNode.Labeling.Non);

                }
                // ite(&x, H, 1)=ite($x, 1, H)
                else if (low.GetId() == One.GetId())
                {
                    t = Tuple.Create(var, LBDDNode.Labeling.Negate, One.GetId(), high.GetId());
                    node = new LBDDNode(var, _id, high, One, LBDDNode.Labeling.Negate);
                }
                else
                {
                    node = new LBDDNode(var, _id, low, high, lab);
                }

                if (_hashTable.ContainsKey(t)) return _hashTable[t];

            }
            else
            {
                node = new LBDDNode(var, _id, low, high, lab);
            }

            _id++;
            _hashTable.Add(t, node);
            return node;
        }

        protected int TopVar(LBDDNode f, LBDDNode g)
        {
            if (g.IsTerminal)
            {
                //return vars[Array.IndexOf(vars, f.GetVarId())];
                return f.GetVarId();
            }

            if (f.IsTerminal)
            {
                // return vars[Array.IndexOf(vars, g.GetVarId())];
                return g.GetVarId();
            }

            //порядок переменных записан в массив по убыванию =>
            //чем меньше индекс в массиве, тем выше должа быть переменная в bdd
            return _vars[Math.Min(
                Array.IndexOf(_vars, f.GetVarId()),
                Array.IndexOf(_vars, g.GetVarId())
                )];
        }

        protected LBDDNode Contraction(LBDDNode x, int v, bool pos) // сужение функции на 0 или 1
        {
            if (v != x.GetVarId()) return x;
            return pos ? x.GetHigh() : x.GetLow();
        }
        public void ClearCache()
        {
            SetCash();
        }

        public void ClearOperatorCache()
        {
            _orCache = new Dictionary<Tuple<int, int>, LBDDNode>();
            _andCache = new Dictionary<Tuple<int, int>, LBDDNode>();
            _notCache = new Dictionary<int, LBDDNode>();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        protected void SetCash()
        {
            _orCache = new Dictionary<Tuple<int, int>, LBDDNode>();
            _andCache = new Dictionary<Tuple<int, int>, LBDDNode>();
            _notCache = new Dictionary<int, LBDDNode>();
            _hashTable = new Dictionary<Tuple<int, LBDDNode.Labeling, int, int>, LBDDNode>();

        }
    }
}