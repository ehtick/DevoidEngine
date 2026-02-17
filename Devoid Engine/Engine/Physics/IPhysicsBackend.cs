using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Physics
{
    public interface IPhysicsBackend
    {
        void Initialize();
        void Step(float deltaTime);


        IPhysicsBody CreateBody(PhysicsBodyDescription desc, GameObject owner);
        void CreateStatic(PhysicsStaticDescription desc, GameObject owner);
        void RemoveBody(IPhysicsBody body);

        //RaycastHit Raycast(Ray ray, float maxDistance);
    }
}
