using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using System.Numerics;

namespace DevoidEngine.Engine.Utilities
{
    public static class RenderUtil
    {
        public static void ResolveInterpolatedTransforms(List<RenderItem> items, float alpha)
        {
            var snapshotMap = new Dictionary<uint, TransformSnapshot>();
            var worldCache = new Dictionary<uint, Matrix4x4>();

            const uint INVALID = uint.MaxValue;

            // 1. Build snapshot map (deduplicated)
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].useInterpolation)
                    continue;

                var snap = items[i].TransformSnapshot;
                snapshotMap[snap.Id] = snap;
            }

            // 2. Local function for computing world (cached recursion)
            Matrix4x4 Compute(uint id)
            {
                if (worldCache.TryGetValue(id, out var cached))
                    return cached;

                var snap = snapshotMap[id];

                // interpolate local
                var pos = Vector3.Lerp(snap.PrevLocalPosition, snap.CurrLocalPosition, alpha);
                var rot = Quaternion.Slerp(snap.PrevLocalRotation, snap.CurrLocalRotation, alpha);
                var scale = Vector3.Lerp(snap.PrevLocalScale, snap.CurrLocalScale, alpha);

                Matrix4x4 local =
                    Matrix4x4.CreateScale(scale) *
                    Matrix4x4.CreateFromQuaternion(rot) *
                    Matrix4x4.CreateTranslation(pos);

                Matrix4x4 world;

                if (snap.parentId != INVALID)
                    world = local * Compute(snap.parentId);
                else
                    world = local;

                worldCache[id] = world;
                return world;
            }

            // 3. Resolve all items (writes final model directly)
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].useInterpolation)
                    continue;

                var id = items[i].TransformSnapshot.Id;
                var item = items[i];
                item.Model = Compute(id);
                items[i] = item;
            }
        }

    }
}
