using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace SAT.LBDD
{
    class LBDDToGraph
    {
        class Edge : IEdge<int>
        {
            public Edge(int s, int t, bool type)
            {
                Source = s;
                Target = t;
                Type = type;
            }

            public int Source { get; set; }
            public int Target { get; set; }
            public bool Type { get; set; } // low - 0, high - 1
        }

        class Graph : AdjacencyGraph<int, LBDDToGraph.Edge> { }
        public LBDDToGraph(LBDDNode root)
        {
            ConvertToDot(root);
            PicturePath = GenDiagramFile();
            idNum.Clear();
            //отдельная обработка только терма
        }

        public string PicturePath { get; private set; }

        LBDDToGraph.Graph g = new LBDDToGraph.Graph();
        static Dictionary<int, Tuple<int, LBDDNode.Labeling>> idNum = new Dictionary<int, Tuple<int, LBDDNode.Labeling>>();

        private int CreateVertex(LBDDNode node)
        {
            if (node.IsTerminal)
            {
                if (!idNum.ContainsKey(node.GetId()))
                {
                    idNum.Add(node.GetId(), Tuple.Create(node.GetId(), node.Label));
                    g.AddVertex(node.GetId());
                }
                return node.GetId();
            }
            else
            {
                if (!idNum.ContainsKey(node.GetId()))
                {
                    idNum.Add(node.GetId(), Tuple.Create(node.GetVarId(), node.Label));
                    g.AddVertex(node.GetId());
                }
                return node.GetId();
            }
        }

        private void ConvertToDot(LBDDNode node)
        {
            if (node.IsTerminal) return;
            var v = CreateVertex(node);
            var l = CreateVertex(node.GetLow());
            var h = CreateVertex(node.GetHigh());
            if (!g.ContainsEdge(v, l)) g.AddEdge(new LBDDToGraph.Edge(v, l, false));
            if (!g.ContainsEdge(v, h)) g.AddEdge(new LBDDToGraph.Edge(v, h, true));
            ConvertToDot(node.GetLow());
            ConvertToDot(node.GetHigh());
        }

        private string GenDiagramFile()
        {
            var graphviz = new GraphvizAlgorithm<int, LBDDToGraph.Edge>(g);
            string path = @"../../dotGraph.dot";
            graphviz.FormatVertex += FormatVertex;
            graphviz.FormatEdge += FormatEdge;
            graphviz.Generate(new FileDotEngine(), path);
            var diagramFile = path.Replace(".dot", ".png");
            Process process = new Process();
            process.StartInfo =
                new ProcessStartInfo(@"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe",
                    string.Format(@"-Tpng {0} -Gsize=9,15\! -Gdpi=100 -o {1}",
                        path, diagramFile));
            process.Start();
            process.WaitForExit();
            return diagramFile;
        }


        private static void FormatVertex(object sender, FormatVertexEventArgs<int> e)
        {
            if (e.Vertex == 0 || e.Vertex == 1)
            {
                e.VertexFormatter.StrokeColor = Color.Brown;
                e.VertexFormatter.Shape = GraphvizVertexShape.Box;
            }
            var item = idNum[e.Vertex].Item2;
            string lb = item == LBDDNode.Labeling.Double ? "& " : item == LBDDNode.Labeling.Negate ? "$ " : " ";
            e.VertexFormatter.Label = lb + idNum[e.Vertex].Item1.ToString();

        }
        private static void FormatEdge(object sender, FormatEdgeEventArgs<int, LBDDToGraph.Edge> e)
        {
            if (e.Edge.Type)
            {
                e.EdgeFormatter.StrokeColor = Color.Blue;
            }
            else
            {
                e.EdgeFormatter.StrokeColor = Color.DarkRed;
                e.EdgeFormatter.Style = GraphvizEdgeStyle.Dotted;
            }
        }

    }
}