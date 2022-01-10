using System;
using System.Collections.Generic;

namespace Graph.Models
{
    public class InMemoryGraph<T> : IGraph<T> where T: IEquatable<T>
    {
        private readonly (T from, T to)[] edges;

        public InMemoryGraph((T from, T to)[] edges)
        {
            this.edges = edges;
        }

        public T[] Edges(T node)
        {
            var res = new List<T>();
            foreach (var (from, to) in edges)
            {
                if (node.Equals(from))
                {
                    res.Add(to);
                }
            }
            return res.ToArray();
        }
    }
}