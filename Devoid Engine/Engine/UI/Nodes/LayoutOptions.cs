using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class LayoutOptions
    {
        public float FlexGrowMain;
        public float FlexGrowCross;

        public float FlexBasis;

        public static readonly LayoutOptions Default = new LayoutOptions
        {
            FlexGrowMain = 1,
            FlexGrowCross = 1,
            FlexBasis = 0
        };
    }
}
