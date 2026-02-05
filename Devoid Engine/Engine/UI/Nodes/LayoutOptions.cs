using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class LayoutOptions
    {
        public bool ExpandV = false;
        public bool ExpandH = false;

        public static readonly LayoutOptions Default = new LayoutOptions
        {
            ExpandV = true,
            ExpandH = true
        };
    }
}
