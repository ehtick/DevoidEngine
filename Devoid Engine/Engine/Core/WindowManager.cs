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
            const double updateHz = 120;
            const double renderHz = 120;

            double updateStep = 1.0 / updateHz;
            double renderStep = 1.0 / renderHz;

            foreach (var wrs in windows)
                wrs.window.Load();

            // ---------------- UPDATE THREAD ----------------
            Thread updateThread = new Thread(() =>
            {
                Stopwatch timer = Stopwatch.StartNew();

                double nextTick = timer.Elapsed.TotalSeconds;
                double step = 1.0 / updateHz;

                while (_running)
                {
                    double now = timer.Elapsed.TotalSeconds;

                    if (now >= nextTick)
                    {
                        double delta = step;

                        for (int i = 0; i < windows.Count; i++)
                        {
                            var wrs = windows[i];

                            wrs.window.Update(delta);

                            wrs.fixedAccumulator += delta;
                            while (wrs.fixedAccumulator >= step)
                            {
                                wrs.window.FixedUpdate(step);
                                wrs.fixedAccumulator -= step;
                            }
                        }

                        nextTick += step;
                    }
                    else
                    {
                        double sleepTime = nextTick - now;
                        if (sleepTime > 0)
                            Thread.Sleep(TimeSpan.FromSeconds(sleepTime));
                    }
                }
            })
            {
                IsBackground = true,
                Name = "Update_Thread"
            };


            updateThread.Start();

            // ---------------- RENDER / EVENT THREAD ----------------
            Stopwatch renderTimer = Stopwatch.StartNew();
            double nextRender = renderTimer.Elapsed.TotalSeconds;

            while (_running && windows.Count > 0)
            {
                double now = renderTimer.Elapsed.TotalSeconds;

                if (now >= nextRender)
                {
                    double delta = renderStep;

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

                    nextRender += renderStep;
                }
                else
                {
                    double sleepTime = nextRender - now;
                    if (sleepTime > 0)
                        Thread.Sleep(TimeSpan.FromSeconds(sleepTime));
                }

                _running = windows.Count > 0;
            }

            updateThread.Join();
        }
    }
}
