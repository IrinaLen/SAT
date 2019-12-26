namespace SAT
{
    struct ColoredEdge
    {
        public readonly int i, j, color;
        public ColoredEdge(int i, int j, int c)
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
}
