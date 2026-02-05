using DevoidEngine.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class FlexNode : UINode
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
            Rect = finalRect;

            float totalGrow = 0f;
            float fixedMain = 0f;
            int count = 0;

            foreach (var child in _children)
            {
                if (!child.ParticipatesInLayout)
                    continue;

                if (child.Layout.FlexGrowMain > 0)
                    totalGrow += child.Layout.FlexGrowMain;
                else
                    fixedMain += FlexboxTools.Main(child.DesiredSize, Direction);

                count++;
            }

            float availableMain =
                FlexboxTools.Main(finalRect.size, Direction)
                - fixedMain
                - Math.Max(0, Gap * (count - 1));

            availableMain = Math.Max(0, availableMain);

            float gap = Gap;
            float cursor = 0f;

            if (Justify != JustifyContent.Start && count > 0)
            {
                float free = availableMain;

                switch (Justify)
                {
                    case JustifyContent.Center:
                        cursor = free * 0.5f;
                        break;
                    case JustifyContent.End:
                        cursor = free;
                        break;
                    case JustifyContent.SpaceBetween:
                        gap = count > 1 ? free / (count - 1) : 0;
                        break;
                    case JustifyContent.SpaceAround:
                        gap = free / count;
                        cursor = gap * 0.5f;
                        break;
                    case JustifyContent.SpaceEvenly:
                        gap = free / (count + 1);
                        cursor = gap;
                        break;
                }
            }

            foreach (var child in _children)
            {
                if (!child.ParticipatesInLayout)
                    continue;

                float mainSize = child.Layout.FlexGrowMain > 0
                    ? availableMain * (child.Layout.FlexGrowMain / totalGrow)
                    : FlexboxTools.Main(child.DesiredSize, Direction);

                float crossSize = Align == AlignItems.Stretch
                    ? FlexboxTools.Cross(finalRect.size, Direction)
                    : FlexboxTools.Cross(child.DesiredSize, Direction);

                float crossOffset = Align switch
                {
                    AlignItems.Start => 0,
                    AlignItems.Center => (FlexboxTools.Cross(finalRect.size, Direction) - crossSize) * 0.5f,
                    AlignItems.End => FlexboxTools.Cross(finalRect.size, Direction) - crossSize,
                    AlignItems.Stretch => 0,
                    _ => 0
                };

                Vector2 pos = finalRect.position +
                    FlexboxTools.FromMainCross(cursor, crossOffset, Direction);

                Vector2 size =
                    FlexboxTools.FromMainCross(mainSize, crossSize, Direction);

                child.Arrange(new UITransform(pos, size));

                cursor += mainSize + gap;
            }

            //UIRenderer.DrawRect(finalRect, DEBUG_NUM_LOCAL);
        }

        protected override void RenderCore()
        {
            throw new NotImplementedException();
        }
    }
}
