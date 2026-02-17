using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Physics
{
    public interface IPhysicsBody
    {
        // --- Transform ---
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }

        // --- Velocity ---
        Vector3 LinearVelocity { get; set; }
        Vector3 AngularVelocity { get; set; }

        // --- Mass ---
        float Mass { get; }
        bool IsKinematic { get; set; }

        // --- Forces ---
        void AddForce(Vector3 force, float dt);
        void AddImpulse(Vector3 impulse);
        void AddTorque(Vector3 torque);

        // --- State ---
        void WakeUp();
        void Sleep();
    }

}
