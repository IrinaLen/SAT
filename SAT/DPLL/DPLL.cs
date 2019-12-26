using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAT.DPLL
{
    class DPLL
    {
        private List<int> Lit;
        private Dictionary<int, int> model;
        public Dictionary<int, int> Model => model;
        public DPLL(IEnumerable<int> litOrd)
        {
            Lit = new List<int>(litOrd);
        }

        public bool DPLLAlgo(CNF f)
        {
            var m = new Dictionary<int, int>(Lit.Count);
            foreach (var l in Lit)
            {
                m.Add(l, -1);
            }

            //Console.WriteLine(f);

            return DPLLAlgo(f, Lit, m);
        }
        private bool DPLLAlgo(CNF f, List<int> unchLit, Dictionary<int, int> totalModel)
        {
            //Console.WriteLine(f);
            CNF newf = f;
            do
            {
                if (newf.IsEmpty())
                {
                    model = new Dictionary<int, int>(totalModel);
                    return true;
                }

                if (newf.HasEmptyConj())
                    return false;


                var unionLit = newf.GetUnionDisj();
                HashSet<Literal> newLit = new HashSet<Literal>();
                foreach (var u in unionLit)
                {
                    UnitPropagation(u, totalModel, newLit);
                }
                unchLit.RemoveAll(x => newLit.Select(y => y.Name).Contains(x));

                newf = EliminatePureLiterals(newf, newLit);
            } while (newf.GetUnionDisj().Count > 0);

            if (newf.IsEmpty())
            {
                model = new Dictionary<int, int>(totalModel);
                return true;
            }

            if (newf.HasEmptyConj())
                return false;

            var l = ChooseLiteral(unchLit);
            if (l.Name == 0) // нет невыьранных литералов
            {
                Console.WriteLine("no Lit: " + newf.Count);
                return DPLLAlgo(newf, unchLit, totalModel);
            }
            else
            {
                newf.Add(l);
                if (DPLLAlgo(newf, new List<int>(unchLit), new Dictionary<int, int>(totalModel))) return true;
                else
                {
                    //Console.WriteLine("Back Track: " + l + " " + newf.Count + " UncLit:" + unchLit.Count);
                    
                    newf.AddInversed(l);
                    return DPLLAlgo(newf, unchLit, totalModel);
                }
            }
        }

        private Literal ChooseLiteral(List<int> unchLit)
        {
            try
            {
                return new Literal(unchLit.First(), true);
            }
            catch (Exception ex)
            {
                return new Literal(0);
            }
        }

        private void UnitPropagation(Literal l, Dictionary<int, int> model, HashSet<Literal> newLit)
        {
            if (model[l.Name] == -1)
            {
                model[l.Name] = l.IsNegate ? 0 : 1;
                newLit.Add(l);
            }
        }

        private CNF EliminatePureLiterals(CNF F, HashSet<Literal> model)
        {
            CNF newCNF = new CNF(F);
            foreach (var l in model)
            {
                var notL = l.Inversed();
                for (int i = 0; i < newCNF.Count; i++)
                {
                    HashSet<Literal> conj = newCNF.GetConj(i);

                    if (conj.Contains(l))
                    {
                        newCNF.Remove(conj); // выкидываем дизъюнкцию
                        i--;
                    }
                    else if (conj.Contains(notL))
                    {
                        conj.Remove(notL);
                    }

                }
            }
            return newCNF;
        }

    }
}
