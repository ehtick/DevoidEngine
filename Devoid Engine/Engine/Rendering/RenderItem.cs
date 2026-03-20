using DevoidEngine.Engine.Core;
using System.Numerics;

namespace DevoidEngine.Engine.Rendering
{
    public struct RenderItem
    {
        public Mesh Mesh;
        public Matrix4x4 Model;
        public MaterialInstance Material;
        public RenderState RenderState;

        public TransformSnapshot TransformSnapshot;
        public bool useInterpolation;

        public bool useClipping;
        public Vector4 ClipRegion;
    }
}