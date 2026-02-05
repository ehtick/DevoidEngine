using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    internal class FlexNode : UINode
    {

        public FlexDirection Direction = FlexDirection.Row;
        public JustifyContent Justify = JustifyContent.Start;
        public AlignItems Align = AlignItems.Stretch;
        public float Gap = 0f;

        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            float main = 0;
            float cross = 0;
            int count = 0;


            foreach (var child in _children)
            {
                if (!child.ParticipatesInLayout)
                    continue;

                var childSize = child.Measure(availableSize);

                float childMain = FlexboxTools.Main(childSize, Direction);
                float childCross = FlexboxTools.Cross(childSize, Direction);

                main += childMain;
                cross = MathF.Max(cross, childCross);
                count++;
            }

            if (count > 1)
                main += Gap * (count - 1);

            return FlexboxTools.FromMainCross(main, cross, Direction);
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            throw new NotImplementedException();
        }

        protected override void RenderCore()
        {
            throw new NotImplementedException();
        }
    }
}
