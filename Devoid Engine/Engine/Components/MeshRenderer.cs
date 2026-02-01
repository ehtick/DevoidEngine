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
            RenderPipeline.OnBeginCameraRender += RenderPipeline_OnBeginCameraRender;
        }

        public void AddMesh(Mesh mesh)
        {
            this.mesh = mesh;
            instance = SceneRenderSystem.SubmitMesh(mesh, 0, Matrix4x4.Identity);   
        }

        private void RenderPipeline_OnBeginCameraRender()
        {
            if (instance == null) return;
            instance.WorldMatrix = Matrix4x4.CreateScale(gameObject.transform.Scale) * Matrix4x4.CreateRotationX(gameObject.transform.Rotation.X) * Matrix4x4.CreateRotationY(gameObject.transform.Rotation.Y) * Matrix4x4.CreateRotationZ(gameObject.transform.Rotation.Z) * Matrix4x4.CreateTranslation(gameObject.transform.Position);
        }
    }
}
