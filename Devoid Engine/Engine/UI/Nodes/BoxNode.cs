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
        //UIRenderer.DrawRect(finalRect, DEBUG_NUM_LOCAL);
    }

    protected override void RenderCore(List<RenderItem> renderList)
    {
        renderList.Add(new RenderItem()
        {
            Mesh = UISystem.QuadMesh,
            Material = Material,
            Model = UISystem.BuildModel(Rect),
        });
    }

    protected override void InitializeCore()
    {
        Material = UISystem.GetMaterial();
        Material.PropertiesVec4Override["Configuration"] = new Vector4(DEBUG_NUM_LOCAL, 0, 0, 0);
    }
}
