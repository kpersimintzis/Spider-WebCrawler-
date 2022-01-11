using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    public class Node : IEquatable<Node>
    {
        public Node[] Edges { get; set; }

        public bool Equals(Node other)
        {
            return ReferenceEquals(other,this);
        }
    }
}
