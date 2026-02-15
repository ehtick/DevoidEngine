using DevoidEngine.Engine.Core;

namespace DevoidEngine.Engine.Rendering
{
    public static class MaterialManager
    {
        public static Dictionary<Guid, Material> Materials = new();

        public static Dictionary<Guid, List<MaterialInstance>> MaterialInstances = new();

        public static Material RegisterMaterial(Material material)
        {
            if (material.Id == Guid.Empty)
                material.Id = Guid.NewGuid();

            Materials[material.Id] = material;
            MaterialInstances[material.Id] = new List<MaterialInstance>();

            return material;
        }

        public static Material GetMaterial(Guid id)
        {
            return Materials.TryGetValue(id, out var mat)
                ? mat
                : null;
        }

        public static MaterialInstance CreateInstance(Material material)
        {
            if (!Materials.ContainsKey(material.Id))
                RegisterMaterial(material);

            var instance = new MaterialInstance(material);

            MaterialInstances[material.Id].Add(instance);

            return instance;
        }
    }
}
