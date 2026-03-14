using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    public static class ConsoleCommands
    {
        public static Dictionary<string, Action<string[], DebugConsole>> Build()
        {
            var cmds = new Dictionary<string, Action<string[], DebugConsole>>();

            RegisterGeneral(cmds);
            RegisterScene(cmds);
            RegisterTransform(cmds);
            RegisterPhysics(cmds);
            RegisterRendering(cmds);
            RegisterDebug(cmds);

            return cmds;
        }

        // ===============================
        // GENERAL
        // ===============================

        static void RegisterGeneral(Dictionary<string, Action<string[], DebugConsole>> cmds)
        {
            cmds["help"] = (args, c) =>
            {
                c.Log("Commands:");
                foreach (var cmd in cmds.Keys)
                    c.Log(" - " + cmd);
            };

            cmds["clear"] = (args, c) =>
            {
                c.logs.Clear();
                c.RefreshLogs();
            };

            cmds["echo"] = (args, c) =>
            {
                c.Log(string.Join(" ", args));
            };

            cmds["time"] = (args, c) =>
            {
                c.Log(DateTime.Now.ToString());
            };

            cmds["random"] = (args, c) =>
            {
                var r = new Random();
                c.Log("Random: " + r.Next());
            };
        }

        // ===============================
        // SCENE COMMANDS
        // ===============================

        static void RegisterScene(Dictionary<string, Action<string[], DebugConsole>> cmds)
        {
            cmds["list"] = (args, c) =>
            {
                var objects = SceneManager.CurrentScene.GameObjects;
                c.Log($"GameObjects ({objects.Count})");

                foreach (var go in objects)
                    c.Log(go.Name);
            };

            cmds["count"] = (args, c) =>
            {
                int count = SceneManager.CurrentScene.GameObjects.Count;
                c.Log($"GameObject count: {count}");
            };

            cmds["spawn"] = (args, c) =>
            {
                string name = args.Length > 0 ? args[0] : "GameObject";
                var go = SceneManager.CurrentScene.addGameObject(name);
                c.Log($"Spawned {go.Name}");
            };

            cmds["destroy"] = (args, c) =>
            {
                if (args.Length == 0) return;

                var scene = SceneManager.CurrentScene;
                var go = scene.GameObjects.FirstOrDefault(x => x.Name == args[0]);

                if (go == null)
                {
                    c.Log("Object not found");
                    return;
                }

                scene.Destroy(go);
                c.Log("Destroyed " + go.Name);
            };

            cmds["find"] = (args, c) =>
            {
                if (args.Length == 0) return;

                foreach (var go in SceneManager.CurrentScene.GameObjects)
                {
                    if (go.Name.Contains(args[0], StringComparison.OrdinalIgnoreCase))
                        c.Log(go.Name);
                }
            };

            cmds["components"] = (args, c) =>
            {
                if (args.Length == 0) return;

                var go = SceneManager.CurrentScene.GameObjects
                    .FirstOrDefault(x => x.Name == args[0]);

                if (go == null)
                {
                    c.Log("Object not found");
                    return;
                }

                foreach (var comp in go.Components)
                    c.Log(comp.Type);
            };
        }

        // ===============================
        // TRANSFORM
        // ===============================

        static void RegisterTransform(Dictionary<string, Action<string[], DebugConsole>> cmds)
        {
            cmds["pos"] = (args, c) =>
            {
                if (args.Length == 0) return;

                var go = FindObject(args[0]);
                if (go == null) return;

                Vector3 p = go.transform.Position;
                c.Log($"{go.Name} position {p.X} {p.Y} {p.Z}");
            };

            cmds["setpos"] = (args, c) =>
            {
                if (args.Length < 4) return;

                var go = FindObject(args[0]);
                if (go == null) return;

                go.transform.Position = new Vector3(
                    float.Parse(args[1]),
                    float.Parse(args[2]),
                    float.Parse(args[3])
                );

                c.Log("Position set");
            };

            cmds["move"] = (args, c) =>
            {
                if (args.Length < 4) return;

                var go = FindObject(args[0]);
                if (go == null) return;

                go.transform.Position += new Vector3(
                    float.Parse(args[1]),
                    float.Parse(args[2]),
                    float.Parse(args[3])
                );

                c.Log("Moved object");
            };

            cmds["rotate"] = (args, c) =>
            {
                if (args.Length < 4) return;

                var go = FindObject(args[0]);
                if (go == null) return;

                go.transform.EulerAngles = new Vector3(
                    float.Parse(args[1]),
                    float.Parse(args[2]),
                    float.Parse(args[3])
                );

                c.Log("Rotation updated");
            };

            cmds["scale"] = (args, c) =>
            {
                if (args.Length < 4) return;

                var go = FindObject(args[0]);
                if (go == null) return;

                go.transform.Scale = new Vector3(
                    float.Parse(args[1]),
                    float.Parse(args[2]),
                    float.Parse(args[3])
                );

                c.Log("Scale updated");
            };
        }

        // ===============================
        // PHYSICS
        // ===============================

        static void RegisterPhysics(Dictionary<string, Action<string[], DebugConsole>> cmds)
        {
            cmds["addrigidbody"] = (args, c) =>
            {
                var go = FindObject(args[0]);
                if (go == null) return;

                RigidBodyComponent rb = go.AddComponent<RigidBodyComponent>();
                rb.Mass = 100;
                rb.Material = new Physics.PhysicsMaterial()
                {
                    Restitution = 1,
                    Friction = 1,
                };

                rb.Shape = new Physics.PhysicsShapeDescription()
                {
                    Type = Physics.PhysicsShapeType.Box,
                    Size = go.transform.Scale
                };


                c.Log("RigidBody added");
            };

            cmds["addstaticcollider"] = (args, c) =>
            {
                var go = FindObject(args[0]);
                if (go == null) return;

                StaticCollider rb = go.AddComponent<StaticCollider>();
                rb.Material = new Physics.PhysicsMaterial()
                {
                    Restitution = 1,
                    Friction = 1,
                };

                rb.Shape = new Physics.PhysicsShapeDescription()
                {
                    Type = Physics.PhysicsShapeType.Box,
                    Size = go.transform.Scale
                };


                c.Log("StaticCollider added");
            };

            cmds["force"] = (args, c) =>
            {
                if (args.Length < 4) return;

                var go = FindObject(args[0]);
                if (go == null) return;

                var rb = go.GetComponent<RigidBodyComponent>();
                if (rb == null)
                {
                    c.Log("No Rigidbody");
                    return;
                }
                rb.WakeUp();
                rb.AddForce(new Vector3(
                    float.Parse(args[1]),
                    float.Parse(args[2]),
                    float.Parse(args[3])
                ));

                c.Log("Force applied");
            };

            cmds["impulse"] = (args, c) =>
            {
                if (args.Length < 4) return;

                var go = FindObject(args[0]);
                var rb = go?.GetComponent<RigidBodyComponent>();
                if (rb == null) return;

                rb.AddImpulse(new Vector3(
                    float.Parse(args[1]),
                    float.Parse(args[2]),
                    float.Parse(args[3])
                ));

                c.Log("Impulse applied");
            };

            cmds["velocity"] = (args, c) =>
            {
                if (args.Length < 4) return;

                var go = FindObject(args[0]);
                var rb = go?.GetComponent<RigidBodyComponent>();
                if (rb == null) return;

                rb.LinearVelocity = new Vector3(
                    float.Parse(args[1]),
                    float.Parse(args[2]),
                    float.Parse(args[3])
                );

                c.Log("Velocity set");
            };
        }

        // ===============================
        // RENDERING
        // ===============================

        static void RegisterRendering(Dictionary<string, Action<string[], DebugConsole>> cmds)
        {
            cmds["spawncube"] = (args, c) =>
            {
                Mesh mesh = new Mesh();
                mesh.SetVertices(Primitives.GetCubeVertex());

                var go = SceneManager.CurrentScene.addGameObject("ConsoleCube");

                var mr = go.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);

                if (args.Length == 3)
                {
                    go.transform.Position = new Vector3(
                        float.Parse(args[0]),
                        float.Parse(args[1]),
                        float.Parse(args[2])
                    );
                }

                c.Log("Cube spawned");
            };

            cmds["spawnlight"] = (args, c) =>
            {
                var go = SceneManager.CurrentScene.addGameObject("ConsoleLight");

                var light = go.AddComponent<LightComponent>();

                if (args.Length == 3)
                {
                    go.transform.Position = new Vector3(
                        float.Parse(args[0]),
                        float.Parse(args[1]),
                        float.Parse(args[2])
                    );
                }

                c.Log("Light spawned");
            };

            cmds["lightintensity"] = (args, c) =>
            {
                if (args.Length < 2) return;

                var go = FindObject(args[0]);
                var light = go?.GetComponent<LightComponent>();

                if (light == null) return;

                light.Intensity = float.Parse(args[1]);

                c.Log("Light intensity updated");
            };
        }

        // ===============================
        // DEBUG
        // ===============================

        static void RegisterDebug(Dictionary<string, Action<string[], DebugConsole>> cmds)
        {
            cmds["cursor"] = (args, c) =>
            {
                if (args.Length == 0) return;

                switch (args[0])
                {
                    case "normal":
                        Cursor.SetCursorState(CursorState.Normal);
                        break;

                    case "hidden":
                        Cursor.SetCursorState(CursorState.Hidden);
                        break;

                    case "grab":
                        Cursor.SetCursorState(CursorState.Grabbed);
                        break;
                }

                c.Log("Cursor updated");
            };

            cmds["memory"] = (args, c) =>
            {
                long mem = GC.GetTotalMemory(false) / (1024 * 1024);
                c.Log($"Memory: {mem} MB");
            };

            cmds["gc"] = (args, c) =>
            {
                GC.Collect();
                c.Log("Garbage collected");
            };

            cmds["threads"] = (args, c) =>
            {
                c.Log($"Main Thread: {RenderThread.mainThreadID}");
            };
        }

        // ===============================
        // UTIL
        // ===============================

        static GameObject FindObject(string name)
        {
            return SceneManager.CurrentScene.GameObjects
                .FirstOrDefault(x => x.Name == name);
        }
    }
}