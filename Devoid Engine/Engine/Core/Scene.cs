using DevoidEngine.Engine.Components;

namespace DevoidEngine.Engine.Core
{
    public class Scene : IDisposable
    {
        private bool _disposed;

        public string Name = "Base Scene";
        public Guid Id = Guid.NewGuid();

        public List<GameObject> GameObjects { get; set; }

        public List<CameraComponent3D> Cameras;
        public List<IRenderComponent> Renderables;


        public bool IsPlaying = false;

        public event Action<Component>? OnComponentAdded;
        public event Action<Component>? OnComponentRemoved;

        public Scene()
        {
            GameObjects = new List<GameObject>();
        }

        public void Initialize()
        {
            Cameras = new List<CameraComponent3D>();
            Renderables = new List<IRenderComponent>();
        }

        public GameObject addGameObject(string name)
        {
            GameObject gameObject = new GameObject();
            gameObject.Name = name;
            gameObject.Scene = this;
            gameObject.Initialize();
            GameObjects.Add(gameObject);
            return gameObject;
        }

        public void addGameObject(GameObject gameObject)
        {
            gameObject.Scene = this;
            GameObjects.Add(gameObject);
        }

        public T GetComponent<T>() where T : Component, new()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                T component = GameObjects[i].GetComponent<T>();

                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        public List<Component> GetComponents()
        {
            List<Component> components = new List<Component>();

            for (int i = 0; i < GameObjects.Count; i++)
            {
                components.AddRange(GameObjects[i].GetComponentsRecursive());
            }
            return components;
        }

        public List<T> GetComponentsOfType<T>() where T : Component, new()
        {
            List<T> components = new List<T>();

            for (int i = 0; i < GameObjects.Count; i++)
            {
                T component = GameObjects[i].GetComponent<T>();

                if (component != null)
                {
                    components.Add(component);
                }
            }
            return components;
        }

        public List<Component> GetAllComponentsOfType(Type componentType)
        {
            List<Component> result = new();

            foreach (var go in GameObjects)
            {
                var allComponents = go.GetComponentsRecursive();
                foreach (var component in allComponents)
                {
                    if (componentType.IsAssignableFrom(component.GetType()))
                    {
                        result.Add(component);
                    }
                }
            }

            return result;
        }



        public void removeGameObject(GameObject gameObject)
        {
            gameObject.OnDestroy();
            GameObjects.Remove(gameObject);
        }

        public void SetMainCamera(CameraComponent3D camera)
        {
            int index = this.Cameras.IndexOf(camera);
            for (int i = 0; i < this.Cameras.Count; i++)
            {
                if (i == index) { continue; }

                this.Cameras[i].IsDefault = false;
            }
        }

        public CameraComponent3D GetMainCamera()
        {
            for (int i = 0; i < this.Cameras.Count; i++)
            {
                if (this.Cameras[i].IsDefault)
                {
                    return this.Cameras[i];
                }
            }
            return null;
        }

        public void AddCamera(CameraComponent3D camera)
        {
            this.Cameras.Add(camera);
        }

        public void RemoveCamera(CameraComponent3D camera)
        {
            this.Cameras.Remove(camera);
        }

        public void Destroy()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.OnDestroy();
            }
        }

        public void Play()
        {
            IsPlaying = true;
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].OnStart();
            }
            foreach (CameraComponent3D camera in Cameras)
            {

            }
        }

        public void Pause()
        {
            IsPlaying = false;

            for (int i = 0; i < GameObjects.Count; i++)
            {
                for (int j = 0; j < GameObjects[i].Components.Count; j++)
                {
                    GameObjects[i].Components[j].IsInitialized = false;
                }
            }
        }

        public void OnUpdate(float dt)
        {
            if (!IsPlaying) { return; }

            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].OnUpdate(dt);
            }
        }

        public void OnRender(float dt)
        {
            if (!IsPlaying) { return; }

            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].OnRender(dt);
            }

            foreach (CameraComponent3D camera in Cameras)
            {

            }
        }

        public void ComponentAdded(Component component)
        {
            OnComponentAdded?.Invoke(component);

            if (component is CameraComponent3D)
            {
                AddCamera((CameraComponent3D)component);
            }

            if (component is IRenderComponent)
            {
                Renderables.Add((IRenderComponent)component);
                Console.WriteLine("[Scene]: Added " + component.Type + " as Renderable.");
            }

            if (IsPlaying)
            {
                component.OnStart();
                component.IsInitialized = true;
            }
        }

        public void ComponentRemoved(Component component)
        {
            if (component is CameraComponent3D)
            {
                RemoveCamera((CameraComponent3D)component);
            }

            OnComponentRemoved?.Invoke(component);
        }

        public void OnResize(float width, float height)
        {
            for (int i = 0; i < Cameras.Count; i++)
            {
                Cameras[i].SetViewportSize((int)width, (int)height);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            // check if already disposed
            if (!_disposed)
            {
                if (disposing)
                {
                    for (int i = 0; i < GameObjects.Count; i++)
                    {
                        GameObjects[i].OnDestroy();
                    }
                    GameObjects.Clear();
                }
                // set the bool value to true
                _disposed = true;
            }
        }

        // The consumer object can call
        // the below dispose method
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Scene()
        {

            Dispose(false);
        }
    }
}
