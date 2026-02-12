using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

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
                double lastTime = timer.Elapsed.TotalSeconds;

                while (_running)
                {
                    double now = timer.Elapsed.TotalSeconds;
                    double delta = now - lastTime;
                    lastTime = now;

                    for (int i = 0; i < windows.Count; i++)
                    {
                        var wrs = windows[i];
                        wrs.fixedAccumulator += delta;

                        wrs.window.Update(delta);

                        while (wrs.fixedAccumulator >= updateStep)
                        {
                            wrs.window.FixedUpdate(updateStep);
                            wrs.fixedAccumulator -= updateStep;
                        }
                    }

                    // Sleep until next update tick
                    double frameTime = timer.Elapsed.TotalSeconds - now;
                    double sleep = updateStep - frameTime;

                    if (sleep > 0)
                        Thread.Sleep((int)(sleep * 1000));
                    else
                        Thread.Yield();
                }
            })
            {
                IsBackground = true,
                Name = "Update_Thread"
            };

            updateThread.Start();

            // ---------------- RENDER / EVENT THREAD ----------------
            Stopwatch renderTimer = Stopwatch.StartNew();
            double lastRender = renderTimer.Elapsed.TotalSeconds;

            while (_running && windows.Count > 0)
            {
                double now = renderTimer.Elapsed.TotalSeconds;
                double delta = now - lastRender;
                lastRender = now;

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

                // Sleep until next render tick
                double frameTime = renderTimer.Elapsed.TotalSeconds - now;
                double sleep = renderStep - frameTime;

                if (sleep > 0)
                    Thread.Sleep((int)(sleep * 1000));
                else
                    Thread.Yield();

                _running = windows.Count > 0;
            }

            updateThread.Join();
        }
    }
}
