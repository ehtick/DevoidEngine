using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Components
{
    public class PortalComponent : Component
    {
        public override string Type => nameof(PortalComponent);

        public override void OnStart()
        {
            gameObject.Scene.addGameObject("Camera1");
        }
    }
}
