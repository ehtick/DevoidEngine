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

            GameObject go = SceneManager.MainScene.addGameObject("A");
            go.AddComponent<CameraComponent3D>();

            MaterialManager.MaterialA = new PBRMaterial()
            {
                Albedo = new Vector4(1, 0, 0, 1),
                Roughness = 0.5f,
                Metallic = 0f
            };

            RenderThreadDispatcher.QueueLatest("TEXTURE_SET", () =>
            {
                ((PBRMaterial)(MaterialManager.MaterialA)).DiffuseTexture = Helper.loadImageAsTex("Engine/Content/Textures/brick.png", DevoidGPU.TextureFilter.Linear);
            });

            instance = SceneRenderSystem.SubmitMesh(mesh, 0, Matrix4x4.Identity);

            RenderPipeline.OnBeginCameraRender += RenderPipeline_OnBeginCameraRender;
        }

        private void RenderPipeline_OnBeginCameraRender()
        {
            instance.WorldMatrix = Matrix4x4.CreateTranslation(gameObject.transform.Position) * Matrix4x4.CreateScale(gameObject.transform.Scale) * Matrix4x4.CreateRotationX(gameObject.transform.Rotation.X) * Matrix4x4.CreateRotationY(gameObject.transform.Rotation.Y) * Matrix4x4.CreateRotationZ(gameObject.transform.Rotation.Z);
        }
    }
}
