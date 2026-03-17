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
        ContainerNode logAreaBackground;

        public List<string> logs = new();
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
            logAreaBackground = new ContainerNode()
            {
                Size = new Vector2(600, 350),
                Color = new Vector4(0, 0, 0, 1f),
                Padding = new Padding()
                {
                    Top = 5,
                    Left = 5,
                    Right = 5,
                    Bottom = 5
                },
            };

            logAreaContainer = new ContainerNode()
            {
                Color = new Vector4(0, 0.188f, 0.286f, 0.8f),
                Padding = new Padding()
                {
                    Top = 7,
                    Left = 7,
                    Right = 7,
                    Bottom = 7
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

            consolePanel.Add(logAreaBackground);
            consolePanel.Add(inputLabel);
            logAreaBackground.Add(logAreaContainer);
            logAreaContainer.Add(logArea);

            rootNode.Add(consolePanel);

            inputLabel.OnSubmit += ExecuteCommand;

            rootNode.Visible = false;
        }

        void RegisterCommands()
        {
            var registry = ConsoleCommands.Build();

            foreach (var c in registry)
            {
                commands[c.Key] = args => c.Value(args, this);
            }
        }

        void ExecuteCommand(string input)
        {
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

        public void Log(string text)
        {
            logs.Add(text);
            RefreshLogs();
        }

        public void RefreshLogs()
        {
            logArea.Clear();

            foreach (var log in logs)
            {
                logArea.Add(new LabelNode(log, font, 14)
                {
                    Color = new Vector4(1, 0.8f, 0, 1)
                });
            }

            logArea.ScrollToBottom();
        }

        public override void OnDetach()
        {

        }

        CursorState prevCursorState;


        public override void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(Keys.F7))
            {
                rootNode.Visible = !rootNode.Visible;

                if (rootNode.Visible)
                {
                    prevCursorState = Cursor.GetCursorState();

                    Cursor.SetCursorState(CursorState.Normal);
                }
                else
                {
                    Cursor.SetCursorState(prevCursorState);
                }
            }
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

            Framebuffer surface = SceneManager.CurrentScene?.GetMainCamera()?.Camera.RenderTarget;

            if (surface == null)
            {
                return;
            }

            surface.Bind();
            RenderBase.SetupCamera(UIRenderer.ScreenData);
            RenderBase.Execute(renderItems, debugRenderState);

        }
    }
}
