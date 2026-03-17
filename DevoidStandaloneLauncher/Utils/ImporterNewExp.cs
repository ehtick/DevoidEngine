using Assimp;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using DevoidGPU;
using System.Collections.Concurrent;
using System.Numerics;

namespace DevoidStandaloneLauncher.Utils
{
    public static class ImporterExp
    {
        static ConcurrentDictionary<int, DevoidEngine.Engine.Core.Mesh> meshCache = new();
        static ConcurrentDictionary<int, MaterialInstance> materialCache = new();
        static ConcurrentDictionary<string, Texture2D> textureCache = new();
        public static void LoadModel(string filePath)
        {
            Assimp.AssimpContext importer = new AssimpContext();


            PostProcessSteps importFlags =
                PostProcessSteps.Triangulate |
                PostProcessSteps.GenerateNormals |
                PostProcessSteps.GenerateSmoothNormals |
                PostProcessSteps.CalculateTangentSpace |
                PostProcessSteps.GenerateUVCoords |
                PostProcessSteps.FlipUVs |
                PostProcessSteps.FlipWindingOrder;
            //PostProcessSteps.JoinIdenticalVertices;


            Assimp.Scene scene;

            try
            {
                scene = importer.ImportFile(filePath, importFlags);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            var lightLookup = scene.Lights.ToDictionary(l => l.Name, l => l);
            Prebuild(scene, filePath);
            ProcessNode(scene.RootNode, scene, lightLookup);
        }

        static DevoidEngine.Engine.Core.Mesh BuildMesh(int meshIndex, Assimp.Scene scene)
        {
            var assimpMesh = scene.Meshes[meshIndex];

            int vertexCount = assimpMesh.VertexCount;
            Vertex[] vertices = new Vertex[vertexCount];

            bool hasNormals = assimpMesh.HasNormals;
            bool hasUVs = assimpMesh.TextureCoordinateChannelCount > 0;
            bool hasTangents = assimpMesh.HasTangentBasis;
            bool uvIs1D = hasUVs && assimpMesh.UVComponentCount[0] == 1;

            Vector3 min = new(
                assimpMesh.Vertices[0].X,
                assimpMesh.Vertices[0].Y,
                assimpMesh.Vertices[0].Z);

            Vector3 max = min;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 pos = new(
                    assimpMesh.Vertices[i].X,
                    assimpMesh.Vertices[i].Y,
                    assimpMesh.Vertices[i].Z);

                min = Vector3.Min(min, pos);
                max = Vector3.Max(max, pos);

                Vector3 normal = hasNormals
                    ? new Vector3(
                        assimpMesh.Normals[i].X,
                        assimpMesh.Normals[i].Y,
                        assimpMesh.Normals[i].Z)
                    : Vector3.Zero;

                Vector2 uv = Vector2.Zero;
                if (hasUVs)
                {
                    var tex = assimpMesh.TextureCoordinateChannels[0][i];
                    uv = uvIs1D ? new Vector2(tex.X, 0) : new Vector2(tex.X, tex.Y);
                }

                Vector3 tangent = hasTangents
                    ? new Vector3(
                        assimpMesh.Tangents[i].X,
                        assimpMesh.Tangents[i].Y,
                        assimpMesh.Tangents[i].Z)
                    : Vector3.Zero;

                Vector3 bitangent = hasTangents
                    ? new Vector3(
                        assimpMesh.BiTangents[i].X,
                        assimpMesh.BiTangents[i].Y,
                        assimpMesh.BiTangents[i].Z)
                    : Vector3.Zero;

                vertices[i] = new Vertex(pos, normal, uv, tangent, bitangent);
            }

            int[] indices = new int[assimpMesh.FaceCount * 3];
            int idx = 0;

            foreach (var face in assimpMesh.Faces)
            {
                indices[idx++] = face.Indices[0];
                indices[idx++] = face.Indices[1];
                indices[idx++] = face.Indices[2];
            }

            var mesh = new DevoidEngine.Engine.Core.Mesh();
            mesh.LocalBounds = new DevoidEngine.Engine.Utilities.BoundingBox(min, max);
            mesh.SetVertices(vertices, false);
            mesh.SetIndices(indices);

            return mesh;
        }

