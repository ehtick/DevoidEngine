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
        GameObject canvas;
        GameObject camera;

        public override void OnInit(Scene main)
        {
            this.scene = main;

            UIButton button = new UIButton();
            button.Setup();

            canvas = scene.addGameObject("Canvas");
            CanvasComponent canvasComponent = canvas.AddComponent<CanvasComponent>();
            

            camera = scene.addGameObject("Camera");
            CameraComponent3D cameraComponent = camera.AddComponent<CameraComponent3D>();
            camera.transform.Position = new System.Numerics.Vector3(0, 0, 0);
        }

        public override void OnUpdate(float delta)
        {
            //cube.transform.Rotation = new System.Numerics.Vector3(cube.transform.Rotation.X + delta * 10, cube.transform.Rotation.Y + delta, cube.transform.Rotation.Z + delta);
        }

    }
}
