using Graph;
using Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class InMemoryTestGraph<T> : IGraph<T> where T : IEquatable<T>
    {
        private readonly InMemoryGraph<T> _graph;
        private readonly Random _random;
        public InMemoryTestGraph(InMemoryGraph<T> graph, Random random)
        {
            _graph = graph;
            _random = random;
        }

        public async Task<T[]> Edges(T node)
        {
            var random = _random.Next(1, 11) * 1000;
            await Task.Delay(random);
            return await _graph.Edges(node);
        }
    }
}