        static MaterialInstance BuildMaterial(int matIndex, Assimp.Scene scene, string basePath)
        {
            var assimpMat = scene.Materials[matIndex];

            var mat = RenderingDefaults.GetMaterial();

            Texture2D diffuse = GetTexture(assimpMat, Assimp.TextureType.Diffuse, basePath);
            Texture2D normal = GetTexture(assimpMat, Assimp.TextureType.Normals, basePath);
            Texture2D roughness = GetTexture(assimpMat, Assimp.TextureType.Shininess, basePath);
            Texture2D emissive = GetTexture(assimpMat, Assimp.TextureType.Emissive, basePath);

            if (diffuse != null)
                mat.SetTexture("MAT_AlbedoMap", diffuse);

            if (normal != null)
            {
                mat.SetInt("useNormalMap", 1);
                mat.SetFloat("NormalStrength", assimpMat.HasBumpScaling ? assimpMat.BumpScaling : 1);
                mat.SetTexture("MAT_NormalMap", normal);
            }

            if (roughness != null)
                mat.SetTexture("MAT_RoughnessMap", roughness);

            if (emissive != null)
                mat.SetTexture("MAT_EmissiveMap", emissive);

            if (assimpMat.HasColorDiffuse)
                mat.SetVector4("Albedo", assimpMat.ColorDiffuse);
            else
                mat.SetVector4("Albedo", new Vector4(1, 1, 1, 1));

            mat.SetFloat("Roughness",
                assimpMat.GetProperty("$mat.roughnessFactor,0,0")?.GetFloatValue() ?? 0.5f);

            mat.SetFloat("Metallic", assimpMat.HasReflectivity ? assimpMat.Reflectivity : 0f);
            mat.SetFloat("AO", 1f);

            if (assimpMat.HasColorEmissive)
            {
                var e = assimpMat.ColorEmissive;
                float strength = MathF.Max(e.X, MathF.Max(e.Y, e.Z));
                Vector3 color = strength > 0 ? e.AsVector3() / strength : Vector3.Zero;

                mat.SetVector3("EmissiveColor", color);
                mat.SetFloat("EmissiveStrength", strength);
            }

            return mat;
        }

        static void Prebuild(Assimp.Scene scene, string basePath)
        {
            Parallel.For(0, scene.MeshCount, i =>
            {
                var assimpMesh = scene.Meshes[i];

                // Build mesh if not cached
                meshCache.GetOrAdd(i, _ =>
                {
                    return BuildMesh(i, scene);
                });

                // Build material if valid
                int matIndex = assimpMesh.MaterialIndex;
                if (matIndex >= 0 && matIndex < scene.MaterialCount)
                {
                    materialCache.GetOrAdd(matIndex, _ =>
                    {
                        return BuildMaterial(matIndex, scene, basePath);
                    });
                }
            });
        }

        static void ProcessNode(Node node, Assimp.Scene scene,
                                Dictionary<string, Assimp.Light> lightLookup)
        {
            LevelSpawnRegistry.Execute(node.Name, node, scene);

            if (lightLookup.TryGetValue(node.Name, out var light))
            {
                LevelSpawnRegistry.ExecuteLight(node.Name, node, light);
            }

            foreach (var child in node.Children)
                ProcessNode(child, scene, lightLookup);
        }
        public static (Vector3, Quaternion, Vector3) GetTransform(Node node)
        {
            Matrix4x4 matrix = Matrix4x4.Transpose(node.Transform);

            Matrix4x4.Decompose(matrix,
                out Vector3 scale,
                out Quaternion rotation,
                out Vector3 translation);

            return (translation, rotation, scale);
        }

        public static DevoidEngine.Engine.Core.Mesh ConvertMesh(Node node, Assimp.Scene scene)
        {
            int meshIndex = node.MeshIndices[0];
            return meshCache[meshIndex];
        }

