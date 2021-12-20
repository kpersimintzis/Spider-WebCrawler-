using System;

namespace Graph
{
    public interface IGraph<TNode>
    {
        TNode[] Edges(TNode node);
    }
}
