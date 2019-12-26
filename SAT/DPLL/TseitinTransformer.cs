using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAT.DPLL
{
    class TseitinTransformer
    {
        Tree logTree;
        CNF cnf;
        int id = -1; // for new vars
        HashSet<int> vars;
        public HashSet<int> GetVars => vars;
        public TseitinTransformer(Tree tree)
        {
            logTree = tree;
            cnf = new CNF();
            vars = new HashSet<int>();
        }

        public CNF Transform()
        {
            var res = Transform(logTree.ReturnRoot());
            cnf = res.Item2;
            cnf.Add(res.Item1);
            return cnf;
        }


        private Tuple<Literal,CNF> Transform(INode node)
        {
            if(node.GetType() == typeof(Tree.AndNode))
            {
                var myLit = new Literal(id);
                vars.Add(id);
                id--;

                var res1 = Transform(node.GetRight());
                var res2 = Transform(node.GetLeft());

                var cnfUnion = new CNF();
                cnfUnion.Add(res1.Item2);
                cnfUnion.Add(res2.Item2);
                // ∆0 = ∆2 ∪ {¬p ∨ `1, ¬p ∨ `2, ¬`1 ∨ ¬`2 ∨ p}

                cnfUnion.Add(new Literal[] { myLit.Inversed(), res1.Item1 });
                cnfUnion.Add(new Literal[] { myLit.Inversed(), res2.Item1 });
                cnfUnion.Add(new Literal[] { myLit, res1.Item1.Inversed(), res2.Item1.Inversed()});
                return Tuple.Create(myLit, cnfUnion);
            }
            else if (node.GetType() == typeof(Tree.OrNode))
            {
                var myLit = new Literal(id);
                vars.Add(id);
                id--;

                var res1 = Transform(node.GetRight());
                var res2 = Transform(node.GetLeft());

                var cnfUnion = new CNF();
                cnfUnion.Add(res1.Item2);
                cnfUnion.Add(res2.Item2);
                // ∆0 = ∆2 ∪ {¬p ∨ `1 ∨ `2, ¬`1 ∨ p, ¬`2 ∨ p}
                cnfUnion.Add(new Literal[] { myLit.Inversed(), res1.Item1, res2.Item1 });
                cnfUnion.Add(new Literal[] { myLit, res2.Item1.Inversed() });
                cnfUnion.Add(new Literal[] { myLit, res1.Item1.Inversed() });

                return Tuple.Create(myLit, cnfUnion);


            }
            else if (node.GetType() == typeof(Tree.NotNode))
            {                
                if(node.GetLeft().GetType() == typeof(Tree.TermNode))
                {
                    int num = ((Tree.TermNode)node.GetLeft()).Num;
                    vars.Add(num);
                    return Tuple.Create(new Literal(num, true), new CNF()); //not x_i
                } 
                else
                {
                    var cnfLoc = Transform(node.GetLeft());
                    return Tuple.Create(cnfLoc.Item1.Inversed(), cnfLoc.Item2);
                }
            }
            else if (node.GetType() == typeof(Tree.TermNode))
            {
                int num = ((Tree.TermNode)node).Num;
                vars.Add(num);
                return Tuple.Create(new Literal(num), new CNF()); // x_i
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
