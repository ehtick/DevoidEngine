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
