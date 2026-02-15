using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class MeshRenderer : Component, IRenderComponent
    {
        public override string Type => nameof(MeshRenderer);

        Mesh mesh;
        MaterialInstance material;

        private RenderInstance instance;

        public override void OnStart()
        {
            material = RenderingDefaults.GetMaterial();
            //RenderPipeline.OnBeginCameraRender += RenderPipeline_OnBeginCameraRender;
        }

        public void AddMesh(Mesh mesh)
        {
            this.mesh = mesh;
        }

        private void RenderPipeline_OnBeginCameraRender()
        {
            //if (instance == null) return;
            //instance.WorldMatrix = Matrix4x4.CreateScale(gameObject.transform.Scale) * Matrix4x4.CreateRotationX(gameObject.transform.Rotation.X) * Matrix4x4.CreateRotationY(gameObject.transform.Rotation.Y) * Matrix4x4.CreateRotationZ(gameObject.transform.Rotation.Z) * Matrix4x4.CreateTranslation(gameObject.transform.Position);
        }

        public void Collect(CameraComponent3D camera, CameraRenderContext viewData)
        {
            if (mesh == null) return;
            viewData.renderItems3D.Add(new RenderItem()
            {
                Material = material,
                Mesh = mesh,
                Model = RenderBase.BuildModel(
                    gameObject.transform.Position,
                    gameObject.transform.Scale
                )
            });
        }
    }
}
