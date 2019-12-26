using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAT.DPLL;

namespace SAT.Clique
{
    class Clique
    {
        private Dictionary<ColoredEdge, int> nameId;
        private int colorNum, n;
        HashSet<int> vars;

        public int FindeMaxClique(int k)
        {
            this.n = 2 * k ;
            colorNum = k;
            
            do
            {
                n++;
                Console.WriteLine("n = " + n);
            } while (Solve2()) ;
                Console.WriteLine("Result: n = {0}", n - 1);
            return n - 1;
        }

        private bool Solve()
        {
            nameId = new Dictionary<ColoredEdge, int>(); // i, об color
            GenerateVariables();

            var cnf = new DPLL.CNF();
            //not one color in clique
            for (int c = 1; c <= colorNum; c++)
            {
                cnf.Add(GenerateCliqueNotOneColor(c));
            }

            Tree t = OnlyOneColor(1, 2);
            //only one color of edge
            for (int i = 1; i < n; i++)
            {
                for (int j = i + 1; j <= n; j++)
                {
                    if(i != 1 || j != 2)  t.AddAnd(t, OnlyOneColor(i, j));
                }
            }

            var ts = new DPLL.TseitinTransformer(t);
            cnf.Add(ts.Transform());

            vars = ts.GetVars;
            vars = new HashSet<int>(vars.Union(nameId.Values));

            var dpll = new DPLL.DPLL(vars);
            return dpll.DPLLAlgo(cnf);
        }

        private void GenerateVariables()
        {
            int counter = 1;

            for (int i = 1; i < n; i++)
            {
                for (int j = i + 1; j <= n; j++)
                {
                    for (int c = 1; c <= colorNum; c++)
                    {
                        nameId.Add(new ColoredEdge(i, j, c), counter);
                        counter++;
                    }
                }
            }
        }

        private DPLL.CNF GenerateCliqueNotOneColor(int colorId)
        {
            DPLL.CNF res = new DPLL.CNF();

            for (int i = 1; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    for (int k = j + 1; k <= n; k++)
                    {
                        var e1 = new DPLL.Literal(nameId[new ColoredEdge(i, j, colorId)]);
                        var e2 = new DPLL.Literal(nameId[new ColoredEdge(i, k, colorId)]);
                        var e3 = new DPLL.Literal(nameId[new ColoredEdge(j, k, colorId)]);
                        if (colorNum < 3)
                        {
                            res.Add(new DPLL.Literal[] { e1, e2, e3 });
                        }
                        res.Add(new DPLL.Literal[] { e1.Inversed(), e2.Inversed(), e3.Inversed() });
                    }
                }
            }
            return res;
        }
        private Tree OnlyOneColor(int i, int j)
        {
            int c1 = 1; // 1 цвет
            var e1 = nameId[new ColoredEdge(i, j, 1)];
            Tree t1 = new Tree(e1);
            for (int c2 = 2; c2 <= colorNum; c2++)
            {
                e1 = nameId[new ColoredEdge(i, j, c2)];

                t1.AddAnd(t1, c1 == c2 ? new Tree(e1) : new Tree(e1, true));

            }
            Tree result = t1;


            for (c1 = 2; c1 <= colorNum; c1++)
            {
                e1 = nameId[new ColoredEdge(i, j, 1)];
                t1 = new Tree(e1, true); //1!=2
                for (int c2 = 2; c2 <= colorNum; c2++)
                {
                     e1 = nameId[new ColoredEdge(i, j, c2)];

                    t1.AddAnd(t1, c1 == c2 ? new Tree(e1) : new Tree(e1, true));

                }

                result.AddOr(result, t1);
            }
            return result;
        }


        private bool Solve2()
        {
            nameId = new Dictionary<ColoredEdge, int>(); // i, об color
            GenerateVariables();

            var cnf = new DPLL.CNF();
            //not one color in clique
            for (int c = 1; c <= colorNum; c++)
            {
                cnf.Add(GenerateCliqueNotOneColor(c));
            }

            for (int i = 1; i < n; i++)
            {
                for (int j = i + 1; j <= n; j++)
                {
                    Literal[] lits = new Literal[colorNum];
                    for (int c = 1; c <= colorNum; c++)
                    {
                        lits[c - 1] = new Literal(nameId[new ColoredEdge(i, j, c)]);
                    }
                    cnf.Add(lits);
                }
            }

            var dpll = new DPLL.DPLL(nameId.Values.ToArray());
            return dpll.DPLLAlgo(cnf);

        }

    }
}
