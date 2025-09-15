using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public static class RenderAPI
    {
        public static void ProcessCommands(List<IRenderCommand> commands)
        {
            foreach (var command in commands)
            {

            }
            for (int i = 0; i < commands.Count; i++)
            {
                IRenderCommand command = commands[i];

                if (command is SetViewInfoCommand3D)
                {
                    SetViewInfoCommand3D viewInfo3D = (SetViewInfoCommand3D)command;

                    EnginePipeline.SetViewInfoPool.Return(ref viewInfo3D);

                } else if (command is DrawMeshIndexed)
                {
                    DrawMeshIndexed drawMeshIndexed = (DrawMeshIndexed)command;

                    EnginePipeline.DrawMeshIndexedPool.Return(ref drawMeshIndexed);
                }

            }
        }


    }
}
