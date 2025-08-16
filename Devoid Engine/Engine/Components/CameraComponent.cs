using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Utilities;
using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class CameraComponent3D : Component
    {
        public override string Type => nameof(CameraComponent3D);

        public bool isDefault;

        internal Camera camera;

        public Vector4 ClearColor
        {
            get => camera.ClearColor;
            set => camera.SetClearColor(value);
        }

        public bool IsDefault
        {
            get => isDefault;
            set => isDefault = value;
        }

        public float Fov
        {
            get => MathHelper.RadToDeg(camera.FovY);
            set
            {
                float fovRad = MathHelper.DegToRad(value);
                camera.FovY = Math.Clamp(fovRad, 0.01f, MathF.PI - 0.01f);
                UpdateProjection();
            }
        }

        public float NearPlane
        {
            get => camera.NearClip;
            set { camera.NearClip = value; UpdateProjection(); }
        }

        public float FarPlane
        {
            get => camera.FarClip;
            set { camera.FarClip = value; UpdateProjection(); }
        }

        private int width = 550, height = 550;
        private int prevWidth, prevHeight;

        public CameraComponent3D()
        {
            camera = new Camera();
            UpdateProjection();
        }

        public override void OnStart()
        {
            gameObject.Scene.AddCamera(this);
        }

        public override void OnUpdate(float dt)
        {
            // Update camera position and orientation from transform
            camera.Position = gameObject.transform.Position;

            // Calculate camera basis vectors and view matrix
            camera.UpdateCameraVectorsFromRotation(gameObject.transform.Rotation);

            //// Handle screen resize
            //width = Screen.Size.X;
            //height = Screen.Size.Y;
            //if (prevWidth != width || prevHeight != height)
            //{
            //    prevWidth = width;
            //    prevHeight = height;
            //    UpdateProjection();
            //}

            // If default camera, set main
            if (IsDefault)
                gameObject.Scene.SetMainCamera(this);
        }

        public override void OnDestroy()
        {
            gameObject.Scene.RemoveCamera(this);
        }

        private void UpdateProjection()
        {
            float aspectRatio = (float)width / height;
            camera.SetProjectionMatrix(
                Matrix4x4.CreatePerspectiveFieldOfView(camera.FovY, aspectRatio, camera.NearClip, camera.FarClip)
            );
        }
    }
}
