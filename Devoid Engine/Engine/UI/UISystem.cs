using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.UI
{
    public static class UISystem
    {
        public static List<UINode> Roots;

        public static Material UIMaterial;
        public static Material TextMaterial;

        public static Mesh QuadMesh;

        static UISystem()
        {
            Roots = new List<UINode>();
        }

        public static void Initialize()
        {
            QuadMesh = new Mesh();
            QuadMesh.SetVertices(Primitives.GetQuadVertex());

            UIMaterial = new Material(new Shader("Engine/Content/Shaders/UI/basic"));

            for (int i = 0; i < Roots.Count; i++)
            {
                Roots[i].Initialize();
            }
        }

        public static MaterialInstance GetMaterial()
        {
            return new MaterialInstance(UIMaterial);
        }

        public static Matrix4x4 BuildModel(UITransform t)
        {
            return
                Matrix4x4.CreateScale(t.size.X, t.size.Y, 1f) *
                Matrix4x4.CreateTranslation(t.position.X, t.position.Y, 0f);
        }

        public static void Update()
        {

            for (int i = 0; i < Roots.Count; i++)
            {
                Roots[i].Measure(Screen.Size);
                Roots[i].Arrange(new UITransform(Vector2.Zero, Screen.Size));
            }

        }
    }
}
