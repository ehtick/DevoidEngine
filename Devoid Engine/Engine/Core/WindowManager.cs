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
            const double updateHz = 60.0;
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
                while (_running)
                {
                    double now = globalTimer.Elapsed.TotalSeconds;
                    double frameTime = now - lastUpdateTime;
                    lastUpdateTime = now;

                    updateAccumulator += frameTime;

                    // Proper catch-up loop
                    while (updateAccumulator >= updateStep)
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

                        updateAccumulator -= updateStep;
                    }

                    // Light yield to avoid 100% CPU
                    Thread.Sleep(0);
                }
            })
            {
                IsBackground = true,
                Name = "Update_Thread"
            };

            updateThread.Start();

            // ---------------- RENDER / EVENT THREAD ----------------
            double lastRenderTime = globalTimer.Elapsed.TotalSeconds;
            double nextRenderTime = lastRenderTime;

            while (_running && windows.Count > 0)
            {
                double now = globalTimer.Elapsed.TotalSeconds;

                if (now >= nextRenderTime)
                {
                    double delta = now - lastRenderTime;
                    lastRenderTime = now;

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

                    nextRenderTime += renderStep;
                }
                else
                {
                    double sleepTime = nextRenderTime - now;

                    // Hybrid sleep for precision
                    if (sleepTime > 0.002)
                        Thread.Sleep(1);
                    else
                        Thread.SpinWait(50);
                }

                _running = windows.Count > 0;
            }

            updateThread.Join();
        }

    }
}
