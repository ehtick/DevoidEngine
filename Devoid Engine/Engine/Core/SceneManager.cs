namespace DevoidEngine.Engine.Core
{
    public static class SceneManager
    {
        public static Scene MainScene;

        static SceneManager()
        {

        }

        public static void LoadScene(int index)
        {

        }

        public static void LoadScene(Scene scene)
        {
            MainScene = scene;

            scene.Initialize();
        }

        public static void Update(float delta)
        {
            MainScene?.OnUpdate(delta);
        }

        public static void Render(float delta)
        {
            MainScene?.OnRender(delta);
        }

        public static bool IsSceneLoaded()
        {
            return MainScene != null;
        }
    }
}
