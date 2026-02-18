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

        public void RunAll()
        {
            const double updateHz = 165;
            const double renderHz = 165;

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
