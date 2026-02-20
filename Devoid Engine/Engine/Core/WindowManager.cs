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
        private readonly List<WindowRunState> windows = new();
        private volatile bool _running = true;

        public static WindowManager Instance { get; } = new WindowManager();

        public void RegisterWindow(Window window)
        {
            windows.Add(new WindowRunState { window = window });
        }

        public void RunAllSync()
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

                // -------- UPDATE --------
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

                // -------- RENDER --------
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

                        wrs.window.Render(renderStep);
                    }

                    nextRender += renderStep;
                }

                Thread.Sleep(0); // yield
                _running = windows.Count > 0;
            }
        }


        public void RunAll()
        {
            const double updateHz = 144.0;
            const double renderHz = 60.0;

            double updateStep = 1.0 / updateHz;
            double renderStep = 1.0 / renderHz;

            foreach (var wrs in windows)
                wrs.window.Load();

            // Shared global timer
            Stopwatch globalTimer = Stopwatch.StartNew();

            double lastUpdateTime = globalTimer.Elapsed.TotalSeconds;
            double updateAccumulator = 0.0;

            // ---------------- UPDATE THREAD ----------------
            Thread updateThread = new Thread(() =>
            {
                double updateStep = 1.0 / updateHz;
                double lastTime = globalTimer.Elapsed.TotalSeconds;

                while (_running && windows.Count > 0)
                {
                    double frameStart = globalTimer.Elapsed.TotalSeconds;
                    double dt = frameStart - lastTime;
                    lastTime = frameStart;

                    if (dt > 0.25)
                        dt = 0.25;

                    // ---- VARIABLE UPDATE ----
                    for (int i = 0; i < windows.Count; i++)
                    {
                        windows[i].window.Update(dt);
                    }

                    // ---- FIXED UPDATE ----
                    for (int i = 0; i < windows.Count; i++)
                    {
                        var wrs = windows[i];

                        wrs.fixedAccumulator += dt;

                        while (wrs.fixedAccumulator >= updateStep)
                        {
                            wrs.window.FixedUpdate(updateStep);
                            wrs.fixedAccumulator -= updateStep;
                        }
                    }

                    // ---- LOCK UPDATE RATE ----
                    double frameEnd = globalTimer.Elapsed.TotalSeconds;
                    double frameDuration = frameEnd - frameStart;
                    double sleepTime = updateStep - frameDuration;

                    if (sleepTime > 0)
                        Thread.Sleep(TimeSpan.FromSeconds(sleepTime));
                }
            });

            //updateThread.Start();

            // ---------------- RENDER / EVENT THREAD ----------------
            double lastRenderTime = globalTimer.Elapsed.TotalSeconds;

            while (_running && windows.Count > 0)
            {
                double frameStart = globalTimer.Elapsed.TotalSeconds;
                double delta = frameStart - lastRenderTime;
                lastRenderTime = frameStart;

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

                    wrs.window.Render(delta);
                }

                double frameEnd = globalTimer.Elapsed.TotalSeconds;
                double frameDuration = frameEnd - frameStart;
                double sleepTime = renderStep - frameDuration;

                if (sleepTime > 0)
                    Thread.Sleep(TimeSpan.FromSeconds(sleepTime));

                _running = windows.Count > 0;
            }

            //updateThread.Join();
        }

    }
}
