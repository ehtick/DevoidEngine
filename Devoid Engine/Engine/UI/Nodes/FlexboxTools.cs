using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public static class FlexboxTools
    {
        //float ComputeJustifyOffset(float freeSpace)
        //{
        //    return Justify switch
        //    {
        //        JustifyContent.Start => 0,
        //        JustifyContent.Center => freeSpace * 0.5f,
        //        JustifyContent.End => freeSpace,
        //        JustifyContent.SpaceBetween => 0,
        //        //JustifyContent.SpaceAround => freeSpace / (count * 2),
        //        //JustifyContent.SpaceEvenly => freeSpace / (count + 1),
        //    };
        //}

        public static void ComputeJustify(
            JustifyContent justify,
            float freeSpace,
            int count,
            float gap,
            out float startOffset,
            out float interItemGap)
        {
            startOffset = 0f;
            interItemGap = gap;

            if (count <= 1)
                return;

            switch (justify)
            {
                case JustifyContent.Start:
                    break;

                case JustifyContent.End:
                    startOffset = freeSpace;
                    break;

                case JustifyContent.Center:
                    startOffset = freeSpace / 2f;
                    break;

                case JustifyContent.SpaceBetween:
                    interItemGap += freeSpace / (count - 1);
                    break;

                case JustifyContent.SpaceAround:
                    interItemGap += freeSpace / count;
                    startOffset = interItemGap / 2f;
                    break;

                case JustifyContent.SpaceEvenly:
                    interItemGap += freeSpace / (count + 1);
                    startOffset = interItemGap;
                    break;
            }
        }

        public static float ComputeCrossOffset(
            AlignItems align,
            float containerCross,
            float crossSize
        )
        {
            return align switch
            {
                AlignItems.Start => 0f,
                AlignItems.Center => (containerCross - crossSize) * 0.5f,
                AlignItems.End => containerCross - crossSize,
                AlignItems.Stretch => 0f,
                _ => 0f
            };
        }



        public static float Main(Vector2 v, FlexDirection dir)
            => dir == FlexDirection.Row ? v.X : v.Y;

        public static float Cross(Vector2 v, FlexDirection dir)
            => dir == FlexDirection.Row ? v.Y : v.X;

        public static Vector2 FromMainCross(float main, float cross, FlexDirection dir)
            => dir == FlexDirection.Row
                ? new Vector2(main, cross)
                : new Vector2(cross, main);


    }
}
