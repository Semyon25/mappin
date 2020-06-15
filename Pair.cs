using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace mappin
{
    public class Pair
    {
        public Node leftNode;
        public Node rightNode;
        public Polyline polyline;
        public Pair(Node l, Node r, Polyline p)
        {
            leftNode = l;
            rightNode = r;
            polyline = p;
        }
        public Pair() { }
    }
}
