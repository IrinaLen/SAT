using System;
using System.Collections.Generic;

namespace SAT.BDD
{
    class BDD
    {
        private int id;
        private Dictionary<Tuple<int, int, int>, BDDNode> nodeTable; // <var, low, high>
        private BDDNode one;
        private BDDNode zero;
        private int[] vars;
        public BDD(int[] vals)
        {
            vars = vals;
            nodeTable = new Dictionary<Tuple<int, int, int>, BDDNode>();
            id = 2;
            // construct leaf nodes 0 and 1 and save to map
            zero = new BDDNode(-2, 0, true);
            one = new BDDNode(-1, 1, true);
           // InsertNode(zero);
           // InsertNode(one);
        }

        public BDDNode AND(BDDNode f, BDDNode g)
        {
            var ite =  ITE(f, g, zero);
            return ite;
        }

        public BDDNode OR(BDDNode f, BDDNode g)
        {
            var ite = ITE(f, one, g);
            return ite;
        }

        public BDDNode NOT(BDDNode f)
        {
            var ite = ITE(f, zero, one);
            return ite;

        }

        void InsertNode(BDDNode node) // Добавление в таблицу
        {
            int leftId = (node.GetLow() == null) ? 0 : node.GetLow().GetId();
            int rightId = (node.GetHigh() == null) ? 0 : node.GetHigh().GetId();
            int varId = node.GetVarId();
            if (!nodeTable.ContainsKey(Tuple.Create(varId, leftId, rightId)))
                nodeTable.Add(Tuple.Create(varId, leftId, rightId), node);
        }

        BDDNode GetNode(int varId, int leftId, int rightId)
        {
            if (nodeTable.ContainsKey(Tuple.Create(varId, leftId, rightId)))
            {
                return nodeTable[Tuple.Create(varId, leftId, rightId)];
            }
            return null;
        }

        public BDDNode CreateVars(int n)//переделать для robdd
        {
            if (nodeTable.ContainsKey(Tuple.Create(n, zero.GetId(), one.GetId())))
                return nodeTable[Tuple.Create(n, zero.GetId(), one.GetId())];
            BDDNode newNode = ConstructOneVarTree(n, id++);
            InsertNode(newNode);
            return newNode;
        }

        private BDDNode ConstructOneVarTree(int n, int id)
        {
            BDDNode root = new BDDNode(n, id);
            root.SetLow(zero);
            root.SetHigh(one);
            return root;
        }

       
        BDDNode MakeNode(int varId, BDDNode low, BDDNode high)
        {
            if (low == high) return low;
            return GetNode(varId,low.GetId(),high.GetId()); //найти в таблице с нодами;

        }

        private Dictionary<Tuple<int, int, int>, BDDNode> cache = new Dictionary<Tuple<int, int, int>, BDDNode>();
        private BDDNode ITE(BDDNode f, BDDNode g, BDDNode h)
        {
            //Один из следующих терминальных
            //ite(F; 1; 0) = ite(1; F; G) = ite(0; G; F ) = ite(G; F; F ) = F;
            if (g.GetId() == one.GetId() && h.GetId() == zero.GetId()) return f;
            if (f.GetId() == one.GetId()) return g;//one
            if (f.GetId() == zero.GetId()) return h;//zero
            if (g.GetVarId() == h.GetVarId()) return g;

            //Normalize
            //ite(F; F; G) =) ite(F; 1; G)
            //ite(F; G; F ) =) ite(F; G; 0)

            if (BDDNode.Eq(f, g))
            {
                h = g;
                g = one;
            }
            if (BDDNode.Eq(f, h))
            {
                h = zero;
            }


            //где-то здесь ошибка
            if (cache.ContainsKey(Tuple.Create(f.GetId(), h.GetId(), g.GetId())))//  HASH_LOOKUP_COMPUTED_TABLE
                return cache[Tuple.Create(f.GetId(), h.GetId(), g.GetId())];

            int v = TopVar(f, g, h); // top variable from f,g,h
            // recursive calls
            BDDNode fn = ITE(Contraction(v, f, true),//сужение по high
                Contraction(v, g, true),
                Contraction(v, h, true));
            BDDNode gn = ITE(Contraction(v, f, false),//сужение по low
                Contraction(v, g, false),
                Contraction(v, h, false));
           

            if (BDDNode.Eq(fn, gn)) return gn;
            BDDNode node = MakeNode(v, gn, fn);// HASH_LOOKUP_UNIQUE_TABLE
            if (node == null)
            {
                node = new BDDNode(v, id++, gn, fn); 
                InsertNode(node); //  insert into UNIQUE_TABLE
            }
            if (!cache.ContainsKey(Tuple.Create(v, gn.GetId(), fn.GetId())))
                cache.Add(Tuple.Create(v, gn.GetId(), fn.GetId()), node);//INSERT_COMPUTED_TABLE
            return node;
        }

        
        private int TopVar(BDDNode f, BDDNode g, BDDNode h)
        {
            if (g.IsTerminal)
            {
                if (h.IsTerminal) return f.GetVarId();
                return vars[Math.Min(
                    Array.IndexOf(vars,f.GetVarId()), Array.IndexOf(vars, h.GetVarId()))];
                
            }
            if (h.IsTerminal)
            {
                return vars[Math.Min(Array.IndexOf(vars, f.GetVarId()), Array.IndexOf(vars, g.GetVarId()))];
            }
            return vars[Math.Min(
                Math.Min(Array.IndexOf(vars, f.GetVarId()), Array.IndexOf(vars, g.GetVarId())),
                Array.IndexOf(vars, h.GetVarId()))];
        }

       

        private BDDNode Contraction(int v, BDDNode x, bool pos) // сужение функции
        {
            if (v != x.GetVarId()) return x;
            return (pos) ? x.GetHigh() : x.GetLow();
        }

      
    }
}
