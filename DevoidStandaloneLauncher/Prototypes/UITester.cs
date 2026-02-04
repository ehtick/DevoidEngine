using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.UI;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidStandaloneLauncher.Prototypes
{
    internal class UITester : Prototype
    {
        Scene scene;
        GameObject cube;
        GameObject camera;

        public override void OnInit(Scene main)
        {
            UIButton button = new UIButton();
            button.Setup();
        }

        public override void OnUpdate(float delta)
        {
            //cube.transform.Rotation = new System.Numerics.Vector3(cube.transform.Rotation.X + delta * 10, cube.transform.Rotation.Y + delta, cube.transform.Rotation.Z + delta);
        }

    }
}
