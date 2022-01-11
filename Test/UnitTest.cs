using System;
using Xunit;
using FsCheck.Xunit;
using Spider;
using Graph;
using Graph.Models;
using System.Collections.Generic;
using Walk;
using System.Diagnostics;
using System.Linq;

namespace Test
{
    public class UnitTest
    {
        //[Property(MaxTest = 5000)]
        //public bool TestCommutativity(int a, int b)
        //{
        //    Debug.WriteLine($"{a} {b}");
        //    return (a + b == b + a);
        //}

        //[Property(MaxTest = 10000)]
        //public bool TestDfsNew(Node node)
        //{
        //    if (node == null) return true;

        //    IGraph<Node> graph = new NodeGraph();
        //    List<Node> orderOfRecursiveNodes = new List<Node>();
        //    Crawler.WalkDfsGeneric<Node>(graph, node, new HashSet<Node>(), x => orderOfRecursiveNodes.Add(x));

        //    List<Node> orderOfNonRecursiveNodes =new List<Node>();
        //    Crawler.WalkDfsWithoutRecursionGeneric(graph, node, x => orderOfNonRecursiveNodes.Add(x));
        //    return orderOfNonRecursiveNodes.SequenceEqual(orderOfRecursiveNodes);
        //}

        [Property(MaxTest = 10000)]
        public bool TestDfs(Tuple<int, int>[] edges)
        {
            if (edges.Length == 0) return true;
            var newEdges = edges.Select(x => (from: x.Item1, to: x.Item2)).ToArray();
            IGraph<int> graph = new InMemoryGraph<int>(newEdges);
            List<int> orderOfRecursiveNodes = new List<int>();
            Crawler.WalkDfsGeneric<int>(graph, newEdges[0].from, new HashSet<int>(), x => orderOfRecursiveNodes.Add(x));

            List<int> orderOfNonRecursiveNodes = new List<int>();
            Crawler.WalkDfsWithoutRecursionGeneric(graph, newEdges[0].from, x => orderOfNonRecursiveNodes.Add(x));
            return orderOfNonRecursiveNodes.SequenceEqual(orderOfRecursiveNodes);
        }
    }
}
