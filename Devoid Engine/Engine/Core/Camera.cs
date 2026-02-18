using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DevoidEngine.Engine.Core
{

    public class Camera
    {
        public Framebuffer RenderTarget { get; set; }
        public Frustum Frustum { get; set; }

        public Vector3 Position { get; private set; } = new Vector3(0, 0, -5);
        public Vector3 Front { get; private set; } = Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;
        public Vector3 Right { get; private set; } = Vector3.UnitX;

        // Previous frame values for interpolation
        private Vector3 prevPosition = Vector3.Zero;
        private Quaternion prevOrientation = Quaternion.Identity;

        // Current orientation stored as quaternion
        private Quaternion _orientation = Quaternion.Identity;

        public float NearClip { get; set; } = 0.1f;
        public float FarClip { get; set; } = 1000f;
        public float FovY { get; set; } = MathF.PI / 3f; // default 60°
        public Vector4 ClearColor { get; private set; } = Vector4.One;

        private Matrix4x4 _viewMatrix = Matrix4x4.Identity;
        private Matrix4x4 _projectionMatrix = Matrix4x4.Identity;



        // --- Data for GPU ---
        public CameraData GetCameraData()
        {
            Matrix4x4.Invert(_projectionMatrix, out Matrix4x4 invProjection);

            return new CameraData
            {
                View = _viewMatrix,
                Projection = _projectionMatrix,
                InverseProjection = invProjection,
                Position = Position,
                NearClip = NearClip,
                FarClip = FarClip,
                ScreenSize = new Vector2(Renderer.Width, Renderer.Height)
            };
        }

        // --- Interpolated camera data for render thread ---
        public CameraData GetInterpolatedCameraData(float alpha)
        {
            // Clamp alpha
            alpha = Math.Clamp(alpha, 0f, 1f);

            // Interpolate position and orientation
            Vector3 interpPos = Vector3.Lerp(prevPosition, Position, alpha);
            Quaternion interpOrient = Quaternion.Slerp(prevOrientation, _orientation, alpha);

            Vector3 interpFront = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, interpOrient));
            Vector3 interpUp = Vector3.Normalize(Vector3.Transform(Vector3.UnitY, interpOrient));

            Matrix4x4 interpView = Matrix4x4.CreateLookAt(interpPos, interpPos + interpFront, interpUp);

            Matrix4x4.Invert(_projectionMatrix, out Matrix4x4 invProjection);

            return new CameraData
            {
                View = interpView,
                Projection = _projectionMatrix,
                InverseProjection = invProjection,
                Position = interpPos,
                NearClip = NearClip,
                FarClip = FarClip,
                ScreenSize = new Vector2(Renderer.Width, Renderer.Height)
            };
        }

        // --- Mutators ---
        public void SetClearColor(Vector4 color) => ClearColor = color;

        public void SetProjectionMatrix(Matrix4x4 proj) => _projectionMatrix = proj;
        public Matrix4x4 GetProjectionMatrix() => _projectionMatrix;

        // Updated UpdateView signature: accept world rotation (quaternion)
        public void UpdateView(Vector3 position, Quaternion rotation)
        {
            // move current -> previous
            prevPosition = Position;
            prevOrientation = _orientation;

            Position = position;
            _orientation = rotation;

            Front = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, _orientation));
            Up = Vector3.Normalize(Vector3.Transform(Vector3.UnitY, _orientation));
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));

            _viewMatrix = Matrix4x4.CreateLookAt(Position, Position + Front, Up);

            Frustum = Frustum.FromMatrix(_viewMatrix * _projection_MATRIX);
        }

        public void SetViewMatrix(Matrix4x4 view) => _viewMatrix = view;
        public Matrix4x4 GetViewMatrix() => _viewMatrix;

        public void UpdateProjectionMatrix(float aspectRatio)
        {
            _projection_matrix = Matrix4x4.CreatePerspectiveFieldOfView(FovY, aspectRatio, NearClip, FarClip);

            Frustum = Frustum.FromMatrix(_viewMatrix * _projection_matrix);
        }
    }
}
