using DevoidEngine.Engine.Utilities;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace DevoidEngine.Engine.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraData
    {
        public Matrix4x4 View;       // 64 bytes
        public Matrix4x4 Projection; // 64 bytes
        public Vector3 Position;     // 12 bytes
        private float _padding0;     // pad to 16 bytes
        public float NearClip;       // 4 bytes
        public float FarClip;        // 4 bytes
        private Vector2 _padding1;   // pad to 16 bytes
    }


    public class Camera
    {
        public Vector3 Position { get; set; } = new Vector3(0, 0, -5);
        public Vector3 Front { get; set; } = Vector3.UnitZ;
        public Vector3 Up { get; set; } = Vector3.UnitY;
        public Vector3 Right { get; set; } = Vector3.UnitX;

        public float NearClip { get; set; } = 0.1f;
        public float FarClip { get; set; } = 1000f;
        public float FovY { get; set; } = MathF.PI / 3f; // 60 DEG
        public Vector4 ClearColor { get; set; }

        private Matrix4x4 _viewMatrix = Matrix4x4.Identity;
        private Matrix4x4 _projectionMatrix = Matrix4x4.Identity;

        public CameraData GetCameraData()
        {
            return new CameraData
            {
                View = _viewMatrix,
                Projection = _projectionMatrix,
                NearClip = NearClip,
                FarClip = FarClip,
            };
        }

        public void SetProjectionMatrix(Matrix4x4 proj) => _projectionMatrix = proj;
        public Matrix4x4 GetProjectionMatrix() => _projectionMatrix;

        public void SetViewMatrix(Matrix4x4 view) => _viewMatrix = view;
        public Matrix4x4 GetViewMatrix() => _viewMatrix;

        public void SetClearColor(Vector4 color) => ClearColor = color;

        public void UpdateCameraVectorsFromRotation(Vector3 eulerRotation)
        {
            float pitch = MathHelper.DegToRad(eulerRotation.X);
            float yaw = -MathHelper.DegToRad(eulerRotation.Y);
            float roll = MathHelper.DegToRad(eulerRotation.Z);

            float FrontX = MathF.Cos(pitch) * MathF.Cos(yaw);
            float FrontY = MathF.Sin(pitch);
            float FrontZ = MathF.Cos(pitch) * MathF.Sin(yaw);
            Front = Vector3.Normalize(new Vector3(FrontX, FrontY, FrontZ));

            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));

            _viewMatrix = Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }


        public void SetPosition(Vector3 newPos)
        {
            Position = newPos;
            _viewMatrix = Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        public void UpdateViewMatrix()
        {
            _viewMatrix = Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        public void UpdateProjectionMatrix(float aspectRatio)
        {
            _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(FovY, aspectRatio, NearClip, FarClip);
        }
    }
}
