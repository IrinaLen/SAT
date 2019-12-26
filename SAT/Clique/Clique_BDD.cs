using SAT.LBDD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAT
{
    class Clique_BDD
    {
        private LBDD.LBDD bdd;
        private Dictionary<ColoredEdge, int> nameId;
        private int colorNum, n;

        public int FindeMaxClique(int k)
        {
            this.n = k;
            colorNum = k;
            LBDDNode res = null;
            do
            {
                if (res != null)
                {
                    res = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                n++;
                Console.WriteLine("n = " + n);
                res = BDDSAT();
            } while (res.GetId() != 0);

            Console.WriteLine("Result: n = {0}", n - 1);
            return n - 1;
        }

        LBDDNode BDDSAT()
        {
            nameId = new Dictionary<ColoredEdge, int>(); // i, об color

            GenerateVariables(n, colorNum);

            var order = nameId.Values.ToArray();
            bdd = new LBDD.LBDD(order);
            LBDDNode conj = bdd.One;

            //not one color in clique
            for (int c = 1; c <= colorNum; c++)
            {
                conj = bdd.AND(conj, GenerateCliqueNotOneColor(c));
            }

            //only one color of edge
            for (int i = 1; i < n; i++)
            {
                for (int j = i + 1; j <= n; j++)
                {
                    conj = bdd.AND(conj, OnlyOneColor(i, j));
                }
            }

            return conj;

        }

        private LBDDNode OnlyOneColor(int i, int j)
        {
            LBDDNode result = bdd.Zero;
            for (int c1 = 1; c1 <= colorNum; c1++)
            {

                //LBDDNode conj = bdd.One;
                //for (int c2 = 1; c2 <= colorNum; c2++)
                //{
                var e1 = bdd.CreateVar(nameId[new ColoredEdge(i, j, c1)]);
                //    conj = bdd.AND(conj, c1 == c2 ? e1 : bdd.NOT(e1));
                //}

                result = bdd.OR(result, e1);
            }
            bdd.ClearOperatorCache();
            return result;
        }


        private void GenerateVariables(int n, int colorNum)
        {
            int counter = 1;
            for (int c = 1; c <= colorNum; c++)
            {
                for (int i = 1; i < n; i++)
                {
                    for (int j = i + 1; j <= n; j++)
                    {
                        nameId.Add(new ColoredEdge(i, j, c), counter);
                        counter++;
                    }
                }
            }
        }

        private LBDDNode GenerateCliqueNotOneColor(int colorId)
        {
            LBDDNode conj = bdd.One;

            for (int i = 1; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    for (int k = j + 1; k <= n; k++)
                    {
                        var e1 = bdd.CreateVar(nameId[new ColoredEdge(i, j, colorId)]);
                        var e2 = bdd.CreateVar(nameId[new ColoredEdge(i, k, colorId)]);
                        var e3 = bdd.CreateVar(nameId[new ColoredEdge(j, k, colorId)]);
                        var local1 = colorNum < 3 ? bdd.OR(e1, bdd.OR(e2, e3)) : bdd.One;
                        var local2 = bdd.OR(bdd.NOT(e1), bdd.OR(bdd.NOT(e2), bdd.NOT(e3)));

                        //Console.WriteLine(i + ", " + j + ", " + k + " color=" + c);
                        conj = bdd.AND(conj, bdd.AND(local1, local2));
                    }
                }
            }
            return conj;
        }
    }
}
