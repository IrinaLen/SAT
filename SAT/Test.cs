using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SAT.LBDD;

namespace SAT
{
    class Test
    {
        struct coloredEdge
        {
            public readonly int i, j, color;
            public coloredEdge(int i, int j, int c)
            {
                this.i = i;
                this.j = j;
                this.color = c;
            }

            public override string ToString()
            {
                return "p_" + i + "," + j + "^" + color;
            }
        }

        private LBDD.LBDD bdd;
        private Dictionary<coloredEdge, int> nameId;
        public  LBDDNode BDDSAT(int n, int color)
        {
            nameId = new Dictionary<coloredEdge, int>(); // i, об color

            int counter = 1;
            for (int c = 1; c <= color; c++)
            {
                for (int i = 1; i < n; i++)
                {
                    for (int j = i + 1; j <= n; j++)
                    {
                        nameId.Add(new coloredEdge(i, j, c), counter);
                        counter++;
                    }
                }
            }
            var order = nameId.Values.ToArray();
            bdd = new LBDD.LBDD(order);
            LBDDNode conj = bdd.One;
            //only one color of edge
            for (int i = 1; i < n; i++)
            {
                for (int j = i + 1; j <= n; j++)
                {
                    conj = bdd.AND(conj, OnlyOneColor(i, j, color));
                }
            }


            //not one color in clique
            for (int c = 1; c <= color; c++)
            {
                for (int i = 1; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        for (int k = j + 1; k <= n; k++)
                        {
                            var e1 = bdd.CreateVar(nameId[new coloredEdge(i, j, c)]);
                            var e2 = bdd.CreateVar(nameId[new coloredEdge(i, k, c)]);
                            var e3 = bdd.CreateVar(nameId[new coloredEdge(j, k, c)]);
                            var local1 = color < 3 ? bdd.OR(e1, bdd.OR(e2, e3)) : bdd.One;
                            var local2 = bdd.OR(bdd.NOT(e1), bdd.OR(bdd.NOT(e2), bdd.NOT(e3)));

                            //Console.WriteLine(i + ", " + j + ", " + k + " color=" + c);
                            conj = bdd.AND(conj, bdd.AND(local1, local2));
                        }
                    }
                }
            }

          

            return conj;

        }

        private LBDDNode OnlyOneColor(int i, int j, int colorNum)
        {
            LBDDNode result = bdd.Zero;
            for (int c1 = 1; c1 <= colorNum; c1++)
            {
                LBDDNode conj = bdd.One;
                for (int c2 = 1; c2 <= colorNum; c2++)
                {
                    var e1 = bdd.CreateVar(nameId[new coloredEdge(i, j, c2)]);
                    conj = bdd.AND(conj, c1 == c2 ? e1 : bdd.NOT(e1));
                }

                result = bdd.OR(result, conj);
            }
            return result;
        }
        

    }
}
