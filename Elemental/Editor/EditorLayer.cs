using DevoidEngine.Engine.Core;
using Elemental.Editor.Utils;
using ImGuiNET;
using System;

namespace Elemental.Editor
{
    public class EditorLayer : Layer
    {

        public override void OnAttach()
        {
            EditorUI.SetEditorStyling();
        }

        public override void OnDetach()
        {
            
        }

        public override void OnGUIRender()
        {
            if (ImGui.Begin("Viewport"))
            {

            }
            ImGui.End();
        }

    }
}
