using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Components
{
    public class MeshRenderer : Component
    {
        public override string Type => nameof(MeshRenderer);

        Mesh mesh;

        private RenderInstance instance;

        public override void OnStart()
        {
            mesh = new Mesh();

            mesh.SetVertices(Primitives.GetCubeVertex());

            instance = SceneRenderSystem.SubmitMesh(mesh, 0, Matrix4x4.Identity);
        }

        public override void OnUpdate(float dt)
        {
            instance.WorldMatrix = Matrix4x4.CreateTranslation(gameObject.transform.Position);
        }
    }
}
