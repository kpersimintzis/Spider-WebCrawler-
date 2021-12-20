using System.Collections.Generic;

namespace Graph.Models
{
    public class InMemoryGraph : IGraph<int>
    {
        private readonly (int from, int to)[] edges;

        public InMemoryGraph((int from, int to)[] edges)
        {
            this.edges = edges;
        }

        public int[] Edges(int node)
        {
            var res = new List<int>();
            foreach (var (from, to) in edges)
            {
                if (node == from)
                {
                    res.Add(to);
                }
            }
            return res.ToArray();
        }
    }
}