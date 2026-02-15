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

        Vector3 color = new Vector3(
            (DEBUG_NUM_LOCAL * 16807u % 255) / 255.0f,
            (DEBUG_NUM_LOCAL * 48271u % 255) / 255.0f,
            (DEBUG_NUM_LOCAL * 69621u % 255) / 255.0f
        );

        Material.PropertiesVec4Override["Color"] = new Vector4(color, 1);
    }
}
