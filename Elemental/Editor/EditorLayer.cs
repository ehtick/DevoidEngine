using DevoidEngine.Engine.Core;
using Elemental.Editor.Panels;
using Elemental.Editor.Utils;
using ImGuiNET;
using System;

namespace Elemental.Editor
{
    public class EditorLayer : Layer
    {
        public Application application;

        public List<Panel> Panels = new List<Panel>();

        public void AddPanel<T>() where T : Panel, new()
        {
            Panel panel = new T();
            panel.editor = this;
            Panels.Add(panel);

        }

        public override void OnAttach()
        {
            AddPanel<ViewportPanel>();
            AddPanel<InspectorPanel>();
            AddPanel<ProjectPanel>();
            AddPanel<GameObjectListPanel>();


            EditorUI.SetEditorStyling();
            
            EditorUI.DefaultFontLarge = application.ImGuiRenderer.AddFontFromFile("Editor/Content/Fonts/JetBrainsMono-Bold.ttf", 32);
            EditorUI.DefaultFontMedium = application.ImGuiRenderer.AddFontFromFile("Editor/Content/Fonts/JetBrainsMono-Regular.ttf", 21);
            EditorUI.DefaultFontSmall = application.ImGuiRenderer.AddFontFromFile("Editor/Content/Fonts/JetBrainsMono-Regular.ttf", 19);


            EditorUI.DefaultIconFont = application.ImGuiRenderer.LoadIconFont("Editor/Content/Fonts/bootstrap-icons.ttf", 19, (BootstrapIconFont.IconMin, BootstrapIconFont.IconMax16));

            application.ImGuiRenderer.ConfigureFontAtlas();
            application.ImGuiRenderer.SetDefaultFont(EditorUI.DefaultFontSmall);

            foreach (var panel in Panels)
            {
                panel.OnAttach();
            }


            SceneManager.LoadScene(new Scene());

            SceneManager.MainScene.Play();

        }

        public override void OnDetach()
        {
            
        }

        public override void OnUpdate(float deltaTime)
        {
            SceneManager.Update(deltaTime);
            foreach (var panel in Panels)
            {
                panel.OnUpdate(deltaTime);
            }
        }

        public override void OnGUIRender()
        {
            foreach (var panel in Panels)
            {
                panel.OnGUI();
            }
        }

    }
}
