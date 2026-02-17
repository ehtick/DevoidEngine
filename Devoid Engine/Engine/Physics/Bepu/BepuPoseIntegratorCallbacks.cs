using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities;
using System.Numerics;

namespace DevoidEngine.Engine.Physics.Bepu
{

    internal struct BepuPoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        public Vector3 Gravity;
        public float LinearDamping;
        public float AngularDamping;


        private Vector3Wide gravityWide;

        public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.ConserveMomentum;

        public bool AllowSubstepsForUnconstrainedBodies => false;

        public bool IntegrateVelocityForKinematics => false;

        public void Initialize(Simulation simulation)
        {
            // Convert gravity into wide representation once
            gravityWide = Vector3Wide.Broadcast(Gravity);
        }

        public void PrepareForIntegration(float dt) { }

        public void IntegrateVelocity(
            Vector<int> bodyIndices,
            Vector3Wide position,
            QuaternionWide orientation,
            BodyInertiaWide localInertia,
            Vector<int> integrationMask,
            int workerIndex,
            Vector<float> dt,
            ref BodyVelocityWide velocity)
        {
            // velocity.Linear += gravity * dt
            Vector3Wide.Scale(gravityWide, dt, out var gravityStep);
            Vector3Wide.Add(velocity.Linear, gravityStep, out velocity.Linear);
        }
    }

}
