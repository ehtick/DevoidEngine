using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public class MaterialHelper
    {
        private static bool TryResolveInt(MaterialInstance i, string name, out int value)
            => i.PropertiesIntOverride.TryGetValue(name, out value)
            || i.BaseMaterial.PropertiesInt.TryGetValue(name, out value);
        private static bool TryResolveFloat(MaterialInstance i, string name, out float value)
            => i.PropertiesFloatOverride.TryGetValue(name, out value)
            || i.BaseMaterial.PropertiesFloat.TryGetValue(name, out value);

        private static bool TryResolveVec4(MaterialInstance i, string name, out Vector4 value)
            => i.PropertiesVec4Override.TryGetValue(name, out value)
            || i.BaseMaterial.PropertiesVec4.TryGetValue(name, out value);

        private static bool TryResolveVec3(MaterialInstance i, string name, out Vector3 value)
            => i.PropertiesVec3Override.TryGetValue(name, out value)
            || i.BaseMaterial.PropertiesVec3.TryGetValue(name, out value);

        private static bool TryResolveMat4(MaterialInstance i, string name, out Matrix4x4 value)
            => i.PropertiesMat4Override.TryGetValue(name, out value)
            || i.BaseMaterial.PropertiesMat4.TryGetValue(name, out value);

        public static void Update(MaterialInstance instance)
        {
            if (!instance.GPUData.Dirty)
                return;

            Material material = instance.BaseMaterial;
            MaterialLayout layout = material.MaterialLayout;

            Span<byte> buffer = stackalloc byte[layout.bufferSize];

            foreach (var prop in layout.Properties)
            {
                switch (prop.Type)
                {
                    case ShaderPropertyType.Int:
                        if (TryResolveInt(instance, prop.Name, out int i))
                            UniformBufferHelper.WriteInt(buffer, prop.Offset, i);
                        break;
                    case ShaderPropertyType.Float:
                        if (TryResolveFloat(instance, prop.Name, out float f))
                            UniformBufferHelper.WriteFloat(buffer, prop.Offset, f);
                        break;

                    case ShaderPropertyType.Vector4:
                        if (TryResolveVec4(instance, prop.Name, out Vector4 v4))
                            UniformBufferHelper.WriteVector4(buffer, prop.Offset, v4);
                        break;

                    case ShaderPropertyType.Vector3:
                        if (TryResolveVec3(instance, prop.Name, out Vector3 v3))
                            UniformBufferHelper.WriteVector3(buffer, prop.Offset, v3);
                        break;

                    case ShaderPropertyType.Matrix4:
                        if (TryResolveMat4(instance, prop.Name, out Matrix4x4 m))
                            UniformBufferHelper.WriteMatrix4x4(buffer, prop.Offset, m);
                        break;
                }
            }

            instance.GPUData.UniformBuffer.SetData(buffer);
            instance.GPUData.Dirty = false;
        }
    }
}
