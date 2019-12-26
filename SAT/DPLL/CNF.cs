using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNFstruct = System.Collections.Generic.List<System.Collections.Generic.HashSet<SAT.DPLL.Literal>>;


namespace SAT.DPLL
{
    class CNF 
    {
        CNFstruct cnf;

        public CNF()
        {
            cnf = new CNFstruct();
        }

        public CNF(Literal lit)
        {
            cnf = new CNFstruct();
            Add(lit);
        }
        public CNF(CNFstruct cnf)
        {
            FullCopy(cnf);
        }
        public CNF(CNF f)
        {
            FullCopy(f.cnf);
        }


        private void FullCopy(CNFstruct str)
        {
            cnf = new CNFstruct();
            foreach(var el in str)
            {
                cnf.Add(new HashSet<Literal>(el));
            }
        }
        public void Add(Literal l)
        {
            cnf.Add(new HashSet<Literal> { l });
        }

        public void Add(IEnumerable<Literal> disj)
        {
            cnf.Add(new HashSet<Literal>(disj));
        }

        // возвращает конъюнкты из одной переменной
        public List<Literal> GetUnionDisj()
        {
            List<Literal> lst = new List<Literal>();
            foreach(var el in cnf)
            {
                if(el.Count == 1)
                {
                    lst.Add(el.First());
                }
            }
            return lst;
        }

        public void AddInversed(Literal l)
        {
            cnf.RemoveAll(disj => disj.Count == 1 && disj.First().Equals(l));
            this.Add(l.Inversed());
        }

        public bool IsEmpty()
        {
            return cnf.Count == 0;
        }

        public bool HasEmptyConj()
        {
            return cnf.Any(x => x.Count == 0);
        }

        public int Count => cnf.Count;

        public override string ToString()
        {
            string s = "";
            foreach(var disj in cnf)
            {
                foreach(var el in disj)
                {
                    s += el.ToString() + " "; 
                }
                s += Environment.NewLine;
            }
            s += Environment.NewLine;
            return s;
        }

        public bool Remove(HashSet<Literal> conj)
        {
            return cnf.Remove(conj);
        }

        public HashSet<Literal> GetConj(int i)
        {
            return cnf[i];
        }

        public void Add(CNF addCNf)
        {
            foreach(var c in addCNf.cnf)
            {
                cnf.Add(c);
            }
        }
    }
}
