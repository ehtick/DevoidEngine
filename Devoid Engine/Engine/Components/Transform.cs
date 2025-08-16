using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class Transform : Component
    {
        public override string Type => nameof(Transform);

        private Vector3 position = Vector3.Zero;
        private Vector3 rotation = Vector3.Zero;
        private Vector3 scale = Vector3.One;

        private Vector3 globalPosition = Vector3.Zero;
        private Vector3 globalRotation = Vector3.Zero;
        private Vector3 globalScale = Vector3.One;

        private Matrix4x4 localMatrix = Matrix4x4.Identity;
        private Matrix4x4 worldMatrix = Matrix4x4.Identity;

        private Vector3 prev_position = Vector3.Zero;
        private Vector3 prev_rotation = Vector3.Zero;
        private Vector3 prev_scale = Vector3.One;

        public bool hasMoved = false;

        public Vector3 Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    hasMoved = true;
                }
            }
        }

        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    hasMoved = true;
                }
            }
        }

        public Vector3 Scale
        {
            get => scale;
            set
            {
                if (scale != value)
                {
                    scale = value;
                    hasMoved = true;
                }
            }
        }




        public override void OnStart()
        {

            prev_position = position;
            prev_rotation = rotation;
            prev_scale = scale;
        }

        public override void OnUpdate(float dt)
        {
            if (
                prev_position != position ||
                prev_rotation != rotation ||
                prev_scale != scale
            )
            {
                prev_position = position;
                prev_rotation = rotation;
                prev_scale = scale;
                hasMoved = true;
            }
            else
            {
                hasMoved = false;
            }
           
        }
    }
}
