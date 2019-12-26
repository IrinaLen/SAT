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
        }
    }
}


