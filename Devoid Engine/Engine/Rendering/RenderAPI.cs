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
            Camera activeCamera = null;

            for (int i = 0; i < commands.Count; i++)
            {
                IRenderCommand command = commands[i];

                if (command is SetViewInfoCommand3D)
                {
                    SetViewInfoCommand3D viewInfo3D = (SetViewInfoCommand3D)command;

                    activeCamera = viewInfo3D.camera;

                    EnginePipeline.SetViewInfoPool.Return(ref viewInfo3D);

                } else if (command is DrawMeshIndexed)
                {
                    DrawMeshIndexed drawMeshIndexed = (DrawMeshIndexed)command;

                    Renderer3D.Render(drawMeshIndexed.Mesh, drawMeshIndexed.WorldMatrix);

                    EnginePipeline.DrawMeshIndexedPool.Return(ref drawMeshIndexed);
                } else if (command is Render3DStateCommand)
                {
                    Render3DStateCommand render3DStartCommand = (Render3DStateCommand)command;

                    if (activeCamera == null)
                        throw new InvalidOperationException("Cannot call render commands without active camera");

                    if (render3DStartCommand.state == RendererStateCommnandType.Begin)
                    {
                        Renderer3D.BeginRender(activeCamera);
                    } else
                    {
                        Renderer3D.EndRender();
                    }

                        EnginePipeline.Renderer3DStateCommand.Return(ref render3DStartCommand);
                }

            }
        }


    }
}
