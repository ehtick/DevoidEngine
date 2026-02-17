using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using System.Numerics;

namespace DevoidEngine.Engine.Physics.Bepu
{

    internal struct BepuNarrowPhaseCallbacks : INarrowPhaseCallbacks
    {
        public Func<CollidableReference, PhysicsMaterial> MaterialLookup;

        public void Initialize(Simulation simulation)
        {

        }

        public void Dispose() { }

        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
            => true;

        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
            => true;

        public bool ConfigureContactManifold<TManifold>(
            int workerIndex,
            CollidablePair pair,
            ref TManifold manifold,
            out PairMaterialProperties pairMaterial)
            where TManifold : unmanaged, IContactManifold<TManifold>
        {
            var matA = MaterialLookup(pair.A);
            var matB = MaterialLookup(pair.B);

            float restitution = (matA.Restitution + matB.Restitution) * 0.5f;

            pairMaterial = new PairMaterialProperties
            {
                FrictionCoefficient = (matA.Friction + matB.Friction) * 0.5f,
                MaximumRecoveryVelocity = 2f + restitution * 10f,
                SpringSettings = new SpringSettings(30, 1)
            };


            return true;
        }


        public bool ConfigureContactManifold(
            int workerIndex,
            CollidablePair pair,
            int childIndexA,
            int childIndexB,
            ref ConvexContactManifold manifold)
        {
            return true;
        }
    }

}
