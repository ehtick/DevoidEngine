using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI;
using DevoidEngine.Engine.UI.Nodes;
using System.Numerics;

class BoxNode : UINode
{
    protected override Vector2 MeasureCore(Vector2 availableSize)
    {
        return Size ?? Vector2.Zero;
    }

    protected override void ArrangeCore(UITransform finalRect)
    {
        Rect = finalRect;
        UIRenderer.DrawRect(finalRect, DEBUG_NUM_LOCAL);
    }

    protected override void RenderCore() { }
}
