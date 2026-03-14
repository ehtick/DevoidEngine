using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.UI.Text;
using DevoidEngine.Engine.Utilities;
using DevoidGPU;
using DevoidStandaloneLauncher.Utils;
using System.Numerics;

namespace DevoidStandaloneLauncher.Prototypes
{
    internal class MeshLoaderPrototype : Prototype
    {
        Scene scene;

        GameObject PlayerObject;
        GameObject CameraObject;

        Texture2D gridTexture = Helper.loadImageAsTex("LauncherContents/Textures/grid_lvl_design.png", TextureFilter.Linear);
        MaterialInstance celMaterial = new MaterialInstance(new Material(new Shader("LauncherContents/Shaders/celtoon")));
        FileReloader reloader;
        string levelPath = "D:/Programming/Devoid Engine/DevoidStandaloneLauncher/LauncherContents/devoid_l1.fbx";
        public override void OnInit()
        {
            gridTexture.SetWrapMode(TextureWrapMode.Repeat, TextureWrapMode.Repeat);

            LoadDCC();

            Mesh mesh = new Mesh();
            mesh.SetVertices(Primitives.GetCubeVertex());


            reloader = new FileReloader(levelPath, () =>
            {
                Console.WriteLine("FBX changed. Reloading level...");
                LoadLevel();
            });
            LoadLevel();

            //Cursor.SetCursorState(CursorState.Normal);
            //scene.Pause();

        }

        void LoadLevel()
        {
            this.scene = new Scene();
            SceneManager.LoadScene(scene);
            loader.CurrentScene = scene;


            GameObject cube = scene.addGameObject("Glowy");
            MeshRenderer mr = cube.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            mesh.SetVertices(Primitives.GetCubeVertex());
            mr.AddMesh(mesh);
            cube.transform.Position = new Vector3(0, 10, 0);
            RenderThread.Enqueue(() =>
            {
                mr.material.SetVector3("EmissiveColor", new Vector3(1, 1, 1));
                mr.material.SetFloat("EmissiveStrength", 1);
            });

            Importer.LoadModel(levelPath);
            scene.Play();
        }

        int mode = 0;
        int mode1 = DebugRenderSystem.AllowDebugDraw ? 1 : 0;

        public override void OnUpdate(float delta)
        {
            reloader?.Consume();

            if (Input.GetKeyDown(Keys.P))
            {
                mode1 = mode1 == 0 ? 1 : 0;
                DebugRenderSystem.AllowDebugDraw = mode1 == 1 ? true : false;
            }

            if (Input.GetKeyDown(Keys.O))
            {
                mode = mode == 0 ? 1 : 0;
                if (mode == 1)
                {
                    ((ForwardRenderTechnique)RenderBase.ActiveRenderTechnique).renderStateOverride = new RenderState()
                    {
                        FillMode = DevoidGPU.FillMode.Solid,
                        BlendMode = DevoidGPU.BlendMode.AlphaBlend
                    };
                }
                else
                {
                    ((ForwardRenderTechnique)RenderBase.ActiveRenderTechnique).renderStateOverride = new RenderState()
                    {
                        FillMode = DevoidGPU.FillMode.Wireframe,
                        BlendMode = DevoidGPU.BlendMode.AlphaBlend,
                    };
                }
            }
        }

        public void LoadDCC()
        {
            LevelSpawnRegistry.Register("Player_Flycam", (assimpNode, assimpScene) =>
            {
                //Cursor.SetCursorState(CursorState.Grabbed);

                Console.WriteLine("Added freecam");
                GameObject camera = scene.addGameObject("Camera");

                camera.transform.Position = Importer.GetTransform(assimpNode).Item1;

                camera.AddComponent<FreeCameraComponent>();
                var camComponent = camera.AddComponent<CameraComponent3D>();
                camComponent.IsDefault = true;

            });

            LevelSpawnRegistry.Register("AreaTrigger", (assimpNode, assimpScene) =>
            {
                var go = scene.addGameObject(assimpNode.Name);

                Importer.ApplyTransform(go, assimpNode);

                AreaComponent area = go.AddComponent<AreaComponent>();
                area.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Box,
                    Size = Importer.GetTransform(assimpNode).Item3 * 2
                };

                area.OnEnter += (GameObject g) =>
                {
                    Console.WriteLine("Entered Area");
                };

                area.OnExit += (GameObject g) =>
                {
                    Console.WriteLine("Exited Area");
                };
            });


            LevelSpawnRegistry.Register("Model", (assimpNode, assimpScene) =>
            {
                var go = scene.addGameObject(assimpNode.Name);

                Importer.ApplyTransform(go, assimpNode);

                var mesh = Importer.ConvertMesh(assimpNode, assimpScene);
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);
                RenderThread.Enqueue(() =>
                {
                    MaterialInstance material = Importer.ConvertMaterial(assimpNode, assimpScene, levelPath);
                    mr.material = material;
                });
                //RenderThread.Enqueue(() =>
                //{
                //    mr.material = celMaterial;
                //});
            });

