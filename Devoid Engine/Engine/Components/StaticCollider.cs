using DevoidEngine.Engine.Core;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class StaticCollider : Component
    {
        public override string Type => nameof(StaticCollider);

        public Vector3 Size = new Vector3(10, 1, 10);

        public override void OnStart()
        {
            gameObject.Scene.Physics.CreateStaticBox(
                gameObject.transform.Position,
                Size,
                gameObject
            );
        }
    }
}
