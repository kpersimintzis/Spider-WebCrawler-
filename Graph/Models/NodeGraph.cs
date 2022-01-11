using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class NodeGraph : IGraph<Node>
    {        
        public Node[] Edges(Node node)
        {
            return node.Edges;
        }
    }
}
