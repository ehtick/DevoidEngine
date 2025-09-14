using DevoidEngine.Editor.Utils;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using Elemental.Editor.Utils;
using ImGuiNET;
using MaterialIconFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Elemental.Editor.Utils.ButtonGroup;

namespace Elemental.Editor.Panels
{
    internal class ViewportPanel : Panel
    {
        List<ViewportControl> controls;

        List<MonitorResolution> supportedResolutions;
        MonitorResolution selectedResolution;

        public override void OnAttach()
        {
            supportedResolutions = editor.application.MainWindow.GetSupportedResolutions();

            foreach(var resolution in supportedResolutions)
            {
                if (resolution.height == 1080 &&  resolution.width == 1920)
                {
                    selectedResolution = resolution;
                    Renderer.Resize(resolution.width, resolution.height);
                    Screen.Size.X = resolution.width;
                    Screen.Size.Y = resolution.height;
                    SceneManager.MainScene.OnResize(resolution.width, resolution.height);
                }
            }

            controls = new List<ViewportControl>
            {
                new ViewportControl(ViewportAlignment.Left, () =>
                {
                    if (ImGui.BeginCombo("##ResCombo", $"{selectedResolution.width}x{selectedResolution.height}", ImGuiComboFlags.HeightLargest))
                    {
                        for (int i = 0; i < supportedResolutions.Count; i++)
                        {
                            var res = supportedResolutions[i];
                            bool isSelected = selectedResolution.Equals(res);
                            if (ImGui.Selectable($"{res.width}x{res.height}", isSelected))
                            {
                                selectedResolution = res;

                                Renderer.Resize(res.width, res.height);
                                Screen.Size.X = res.width;
                                Screen.Size.Y = res.height;
                                SceneManager.MainScene.OnResize(res.width, res.height);

                                //DebugLogPanel.Log("RESIZED RENDERER", DebugLogPanel.DebugMessageSeverity.Information, "Viewport Change");
                            }
                        }

                        ImGui.EndCombo();
                    }
                }, new Vector2(200f)),

                new ViewportControl(
                ViewportAlignment.Right,
                () =>
                {

                },
                new (400, 40)
                ),

                new ViewportControl(ViewportAlignment.Right, () =>
                {
                    var btnGroup = new ButtonGroup("Test Group 2", ButtonGroup.RenderMode.CustomDraw);

                    btnGroup.Add(BootstrapIconFont.GearFill, () =>
                    {
                        ImGui.OpenPopup("OptionsPopup");


                    });

                    

                    btnGroup.Render();

                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));



                    ImGui.PopStyleVar();
                },
                    new Vector2(200, 40)
                )
            };


        }

        public override void OnGUI()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

            if (ImGui.Begin("Viewport"))
            {
                ViewportUtil.DrawViewportTools(60, controls);

                if (SceneManager.MainScene.GetMainCamera() != null)
                {
                    ImGui.Image(SceneManager.MainScene.GetMainCamera().Camera.RenderTarget.GetRenderTexture(0).GetDeviceTexture().GetHandle(), new System.Numerics.Vector2(600, 400));
                }
            }

            ImGui.End();


            ImGui.PopStyleVar();
        }


    }
}
