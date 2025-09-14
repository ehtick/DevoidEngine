using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Utilities;
using DevoidEngine.Engine.Rendering;
using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class CameraComponent3D : Component
    {
        public override string Type => nameof(CameraComponent3D);

        public bool IsDefault { get => isDefault; set
            {
                isDefault = value;
                if (value == true)
                {
                    gameObject.Scene.SetMainCamera(this);
                }
            }
        }

        public Camera Camera { get; private set; }

        private bool isDefault;
        private int width = 800;
        private int height = 600;

        public CameraComponent3D()
        {
            Camera = new Camera();
            Camera.RenderTarget = new Framebuffer();
            Camera.RenderTarget.AttachRenderTexture(new Texture2D((int)Screen.Size.X, (int)Screen.Size.Y, DevoidGPU.TextureFormat.RGBA16_Float));
            Camera.RenderTarget.AttachDepthTexture(new Texture2D((int)Screen.Size.X, (int)Screen.Size.Y, DevoidGPU.TextureFormat.Depth24_Stencil8, false, false, true));

            UpdateProjection();
        }

        public override void OnStart()
        {

            //gameObject.Scene.AddCamera(this);
            //if (IsDefault) gameObject.Scene.SetMainCamera(this);
        }

        public override void OnUpdate(float dt)
        {
            //Pull transform data into camera
            var pos = gameObject.transform.Position;

            // Convert Euler rotation → forward vector
            float pitch = MathHelper.DegToRad(gameObject.transform.Rotation.X);
            float yaw = -MathHelper.DegToRad(gameObject.transform.Rotation.Y);

            Vector3 front = new Vector3(
                MathF.Cos(pitch) * MathF.Cos(yaw),
                MathF.Sin(pitch),
                MathF.Cos(pitch) * MathF.Sin(yaw)
            );

            Vector3 up = Vector3.UnitY;

            Camera.UpdateView(pos, front, up);
        }

        public override void OnDestroy()
        {
            gameObject.Scene.RemoveCamera(this);
        }

        // --- API for projection ---
        public float Fov
        {
            get => MathHelper.RadToDeg(Camera.FovY);
            set
            {
                Camera.FovY = MathHelper.DegToRad(Math.Clamp(value, 1f, 179f));
                UpdateProjection();
            }
        }

        public float NearPlane
        {
            get => Camera.NearClip;
            set { Camera.NearClip = value; UpdateProjection(); }
        }

        public float FarPlane
        {
            get => Camera.FarClip;
            set { Camera.FarClip = value; UpdateProjection(); }
        }

        public void SetViewportSize(int newWidth, int newHeight)
        {
            if (newWidth == width && newHeight == height) return;

            width = newWidth;
            height = newHeight;
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            float aspectRatio = (float)width / height;
            Camera.UpdateProjectionMatrix(aspectRatio);
        }
    }
}
