using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.UI.Text;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DevoidEngine.Engine.Core
{
    public class DebugConsole : Layer
    {
        public CanvasNode rootNode = new CanvasNode()
        {
            
        };
        public RenderState debugRenderState = new RenderState()
        {
            BlendMode = DevoidGPU.BlendMode.AlphaBlend,
            CullMode = DevoidGPU.CullMode.None,
            DepthTest = DevoidGPU.DepthTest.Disabled,
            DepthWrite = false,
            FillMode = DevoidGPU.FillMode.Solid,
            PrimitiveType = DevoidGPU.PrimitiveType.Triangles
        };

        FontInternal font;

        FlexboxNode consolePanel;
        ScrollNode logArea;
        ContainerNode logAreaContainer;

        InputFieldNode inputLabel;
        BoxNode background;

        List<string> logs = new();
        string currentInput = "";

        Dictionary<string, Action<string[]>> commands = new();

        public override void OnAttach()
        {
            font = FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 20);


            RegisterCommands();

            UISystem.Roots.Add(rootNode);

            consolePanel = new FlexboxNode()
            {
                ParticipatesInLayout = false,
                Offset = new Vector2(100, 30),
                Size = new Vector2(600, 350),
                Direction = FlexDirection.Column,
                //Gap = 4,
                Justify = JustifyContent.Center
            };

            // background
            background = new BoxNode()
            {
                Size = new Vector2(600, 350),
                Color = new Vector4(0, 0, 0, 0.2f)
            };

            logAreaContainer = new ContainerNode()
            {
                Color = new Vector4(1, 0, 0, 0f),
                Padding = new Padding()
                {
                    Top = 50,
                    Left = 5
                }
            };

            logArea = new ScrollNode()
            {
                Direction = FlexDirection.Column,
                MaxSize = new Vector2(600, 350),
                Gap = 2,
            };

            inputLabel = new InputFieldNode(font)
            {
                Text = "",
                
            };

            consolePanel.Add(background);
            consolePanel.Add(inputLabel);
            background.Add(logAreaContainer);
            logAreaContainer.Add(logArea);

            rootNode.Add(consolePanel);

            inputLabel.OnSubmit += ExecuteCommand;
        }

        void RegisterCommands()
        {
            commands["help"] = args =>
            {
                Log("Available commands:");
                foreach (var cmd in commands.Keys)
                    Log(" - " + cmd);
            };

            commands["list"] = args =>
            {
                List<GameObject> gameObjects = SceneManager.CurrentScene.GameObjects;
                Log($"GameObjects ({gameObjects.Count}):");
                foreach (GameObject go in gameObjects)
                    Log(" " + go.Name);

            };

            commands["spawnCube"] = args =>
            {
                Mesh mesh = new Mesh();
                mesh.SetVertices(Primitives.GetCubeVertex());

                GameObject go = SceneManager.CurrentScene.addGameObject("CubeObject-Console");
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.AddMesh(mesh);

                if (args.Length == 3)
                {
                    go.transform.Position = new Vector3(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
                }


                Log("Successfully Added GameObject Cube");
            };

            commands["clear"] = args =>
            {
                logs.Clear();
                RefreshLogs();
            };

            commands["echo"] = args =>
            {
                Log(string.Join(" ", args));
            };
        }

        void ExecuteCommand(string input)
        {
            Console.WriteLine(input);
            Log("> " + input);

            if (string.IsNullOrWhiteSpace(input))
                return;

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string cmd = parts[0];
            string[] args = parts.Skip(1).ToArray();

            if (commands.TryGetValue(cmd, out var command))
            {
                command(args);
            }
            else
            {
                Log("Unknown command: " + cmd);
            }
        }

        void Log(string text)
        {
            logs.Add(text);
            RefreshLogs();
        }

        void RefreshLogs()
        {
            logArea.Clear();

            foreach (var log in logs)
            {
                logArea.Add(new LabelNode(log, font, 16));
            }

            logArea.ScrollToBottom();
        }

        public override void OnDetach()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            
        }

        public override void OnRender(float deltaTime)
        {
            //Matrix4x4 model = UISystem.BuildModel(new UITransform(new Vector2(10, 10), new Vector2(100, 100)));
            //DebugRenderSystem.DrawRectUI(model);

            //DebugRenderSystem.DrawRectUI(
            //    UISystem.BuildModel(background.Rect)
            //);
        }

        public override void OnLateRender()
        {
            List<RenderItem> renderItems = new List<RenderItem>();
            rootNode.Render(renderItems, Matrix4x4.Identity);

            Framebuffer surface = SceneManager.CurrentScene.GetMainCamera().Camera.RenderTarget;

            surface.Bind();
            RenderBase.SetupCamera(UIRenderer.ScreenData);
            RenderBase.Execute(renderItems, debugRenderState);

        }
    }
}
