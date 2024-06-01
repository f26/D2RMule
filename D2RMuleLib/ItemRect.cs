using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    public class ItemRect
    {
        public Rectangle rect = new Rectangle();
        public Item Item { get; set; } = new Item();
        public ItemRect(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
        }
    }
}
