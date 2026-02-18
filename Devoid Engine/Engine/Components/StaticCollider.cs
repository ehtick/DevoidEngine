using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class StaticCollider : Component
    {
        public override string Type => nameof(StaticCollider);

        // ===============================
        // Shape Definition
        // ===============================

        public PhysicsShapeDescription Shape = new PhysicsShapeDescription
        {
            Type = PhysicsShapeType.Box,
            Size = new Vector3(1, 1, 1)
        };

        public PhysicsMaterial Material = PhysicsMaterial.Default;


        // ===============================
        // Lifecycle
        // ===============================

        public override void OnStart()
        {
            var desc = new PhysicsStaticDescription
            {
                Position = gameObject.transform.Position,
                Rotation = gameObject.transform.Rotation,
                Shape = Shape,
                Material = Material
            };

            gameObject.Scene.Physics.CreateStatic(desc, gameObject);
        }
    }
}
