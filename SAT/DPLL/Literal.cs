using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAT.DPLL
{ 
    struct Literal//<T>
    {
        public int Name; //T name
        public bool IsNegate;
        public Literal Inversed()
        {
            return new Literal{ Name = Name, IsNegate = !IsNegate};
        }
       
        public Literal(int i, bool neg = false)
        {
            Name = i;
            IsNegate = neg;
        }

        public override string ToString()
        {
            return (IsNegate ? "!":"") + "X" + Name.ToString();
        }
        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Literal) && ((Literal)obj).Name == this.Name && ((Literal)obj).IsNegate == this.IsNegate;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() + IsNegate.GetHashCode();
        }

    }
    
}
