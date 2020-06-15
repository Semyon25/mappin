using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace mappin
{
    public class Node
    {
        public Border border;
        public Point point;
        public int number;
        public bool isLeft;
        public bool isUsed;
        public Node()
        {
            isUsed = false;
        }
        public Node(Border b, Point p)
        {
            border = b;
            int n = Convert.ToInt32(b.Uid);
            point = p;
            number = n > 32 ? n - 32 : n;
            isLeft = n > 32 ? false : true;
            isUsed = true;
        }
    }
}
