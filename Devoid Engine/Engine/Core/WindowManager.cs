using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public struct WindowRunState
    {
        public Window window;
        public double accumulated;

    }
    public class WindowManager
    {
        private readonly List<WindowRunState> windows = new List<WindowRunState>();
        private bool _running = true;

        public static WindowManager Instance { get; } = new WindowManager();


        public void RegisterWindow(Window window)
        {
            windows.Add(new WindowRunState() {
                window = window
            });
        }

        public void RunAll()
        {

            foreach (var wrs in windows)
                wrs.window.Load();

            Thread updateThread = new Thread(() =>
            {
                Stopwatch timer = Stopwatch.StartNew();
                double lastTime = timer.Elapsed.TotalSeconds;
                const double fixedUpdate = 1.0 / 60.0;

                while (_running)
                {
                    double now = timer.Elapsed.TotalSeconds;
                    double delta = now - lastTime;
                    lastTime = now;

                    for (int i = 0; i < windows.Count; i++)
                    {
                        var wrs = windows[i];
                        wrs.accumulated += delta;
                        wrs.window.Update(delta);

                        while (wrs.accumulated >= fixedUpdate)
                        {
                            wrs.window.FixedUpdate(fixedUpdate);
                            wrs.accumulated -= fixedUpdate;
                        }
                    }
                }



            })
            {
                IsBackground = true,
            };

            updateThread.Start();

            Stopwatch timer = Stopwatch.StartNew();
            double lastRender = timer.Elapsed.TotalSeconds;

            while (_running && windows.Count > 0)
            {
                double currentTime = timer.Elapsed.TotalSeconds;

                double deltaTime = currentTime - lastRender;
                lastRender = currentTime;

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

                    // Render
                    wrs.window.Render();
                }
                _running = windows.Count > 0;
            }

            updateThread.Join();
        }

        public void Shutdown()
        {

        }
    }

}
