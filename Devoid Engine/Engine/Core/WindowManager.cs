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

        ManualResetEventSlim updateStart = new(false);
        ManualResetEventSlim updateDone = new(false);
        double _currentUpdateStep;

        public void RunTicked()
        {
            const int updatesPerFrame = 1;

            foreach (var wrs in windows)
            {
                wrs.window.Load();
            }


            _running = true;
            Thread updateThread = new Thread(() =>
            {
                while (_running)
                {
                    updateStart.Wait();
                    updateStart.Reset();

                    if (!_running)
                        break;

                    double step = Interlocked.CompareExchange(ref _currentUpdateStep, 0, 0);

                    for (int u = 0; u < updatesPerFrame; u++)
                    {
                        for (int i = windows.Count - 1; i >= 0; i--)
                            windows[i].window.Update(step);
                    }

                    updateDone.Set();
                }
            })
            {
                IsBackground = true,
                Name = "Update Thread"
            };

            updateThread.Start();
            Stopwatch timer = Stopwatch.StartNew();
            double lastRenderTime = timer.Elapsed.TotalSeconds;

            while (_running)
            {
                if (windows.Count == 0)
                    break;

                double now = timer.Elapsed.TotalSeconds;
                double dt = now - lastRenderTime;
                lastRenderTime = now;

                if (dt > 0.25)
                    dt = 0.25;
                Interlocked.Exchange(ref _currentUpdateStep, dt / updatesPerFrame);

                updateDone.Reset();
                updateStart.Set();
                updateDone.Wait();

                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    var window = windows[i].window;

                    window.ProcessEvents();

                    if (window.IsExiting)
                    {
                        window.Close();
                        windows.RemoveAt(i);
                        continue;
                    }

                    window.Render(dt, 0);
                }

                _running = windows.Count > 0;
            }

            _running = false;
            updateStart.Set();
            updateThread.Join();
        }

        private volatile float _interpolationAlpha = 0f;
        private double _simulationTime = 0.0;

        public void RunTickedTrue()
        {
            const int updatesPerFrame = 1;

            foreach (var wrs in windows)
            {
                wrs.window.Load();
            }


            _running = true;
            const double fixedDt = 1.0 / 30.0;

            Thread updateThread = new Thread(() =>
            {
                Stopwatch timer = Stopwatch.StartNew();
                double lastTime = timer.Elapsed.TotalSeconds;

                double accumulator = 0.0;

                while (_running)
                {
                    double now = timer.Elapsed.TotalSeconds;
                    double dt = now - lastTime;
                    lastTime = now;

                    if (dt > 0.25)
                        dt = 0.25;

                    accumulator += dt;

                    // ---- FIXED UPDATE ----
                    while (accumulator >= fixedDt)
                    {
                        // capture previous BEFORE simulation

                        for (int i = 0; i < windows.Count; i++)
                            windows[i].window.FixedUpdate(fixedDt);

                        accumulator -= fixedDt;
                    }

                    for (int i = 0; i < windows.Count; i++)
                        windows[i].window.Update(dt);

                    // ---- COMPUTE ALPHA ----
                    float alpha = (float)(accumulator / fixedDt);
                    alpha = Math.Clamp(alpha, 0f, 1f);

                    Volatile.Write(ref _interpolationAlpha, alpha);

                    Thread.Yield();
                }
            })
            {
                Name = "Update Thread",
                IsBackground = true
            };

            updateThread.Start();
            Stopwatch timer = Stopwatch.StartNew();
            double lastRenderTime = timer.Elapsed.TotalSeconds;

            while (_running)
            {
                if (windows.Count == 0)
                {
                    _running = false;
                    break;
                }

                double now = timer.Elapsed.TotalSeconds;
                double dt = now - lastRenderTime;
                lastRenderTime = now;

                if (dt > 0.25)
                    dt = 0.25;

                float alpha = Volatile.Read(ref _interpolationAlpha);
                EngineSingleton.Instance.InterpolationAlpha = alpha;

                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    var window = windows[i].window;

                    window.ProcessEvents();

                    if (window.IsExiting)
                    {
                        window.Close();
                        windows.RemoveAt(i);
                        continue;
                    }

                    window.Render((float)dt, alpha);
                }

                _frameIndex++;
            }
            _running = false;
            updateThread.Join();
        }
    }
}