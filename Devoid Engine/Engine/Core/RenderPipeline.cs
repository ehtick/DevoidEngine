using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public static class RenderPipeline
    {
        public static event Action OnBeginCameraRender;
        public static event Action OnEndCameraRender;

        public static void BeginCameraRender()
        {
            OnBeginCameraRender?.Invoke();
        }

        public static void EndCameraRender()
        {
            OnEndCameraRender?.Invoke();
        }

    }
}
