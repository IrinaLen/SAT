using System.Linq;

namespace BDD
{
    class BDDToDNF
    {
        public BDDToDNF(BDDNode bdd)
        {
            ToDNF("", bdd);
            if (total == "")
            {
                if (bdd.GetId() == 0) total = "тождественная ложь";
                if (bdd.GetId() == 1) total = "тождественния истина";
            }
        }

        public string ReturnResult()
        {
            return total;
        }

        private string total = "";

        private void ToDNF(string curConj, BDDNode node)
        {
            if(node.GetId() == 0) return;
            if (node.GetId() == 1)
            {
                if (total.Any()) total += " + " + curConj;
                else total += curConj;
                return;
            }
            ToDNF(curConj + "X" + node.GetVarId() + "\"", node.GetLow());
            ToDNF(curConj + "X" + node.GetVarId(), node.GetHigh());
        }
    }
}