        public static MaterialInstance ConvertMaterial(Node node, Assimp.Scene scene, string basePath)
        {
            int matIndex = scene.Meshes[node.MeshIndices[0]].MaterialIndex;
            return materialCache.TryGetValue(matIndex, out var mat) ? mat : null;
        }

        public static DevoidEngine.Engine.Core.Mesh ConvertMesh1(Node node, Assimp.Scene scene)
        {
            int meshIndex = node.MeshIndices[0];

            if (meshCache.TryGetValue(meshIndex, out var cachedMesh))
                return cachedMesh;

            var assimpMesh = scene.Meshes[meshIndex];

            if (assimpMesh.VertexCount == 0 || assimpMesh.FaceCount == 0)
                return null;

            int vertexCount = assimpMesh.VertexCount;
            Vertex[] vertices = new Vertex[vertexCount];
            int indexCount = assimpMesh.FaceCount * 3;
            int[] indices = new int[indexCount];

            bool hasNormals = assimpMesh.HasNormals;
            bool hasUVs = assimpMesh.TextureCoordinateChannelCount > 0;
            bool uvIs1D = hasUVs && assimpMesh.UVComponentCount[0] == 1;
            bool hasTangents = assimpMesh.HasTangentBasis;

            Vector3 min = new Vector3(
                assimpMesh.Vertices[0].X,
                assimpMesh.Vertices[0].Y,
                assimpMesh.Vertices[0].Z);
            Vector3 max = new Vector3(
                assimpMesh.Vertices[0].X,
                assimpMesh.Vertices[0].Y,
                assimpMesh.Vertices[0].Z);

            for (int i = 0; i < assimpMesh.VertexCount; i++)
            {
                Vector3 position = new Vector3(
                    assimpMesh.Vertices[i].X,
                    assimpMesh.Vertices[i].Y,
                    assimpMesh.Vertices[i].Z);

                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);

                Vector3 normal = hasNormals
                    ? new Vector3(
                        assimpMesh.Normals[i].X,
                        assimpMesh.Normals[i].Y,
                        assimpMesh.Normals[i].Z)
                    : Vector3.Zero;

                Vector2 uv = Vector2.Zero;
                if (hasUVs)
                {
                    var tex = assimpMesh.TextureCoordinateChannels[0][i];

                    uv = uvIs1D ? new Vector2(tex.X, 0) : new Vector2(tex.X, tex.Y);
                }

                Vector3 tangent = hasTangents
                    ? new Vector3(
                        assimpMesh.Tangents[i].X,
                        assimpMesh.Tangents[i].Y,
                        assimpMesh.Tangents[i].Z)
                    : Vector3.Zero;

                Vector3 bitangent = hasTangents
                    ? new Vector3(
                        assimpMesh.BiTangents[i].X,
                        assimpMesh.BiTangents[i].Y,
                        assimpMesh.BiTangents[i].Z)
                    : Vector3.Zero;

                vertices[i] = new DevoidGPU.Vertex(position, normal, uv, tangent, bitangent);
            }

            int idx = 0;
            foreach (var face in assimpMesh.Faces)
            {
                indices[idx++] = face.Indices[0];
                indices[idx++] = face.Indices[1];
                indices[idx++] = face.Indices[2];
            }

            var mesh = new DevoidEngine.Engine.Core.Mesh();
            mesh.LocalBounds = new DevoidEngine.Engine.Utilities.BoundingBox(min, max);
            mesh.SetVertices(vertices, false);
            mesh.SetIndices(indices);

            meshCache[meshIndex] = mesh;
            return mesh;
        }

