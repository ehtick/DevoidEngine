using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using Elemental.Editor.Panels;
using Elemental.Editor.Utils;

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


            EditorUI.SetEditorStyling();

            EditorUI.DefaultFontLarge = application.ImGuiRenderer.AddFontFromFile("Editor/Content/Fonts/JetBrainsMono-Bold.ttf", 32);
            EditorUI.DefaultFontMedium = application.ImGuiRenderer.AddFontFromFile("Editor/Content/Fonts/JetBrainsMono-Regular.ttf", 21);
            EditorUI.DefaultFontSmall = application.ImGuiRenderer.AddFontFromFile("Editor/Content/Fonts/JetBrainsMono-Regular.ttf", 19);


            EditorUI.DefaultIconFont = application.ImGuiRenderer.LoadIconFont("Editor/Content/Fonts/bootstrap-icons.ttf", 19, (BootstrapIconFont.IconMin, BootstrapIconFont.IconMax16));

            application.ImGuiRenderer.ConfigureFontAtlas();
            application.ImGuiRenderer.SetDefaultFont(EditorUI.DefaultFontSmall);


            SceneManager.LoadScene(new Scene());

            SceneManager.MainScene.Play();

            AddPanel<ViewportPanel>();
            AddPanel<InspectorPanel>();
            AddPanel<ProjectPanel>();
            AddPanel<GameObjectListPanel>();

            foreach (var panel in Panels)
            {
                panel.OnAttach();
            }

            SetupSandbox();

        }

        void SetupSandbox()
        {
            GameObject gameObject = SceneManager.MainScene.addGameObject("SandboxObj#1");
            CameraComponent3D camera = gameObject.AddComponent<CameraComponent3D>();
            camera.IsDefault = true;
        }

        public override void OnDetach()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var panel in Panels)
            {
                panel.OnUpdate(deltaTime);
            }
        }

        public override void OnRender(float deltaTime)
        {

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
