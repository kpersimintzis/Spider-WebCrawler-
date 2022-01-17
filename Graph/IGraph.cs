using System;
using System.Threading.Tasks;

namespace Graph
{
    public interface IGraph<TNode>
    {
        Task<TNode[]> Edges(TNode node);
    }
}