        public static DevoidEngine.Engine.Core.MaterialInstance ConvertMaterial1(Node node, Assimp.Scene scene, string baseModelPath)
        {
            var assimpMat = GetMaterial(node, scene);
            if (assimpMat == null) return null;

            int meshIndex = node.MeshIndices[0];
            var mesh = scene.Meshes[meshIndex];
            int matIndex = mesh.MaterialIndex;

            if (materialCache.TryGetValue(matIndex, out var cachedMat))
                return cachedMat;

            DevoidEngine.Engine.Core.MaterialInstance devoidMaterial = RenderingDefaults.GetMaterial();

            MaterialProperty roughnessProperty = assimpMat.GetProperty("$mat.roughnessFactor,0,0");

            Texture2D diffuseTexture = GetTexture(assimpMat, Assimp.TextureType.Diffuse, baseModelPath);
            Texture2D roughnessTexture = GetTexture(assimpMat, Assimp.TextureType.Shininess, baseModelPath);
            Texture2D normalTexture = GetTexture(assimpMat, Assimp.TextureType.Normals, baseModelPath);
            Texture2D emissiveTexture = GetTexture(assimpMat, Assimp.TextureType.Emissive, baseModelPath);

            if (diffuseTexture != null)
            {
                devoidMaterial.SetTexture("MAT_AlbedoMap", diffuseTexture);
            }

            if (normalTexture != null)
            {
                devoidMaterial.SetInt("useNormalMap", 1);
                if (assimpMat.HasBumpScaling)
                {
                    devoidMaterial.SetFloat("NormalStrength", assimpMat.BumpScaling);
                }
                else
                {
                    devoidMaterial.SetFloat("NormalStrength", 1);
                }
                devoidMaterial.SetTexture("MAT_NormalMap", normalTexture);
            }

            if (roughnessTexture != null)
            {
                devoidMaterial.SetTexture("MAT_RoughnessMap", roughnessTexture);
            }

            if (emissiveTexture != null)
            {
                devoidMaterial.SetTexture("MAT_EmissiveMap", emissiveTexture);
                Console.WriteLine("Emissive Map Loaded");
            }

            if (assimpMat.HasColorDiffuse)
            {
                var c = assimpMat.ColorDiffuse;
                devoidMaterial.SetVector4("Albedo", c);
            }
            else
            {
                devoidMaterial.SetVector4("Albedo", new Vector4(1, 1, 1, 1));
            }

            //if (node.Name == "Glassy:Model")
            //{
            //    MaterialProperty[] mps = assimpMat.GetAllProperties();
            //    foreach (var mp in mps)
            //    {
            //        Console.WriteLine(mp.Name);
            //        Console.WriteLine(mp.FullyQualifiedName);
            //        Console.WriteLine(mp.GetFloatValue());
            //    }
            //}

            if (roughnessProperty != null)
            {
                float roughness = assimpMat.GetProperty("$mat.roughnessFactor,0,0").GetFloatValue();
                devoidMaterial.SetFloat("Roughness", roughness);
            }
            else
            {
                devoidMaterial.SetFloat("Roughness", 0.5f);
            }

            //Console.WriteLine(assimpMat.BumpScaling);

            //if (assimpMat.HasColorSpecular)
            //{
            //    var c = assimpMat.ColorSpecular;
            //    devoidMaterial.SetVector3("SpecularColor", new Vector3(c.R, c.G, c.B));

            //}

            //MaterialProperty[] mps = assimpMat.GetAllProperties();
            //if (node.Name == "Glassy:Model")
            //{
            //    Console.WriteLine(node.Name);
            //    foreach (var mp in mps)
            //    {
            //        Console.WriteLine(mp.Name);
            //        Console.WriteLine(mp.GetFloatValue());
            //    }
            //}

            ////devoidMaterial.SetVector3("SpecularColor", new Vector3(1, 1, 1));
            if (assimpMat.HasReflectivity)
                devoidMaterial.SetFloat("Metallic", assimpMat.Reflectivity);
            else
                devoidMaterial.SetFloat("Metallic", 0f);

            devoidMaterial.SetFloat("AO", 1f);

            Vector3 emissiveColor = Vector3.Zero;
            float emissiveStrength = 1.0f;

            if (assimpMat.HasColorEmissive)
            {
                var e = assimpMat.ColorEmissive;

                emissiveColor = e.AsVector3();

                // optional: normalize color and extract strength
                emissiveStrength = MathF.Max(e.X, MathF.Max(e.Y, e.Z));

                if (emissiveStrength > 0)
                    emissiveColor /= emissiveStrength;
            }

            devoidMaterial.SetVector3("EmissiveColor", emissiveColor);
            devoidMaterial.SetFloat("EmissiveStrength", emissiveStrength);

            //if (assimpMat.HasOpacity)
            //{
            //    Console.WriteLine($"{node.Name} Possibly Glass: {assimpMat.Opacity}");
            //    //devoidMaterial.BaseMaterial.BlendMode = DevoidGPU.BlendMode.AlphaBlend;
            //}
            materialCache[matIndex] = devoidMaterial;
            return devoidMaterial;
        }

