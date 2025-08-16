using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DXUtils
    {
        public static InputElement[] CreateInputElements(VertexInfo vertexInfo)
        {
            InputElement[] InputLayoutElements = new InputElement[vertexInfo.VertexAttributes.Length];
            for (int i = 0; i < InputLayoutElements.Length; i++)
            {
                VertexAttribute attribute = vertexInfo.VertexAttributes[i];

                InputLayoutElements[i] = new InputElement()
                {
                    SemanticName = attribute.Name,
                    SemanticIndex = attribute.Index,
                    Format = MapDXGIComponentCountToFormat(attribute),
                    AlignedByteOffset = attribute.Offset,
                    Slot = 0
                };

            }

            return InputLayoutElements;
        }

        public static Format MapDXGIComponentCountToFormat(VertexAttribute attr)
        {
            return attr.ComponentCount switch
            {
                1 => Format.R32_Float,
                2 => Format.R32G32_Float,
                3 => Format.R32G32B32_Float,
                4 => Format.R32G32B32A32_Float,
                _ => throw new ArgumentException("Unsupported component count")
            };
        }


    }
}
