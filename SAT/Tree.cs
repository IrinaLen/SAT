using System;
using System.Collections;

namespace SAT
{
    public interface INode
    {
        bool HasLeft();
        INode GetLeft();
        void SetLeft(INode node);
        bool HasRight();
        INode GetRight();
        void SetRight(INode node);
        bool HasParent();
        INode GetParent();
        void SetParent(INode node);
        int GetDepth();
    }



    class Tree : IEnumerable
    {
        private Node root = dummy;
        private static readonly DummyNode dummy = new DummyNode();

        public Tree()
        {
            
        }

        public Tree(INode root)
        {
            if (root != null) this.root = (Node) root;
            else new Exception("Null root");
        }
        public Tree(int n, bool negate = false)
        {
            if (root == dummy)
            {
                if (negate) root = new NotNode(new TermNode(n));
                else root = new TermNode(n);
            }
            else
            {
                new Exception("недопустимая операция");
            }
        }

        public void AddAnd(Tree t, int n, bool negate = false)
        {
            if (negate) root = new AndNode(t.root, new NotNode(new TermNode(n)));
            else root = new AndNode(t.root, new TermNode(n));

        }

        public void AddAnd(Tree t1, Tree t2)
        {
            root = new AndNode(t1.root, t2.root);
        }

        public void AddOr(Tree t1, Tree t2)
        {
            root = new OrNode(t1.root, t2.root);
        }

        public void AddNot()
        {
            root = new NotNode(root);
        }

        class DummyNode : Node
        {
            public DummyNode() : base()
            {

            }

            public override INode GetLeft()
            {
                return dummy;
            }

            public override INode GetParent()
            {
                INode par;
                if (parent.TryGetTarget(out par)) return par;
                return dummy;
            }

            public override INode GetRight()
            {
                return dummy;
            }

            public override void SetLeft(INode node)
            {
            }

            public override void SetRight(INode node)
            {
            }

            public override void SetParent(INode node)
            {

            }

        }

        public abstract class Node : INode
        {
            private INode left = dummy;
            private INode right = dummy;
            protected WeakReference<INode> parent = new WeakReference<INode>(dummy);

            protected Node(INode left, INode right)
            {
                SetLeft(left);
                SetRight(right);
            }

            protected Node(INode left)
            {
                SetLeft(left);
            }

            protected Node()
            {
            }

            public bool HasLeft()
            {
                return left != dummy;
            }

            public bool HasRight()
            {
                return right != dummy;
            }

            public bool HasParent()
            {
                return GetParent() != dummy;
            }

            public virtual INode GetLeft()
            {
                return left;
            }

            public virtual void SetLeft(INode node)
            {
                left = node;
                if (node == null)
                {
                    new NullReferenceException("Пустой узел недопустим");
                    return;
                }
                if (node.GetParent() == dummy) node.SetParent(this);
                else
                {
                    new Exception("Узел должен быть без родителя (то есть корневым)");
                }
            }

            public virtual INode GetRight()
            {
                return right;
            }

            public virtual void SetRight(INode node)
            {
                right = node;
                if (node == null)
                {
                    new NullReferenceException("Пустой узел недопустим");
                    return;
                }
                if (node.GetParent() == dummy) node.SetParent(this);
                else
                {
                    new Exception("Узел должен быть без родителя (то есть корневым)");
                }
            }

            public virtual INode GetParent()
            {
                INode par;
                if (parent.TryGetTarget(out par)) return par;
                return dummy;
            }

            public virtual void SetParent(INode node)
            {
                parent = new WeakReference<INode>(node);
            }

            public int GetDepth()
            {
                int d = 0;
                INode n = this;
                while (n.HasParent())
                {
                    d++;
                    n = n.GetParent();
                }
                return d;
            }

            public int NodeDepth()
            {
                INode node = this;
                int depth = 0;
                while (node.HasParent())
                {
                    depth += 1;
                    node = node.GetParent();
                }
                return depth;
            }

        }

        public class TermNode : Node
        {
            public int Num { get; private set; }

            public TermNode(int num)
            {
                Num = num;
            }

            public override string ToString()
            {
                return "X" + Num.ToString();
            }
        }

        public class AndNode : Node
        {
            public AndNode(Node l, Node r) : base(l, r)
            {

            }

            public override string ToString()
            {
                return "AND (" + GetLeft().ToString() + ", " + GetRight().ToString() + ")";
            }
        }

        public class OrNode : Node
        {
            public OrNode(Node l, Node r) : base(l, r)
            {

            }

            public override string ToString()
            {
                return "OR (" + GetLeft().ToString() + ", " + GetRight().ToString() + ")";
            }

        }

        public class NotNode : Node
        {
            public NotNode(Node l) : base(l)
            {

            }

            public override string ToString()
            {
                return "NOT (" + GetLeft().ToString() + ")";
            }
        }

        public IEnumerator GetEnumerator() //.......
        {
            bool fromright = false;
            INode shuttle = root, holder;
            while (shuttle.HasParent())
            {
                while (fromright)
                {
                    yield return shuttle;
                    if (shuttle == root) yield return shuttle;
                    holder = shuttle;
                    shuttle = shuttle.GetParent();
                    fromright = shuttle.GetRight() == holder;
                    if (!fromright && shuttle.HasRight()) shuttle = shuttle.GetRight();
                    else fromright = true;
                }
                while (shuttle.HasLeft() && shuttle != dummy) shuttle = shuttle.GetLeft();
                if (shuttle.HasRight() && shuttle != dummy) shuttle = shuttle.GetRight();
                else fromright = true;
            }
        }

        public INode ReturnRoot()
        {
            return root;
        }

        public override string ToString()
        {
            return root.ToString();
        }
    }

}