        static void SetWrap(Assimp.TextureWrapMode U, Assimp.TextureWrapMode V, Texture texture)
        {
            if (U == Assimp.TextureWrapMode.Wrap && V == Assimp.TextureWrapMode.Wrap)
            {
                texture.SetWrapMode(DevoidGPU.TextureWrapMode.Repeat, DevoidGPU.TextureWrapMode.Repeat);
            }
            else if (U == Assimp.TextureWrapMode.Wrap && V == Assimp.TextureWrapMode.Clamp)
            {
                texture.SetWrapMode(DevoidGPU.TextureWrapMode.Repeat, DevoidGPU.TextureWrapMode.ClampToEdge);
            }
            else if (U == Assimp.TextureWrapMode.Clamp && V == Assimp.TextureWrapMode.Wrap)
            {
                texture.SetWrapMode(DevoidGPU.TextureWrapMode.ClampToEdge, DevoidGPU.TextureWrapMode.Repeat);
            }
        }

        public static Texture2D GetTexture(Assimp.Material mat, Assimp.TextureType type, string basePath)
        {
            if (mat.GetMaterialTextureCount(type) == 0)
                return null;

            mat.GetMaterialTexture(type, 0, out TextureSlot slot);

            if (!Path.IsPathFullyQualified(basePath))
                return null;

            string modelDirectory = Path.GetDirectoryName(basePath);
            string fullPath = Path.Combine(modelDirectory, slot.FilePath);

            // Normalize path (important for cache hits)
            fullPath = Path.GetFullPath(fullPath);

            // 🔥 CACHE CHECK
            if (textureCache.TryGetValue(fullPath, out var cached))
                return cached;

            Texture2D tex;

            if (type == Assimp.TextureType.Normals)
            {
                tex = Helper.LoadNormalMap(fullPath, TextureFilter.Linear);
            }
            else if (type != Assimp.TextureType.Diffuse)
            {
                tex = Helper.LoadImageAsDataTex(fullPath, TextureFilter.Linear);
            }
            else
            {
                tex = Helper.LoadImageAsTex(fullPath, TextureFilter.Linear);
            }

            SetWrap(slot.WrapModeU, slot.WrapModeV, tex);

            // 🔥 STORE IN CACHE
            textureCache[fullPath] = tex;

            return tex;
        }

        public static Assimp.Material GetMaterial(Node node, Assimp.Scene scene)
        {
            if (node.MeshIndices.Count == 0)
                return null;

            var mesh = scene.Meshes[node.MeshIndices[0]];

            if (mesh.MaterialIndex < 0 || mesh.MaterialIndex >= scene.MaterialCount)
                return null;

            return scene.Materials[mesh.MaterialIndex];
        }

        public static void ApplyTransform(GameObject go, Node node)
        {
            Matrix4x4 matrix = Matrix4x4.Transpose(node.Transform);

            Matrix4x4.Decompose(matrix,
                out Vector3 scale,
                out Quaternion rotation,
                out Vector3 translation);

            go.transform.Position = translation;
            go.transform.Rotation = rotation;
            go.transform.Scale = scale;
        }
    }
}
