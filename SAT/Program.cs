using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Solvers;
using SAT.DPLL;
using SAT.LBDD;


namespace SAT
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите k: ");
            int k = Convert.ToInt32(Console.ReadLine());
            var q = new Clique.Clique();
            q.FindeMaxClique(k);



            // Dictionary<int, int> freq = new Dictionary<int, int>();
            // DPLL.CNF cnf = new DPLL.CNF();
            // foreach (string line in File.ReadLines(@"d:\10_3"))
            // {
            //     var s = line.Split(' ');
            //     if (s[1].Contains("cnf"))
            //     {
            //         for (int i = 0; i < Int32.Parse(s[2]); i++)
            //         {
            //             freq.Add(i+1, 0);
            //         }
            //     }
            //     else
            //     {
            //         DPLL.Literal[] lits = new DPLL.Literal[s.Length - 1];
            //         for(int i = 0; i < s.Length - 1; i++)
            //         {
            //             int num = Int32.Parse(s[i]);
            //             freq[Math.Abs(num)]++;
            //             lits[i] = num > 0 ? new DPLL.Literal(num) : new DPLL.Literal(-num, true);
            //         }
            //         cnf.Add(lits);
            //     }
            // }
            // var vars = freq.OrderByDescending(x => x.Value).Select(X => X.Key).ToArray();



            var t = new ExpressionParser().Parse("x1x2\"x4+x1x2\"x5+x2\"x4\"x5\"+x1x2x3x4+x1x2x3x4\"+x1x2x3\"x5+x1x2x3\"x5\"");
            var ts = new TseitinTransformer(new Tree(t));
            var cnf = ts.Transform();
            DPLL.DPLL dpll = new DPLL.DPLL(ts.GetVars);

            Console.WriteLine(cnf);

            Console.WriteLine(dpll.DPLLAlgo(cnf));
            foreach(var i in dpll.Model)
            {

            }
        }



    }
}