            LevelSpawnRegistry.Register("Collideable_Static", (assimpNode, assimpScene) =>
            {
                var go = scene.addGameObject(assimpNode.Name);

                Importer.ApplyTransform(go, assimpNode);

                var mesh = Importer.ConvertMesh(assimpNode, assimpScene);
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);
                RenderThread.Enqueue(() =>
                {
                    MaterialInstance material = Importer.ConvertMaterial(assimpNode, assimpScene, levelPath);
                    mr.material = material;
                });

                var rb = go.AddComponent<StaticCollider>();

                rb.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Box,
                    Size = Importer.GetTransform(assimpNode).Item3 * 2
                };

                rb.Material = new PhysicsMaterial()
                {
                    Friction = 2f
                };

                Console.WriteLine("Collideable Added");
            });


            LevelSpawnRegistry.Register("Player", (assimpNode, assimpScene) =>
            {
                PlayerObject = scene.addGameObject("Player");
                PlayerObject.transform.Position = Importer.GetTransform(assimpNode).Item1;

                var playerBody = PlayerObject.AddComponent<RigidBodyComponent>();
                playerBody.Mass = 1000;
                playerBody.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Capsule,
                    Height = 2f,
                    Radius = 0.5f
                };

                playerBody.Material = new PhysicsMaterial()
                {
                    Friction = 0.8f,
                    Restitution = 0f,
                    AngularDamping = 10f,
                };

                FPSController playerController = PlayerObject.AddComponent<FPSController>();
                playerController.MoveSpeed = 10f;
                playerController.JumpForce = 7f;
                playerController.MouseSensitivity = 0.15f;

                Cursor.SetCursorState(CursorState.Grabbed);

                // Camera pivot (for vertical rotation)
                GameObject cameraPivot = scene.addGameObject("CameraPivot");
                cameraPivot.SetParent(PlayerObject, false);
                playerController.SetCameraPivot(cameraPivot.transform);

                cameraPivot.transform.LocalPosition = new Vector3(0, 1.4f, 0);

                // Camera
                CameraObject = scene.addGameObject("Camera");
                CameraObject.SetParent(cameraPivot, false);

                var camComponent = CameraObject.AddComponent<CameraComponent3D>();
                camComponent.IsDefault = true;
                

            });


            LevelSpawnRegistry.Register("Gun", (assimpNode, assimpScene) =>
            {
                Console.WriteLine("Loaded gun");

                Console.WriteLine(CameraObject);

                GameObject gun = scene.addGameObject("Gun");

                Importer.ApplyTransform(gun, assimpNode);

                gun.SetParent(CameraObject, false);

                // Position gun to the right side of the camera
                gun.transform.LocalPosition = new Vector3(-0.6f, -0.5f, 0.8f);
                //gun.transform.EulerAngles = new Vector3(-90, 90, 0);
                //gun.transform.LocalScale = new Vector3(0.2f, 0.2f, 0.2f);


                var mesh = Importer.ConvertMesh(assimpNode, assimpScene);
                // Add mesh renderer
                var gunRenderer = gun.AddComponent<MeshRenderer>();
                gunRenderer.AddMesh(mesh);

                RenderThread.Enqueue(() =>
                {
                    gunRenderer.material = celMaterial;
                });
                PlayerObject.GetComponent<FPSController>().SetGunTransform(gun.transform);
            });


            LevelSpawnRegistry.Register("Collideable_Dynamic", (assimpNode, assimpScene) =>
            {
                var go = scene.addGameObject(assimpNode.Name);

                Importer.ApplyTransform(go, assimpNode);

                var mesh = Importer.ConvertMesh(assimpNode, assimpScene);
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);
                RenderThread.Enqueue(() =>
                {
                    MaterialInstance material = Importer.ConvertMaterial(assimpNode, assimpScene, levelPath);
                    mr.material = material;
                });

                var rb = go.AddComponent<RigidBodyComponent>();
                rb.Mass = 20;

                rb.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Box,
                    Size = Importer.GetTransform(assimpNode).Item3 * 2,
                    //Height = 2,
                    //Radius = 2
                };

                rb.Material = new PhysicsMaterial()
                {
                    Friction = 2f
                };
            });

            LevelSpawnRegistry.Register("Door_Hinged", (assimpNode, assimpScene) =>
            {
                // 1️⃣ Create hinge pivot
                GameObject hinge = scene.addGameObject(assimpNode.Name + "_Hinge");
                Importer.ApplyTransform(hinge, assimpNode);

                Vector3 size = Importer.GetTransform(assimpNode).Item3 * 2;
                float halfWidth = size.Z * 0.25f; // adjust axis if needed

                // 🔥 Move hinge to door edge (important)
                hinge.transform.Position -=
                    Vector3.Transform(new Vector3(0f, 0f, halfWidth), hinge.transform.Rotation);

                // 2️⃣ Create door body
                GameObject door = scene.addGameObject(assimpNode.Name + "_Body");
                door.SetParent(hinge, false);

                // Door sits centered relative to hinge
                door.transform.LocalPosition = new Vector3(0f, 0f, halfWidth);

                // 3️⃣ Collider on door
                var rb = door.AddComponent<RigidBodyComponent>();
                rb.StartKinematic = true;
                rb.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Box,
                    Size = size
                };

                rb.Material = new PhysicsMaterial()
                {
                    Friction = 1f,
                    Restitution = 0f
                };

                // 4️⃣ Mesh on door
                var mesh = Importer.ConvertMesh(assimpNode, assimpScene);
                MeshRenderer mr = door.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);
                //RenderThread.Enqueue(() =>
                //{
                //    mr.material = celMaterial;
                //});

                // 5️⃣ Rotate hinge only
                hinge.AddComponent<DoorComponent>();
            });

            LevelSpawnRegistry.Register("Trigger_Button", (assimpNode, assimpScene) =>
            {
                GameObject button = scene.addGameObject(assimpNode.Name);

                Importer.ApplyTransform(button, assimpNode);

                var mesh = Importer.ConvertMesh(assimpNode, assimpScene);
                MeshRenderer mr = button.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);
                //RenderThread.Enqueue(() =>
                //{
                //    mr.material = celMaterial;
                //});

                var btnCollider = button.AddComponent<RigidBodyComponent>();
                btnCollider.StartKinematic = true;
                btnCollider.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Box,
                    Size = new Vector3(2.2f, 2.2f, 1f)
                };
                btnCollider.Material = new PhysicsMaterial()
                {
                    Restitution = 0,
                    Friction = 1f
                };


                GameObject button_internal = scene.addGameObject(assimpNode.Name + "internal");
                Importer.ApplyTransform(button_internal, assimpNode);

                var collider = button_internal.AddComponent<AreaComponent>();
                collider.AllowSleep = false;
                collider.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Box,
                    Size = new Vector3(2.2f, 2.2f, 0.2f)
                };


                var buttonComp = button.AddComponent<PortalButtonComponent>();
                buttonComp.SetTriggerArea(collider);

                buttonComp.OnPressed += () =>
                {
                    scene.GetComponentsOfType<DoorComponent>()[0].Turn(true);
                };

                buttonComp.OnReleased += () =>
                {
                    scene.GetComponentsOfType<DoorComponent>()[0].Turn(false);
                };
            });

            LevelSpawnRegistry.Register("Interactable_Cube", (assimpNode, assimpScene) =>
            {
                var go = scene.addGameObject(assimpNode.Name);

                Importer.ApplyTransform(go, assimpNode);

                var mesh = Importer.ConvertMesh(assimpNode, assimpScene);
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);

                //RenderThread.Enqueue(() =>
                //{
                //    mr.material = celMaterial;
                //});

                //RenderThread.Enqueue(() =>
                //{


                //    Texture2D compCubeTex = Helper.loadImageAsTex("D:/Programming/Devoid Engine/DevoidStandaloneLauncher/LauncherContents/companion_cube.png", DevoidGPU.TextureFilter.Linear);
                //    mr.material.SetTexture("MAT_AlbedoMap", compCubeTex);
                //});

                var rb = go.AddComponent<RigidBodyComponent>();
                rb.Mass = 10f;
                rb.Shape = new PhysicsShapeDescription()
                {
                    Type = PhysicsShapeType.Box,
                    Size = Importer.GetTransform(assimpNode).Item3 * 2
                };

                rb.Material = new PhysicsMaterial()
                {
                    Friction = 2f,
                    Restitution = 0f
                };

                go.AddComponent<PortalCubeComponent>();
            });

            LevelSpawnRegistry.RegisterLight((assimpNode, assimpLight) =>
            {
                GameObject lightGO = scene.addGameObject(assimpNode.Name);

                Importer.ApplyTransform(lightGO, assimpNode);

                var lightComponent = lightGO.AddComponent<LightComponent>();

                switch (assimpLight.LightType)
                {
                    case Assimp.LightSourceType.Point:
                        lightComponent.LightType = LightType.PointLight;
                        break;
                    case Assimp.LightSourceType.Directional:
                        lightComponent.LightType = LightType.DirectionalLight;
                        break;
                    case Assimp.LightSourceType.Spot:
                        lightComponent.LightType = LightType.SpotLight;
                        break;
                }

                lightComponent.Color = new Vector4(
                    assimpLight.ColorDiffuse.R,
                    assimpLight.ColorDiffuse.G,
                    assimpLight.ColorDiffuse.B,
                    1f);

                lightComponent.Radius = 200f;
                lightComponent.Intensity = 20f; // your scale
            });
        }

    }
}
