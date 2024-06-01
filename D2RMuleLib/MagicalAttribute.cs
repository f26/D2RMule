using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    public class MagicalAttribute
    {
        public string modName = "";
        public List<Int32> values = new List<Int32>();
        public string tooltip = "";

        public void Add(UInt32 val)
        {
            this.values.Add((Int32)val);
        }
    }
}
