using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public class PostProcessor
    {
        List<IPostProcessEffect> effects = new();

        RenderGraph graph = new RenderGraph();

        bool dirty = true;

        RGResource sceneColorResource;
        RGResource finalResource;

        public PostProcessor()
        {
            sceneColorResource = graph.ImportTexture("SceneColor", null);
        }

        public void AddEffect(IPostProcessEffect effect)
        {
            effects.Add(effect);
            dirty = true;
        }

        public void RemoveEffect(IPostProcessEffect effect)
        {
            effects.Remove(effect);
            dirty = true;
        }

        void Rebuild()
        {
            graph.Clear();

            RGResource current = sceneColorResource;

            foreach (var effect in effects)
                effect.Build(graph, current, out current);

            finalResource = current;

            graph.Compile();

            dirty = false;
        }

        public Texture2D Run(Texture2D sceneColor)
        {
            graph.UpdateImported(sceneColorResource, sceneColor);

            if (dirty)
                Rebuild();

            graph.Execute();

            return graph.GetTexture(finalResource);
        }
    }
}
