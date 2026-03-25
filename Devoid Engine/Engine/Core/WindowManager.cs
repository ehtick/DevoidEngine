using DevoidEngine.Engine.Rendering;
using System.Diagnostics;

namespace DevoidEngine.Engine.Core
{
    public class WindowRunState
    {
        public Window window;
        public double fixedAccumulator;
    }

    public class WindowManager
    {
        public int MainThreadID = -1;
        public int UpdateThreadID = -1;
        public int FrameIndex => _frameIndex;

        private readonly List<WindowRunState> windows = new();
        private volatile bool _running = true;
        private int _frameIndex = 0;
        private volatile float _interpolationAlpha = 0f;
        private double _simulationTime = 0.0;

        public static WindowManager Instance { get; } = new WindowManager();
        public bool useVsyncLimiter = false;

        public WindowManager()
        {
            MainThreadID = Thread.CurrentThread.ManagedThreadId;
        }

        public void RegisterWindow(Window window)
        {
            windows.Add(new WindowRunState { window = window });
        }

        public void RunSync()
        {
            const double updateHz = 165;
            const double renderHz = 165;

            double updateStep = 1.0 / updateHz;
            double renderStep = 1.0 / renderHz;

            foreach (var wrs in windows)
                wrs.window.Load();

            Stopwatch timer = Stopwatch.StartNew();

            double nextUpdate = timer.Elapsed.TotalSeconds;
            double nextRender = timer.Elapsed.TotalSeconds;

            while (_running && windows.Count > 0)
            {
                double now = timer.Elapsed.TotalSeconds;

                if (now >= nextUpdate)
                {
                    for (int i = 0; i < windows.Count; i++)
                    {
                        var wrs = windows[i];

                        wrs.window.Update(updateStep);

                        wrs.fixedAccumulator += updateStep;
                        while (wrs.fixedAccumulator >= updateStep)
                        {
                            wrs.window.FixedUpdate(updateStep);
                            wrs.fixedAccumulator -= updateStep;
                        }
                    }

                    nextUpdate += updateStep;
                }

                if (now >= nextRender)
                {
                    for (int i = windows.Count - 1; i >= 0; i--)
                    {
                        var wrs = windows[i];

                        wrs.window.ProcessEvents();

                        if (wrs.window.IsExiting)
                        {
                            wrs.window.Close();
                            windows.RemoveAt(i);
                            continue;
                        }

                        wrs.window.Render(renderStep, 0);
                    }

                    nextRender += renderStep;
                }

                Thread.Sleep(0);
                _running = windows.Count > 0;
            }
        }

        public void Run()
        {
            const float updateHz = 165;
            const float physicsHz = 120;

            const float updateStep = 1f / updateHz;
            const float physicsStep = 1f / physicsHz;

            foreach (var wrs in windows)
                wrs.window.Load();

            _running = true;

            Stopwatch timer = Stopwatch.StartNew();

            double accumulator = 0;
            double lastTime = timer.Elapsed.TotalSeconds;

            while (_running)
            {
                double now = timer.Elapsed.TotalSeconds;
                double frameTime = now - lastTime;
                lastTime = now;

                accumulator += frameTime;


                for (int i = 0; i < windows.Count; i++)
                {
                    windows[i].window.StartFrame();
                    windows[i].window.Update(frameTime);
                }

                while (accumulator >= physicsStep)
                {
                    for (int i = 0; i < windows.Count; i++)
                    {
                        windows[i].window.FixedUpdate(physicsStep);
                    }
                    accumulator -= physicsStep;
                }

                double alpha = accumulator / physicsStep;
                EngineSingleton.Instance.InterpolationAlpha = (float)alpha;


                for (int i = 0; i < windows.Count; i++)
                {
                    windows[i].window.ProcessEvents();
                    windows[i].window.Render(frameTime, (float)alpha);

                    if (windows[i].window.IsExiting)
                    {
                        windows[i].window.Close();
                        windows.RemoveAt(i);
                        continue;
                    }
                    windows[i].window.EndFrame();
                }

                _running = windows.Count > 0;
            }
        }

    }
}