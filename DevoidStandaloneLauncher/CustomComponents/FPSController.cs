using DevoidEngine.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidStandaloneLauncher.CustomComponents
{
    class FPSController : Component
    {
        public override string Type => nameof(FPSController);

        RigidBodyComponent rb;

        float moveSpeed = 6f;
        float jumpForce = 8f;
        float mouseSensitivity = 0.1f;

        float pitch;

        void OnUpdate(float dt)
        {
            //HandleMouseLook();
            //HandleMovement();
        }
    }

}
