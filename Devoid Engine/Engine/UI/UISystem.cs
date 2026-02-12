using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

            UIMaterial = new Material();
            UIMaterial.Shader = new Shader("Engine/Content/Shaders/UI/basic");

            UIMaterial.MaterialLayout = new MaterialLayout()
            {
                bufferSize = 80,
                Properties =
                {
                    new ShaderPropertyInfo() {Name = "Model", Type = ShaderPropertyType.Matrix4},
                    new ShaderPropertyInfo() {Name = "Configuration", Offset = 64, Type = ShaderPropertyType.Vector4 }
                }
            };

            UIMaterial.PropertiesVec4["Configuration"] = new Vector4(0,0,0,0);

            MaterialManager.RegisterMaterial(UIMaterial);

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
