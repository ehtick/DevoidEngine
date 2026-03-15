//using Assimp;
//using DevoidEngine.Engine.Core;
//using DevoidEngine.Engine.Rendering;
//using DevoidEngine.Engine.Utilities;
//using DevoidGPU;
//using System.Numerics;

//namespace DevoidStandaloneLauncher.Utils
//{
//    public static class Importer
//    {
//        public static void LoadModel(string filePath)
//        {
//            Assimp.AssimpContext importer = new AssimpContext();


//            PostProcessSteps importFlags =
//                PostProcessSteps.Triangulate |
//                PostProcessSteps.GenerateNormals |
//                PostProcessSteps.GenerateSmoothNormals |
//                PostProcessSteps.CalculateTangentSpace |
//                PostProcessSteps.GenerateUVCoords |
//                PostProcessSteps.FlipWindingOrder;
//            //PostProcessSteps.JoinIdenticalVertices;


//            Assimp.Scene scene;

//            try
//            {
//                scene = importer.ImportFile(filePath, importFlags);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                return;
//            }

//            var lightLookup = scene.Lights.ToDictionary(l => l.Name, l => l);
//            ProcessNode(scene.RootNode, scene, lightLookup);
//        }

//        static void ProcessNode(Node node, Assimp.Scene scene,
//                                Dictionary<string, Assimp.Light> lightLookup)
//        {
//            LevelSpawnRegistry.Execute(node.Name, node, scene);

//            if (lightLookup.TryGetValue(node.Name, out var light))
//            {
//                LevelSpawnRegistry.ExecuteLight(node.Name, node, light);
//            }

//            foreach (int meshIndex in node.MeshIndices)
//            {
//                var mesh = scene.Meshes[meshIndex];
//                ProcessMesh(mesh);
//            }

//            foreach (var child in node.Children)
//                ProcessNode(child, scene, lightLookup);
//        }

//        static void ProcessMesh(Assimp.Mesh mesh)
//        {

//            List<Vector3> vertices = new();
//            List<Vector3> normals = new();
//            List<Vector2> uvs = new();
//            List<uint> indices = new();

//            Vector3 min = new Vector3(
//                    mesh.Vertices[0].X,
//                    mesh.Vertices[0].Y,
//                    mesh.Vertices[0].Z);
//            Vector3 max = new Vector3(
//                    mesh.Vertices[0].X,
//                    mesh.Vertices[0].Y,
//                    mesh.Vertices[0].Z);

//            for (int i = 0; i < mesh.VertexCount; i++)
//            {
//                vertices.Add(new Vector3(
//                    mesh.Vertices[i].X,
//                    mesh.Vertices[i].Y,
//                    mesh.Vertices[i].Z));

//                if (mesh.HasNormals)
//                {
//                    normals.Add(new Vector3(
//                        mesh.Normals[i].X,
//                        mesh.Normals[i].Y,
//                        mesh.Normals[i].Z));
//                }

//                if (mesh.TextureCoordinateChannelCount > 0)
//                {
//                    var tex = mesh.TextureCoordinateChannels[0][i];
//                    uvs.Add(new Vector2(tex.X, tex.Y));
//                }
//                min = Vector3.Min(min, vertices[i]);
//                max = Vector3.Max(max, vertices[i]);
//            }

//            foreach (var face in mesh.Faces)
//            {
//                foreach (var index in face.Indices)
//                    indices.Add((uint)index);
//            }
//        }

//        public static (Vector3, System.Numerics.Quaternion, Vector3) GetTransform(Node node)
//        {
//            var m = node.Transform;

//            System.Numerics.Matrix4x4 matrix = new System.Numerics.Matrix4x4(
//                m.A1, m.B1, m.C1, m.D1,
//                m.A2, m.B2, m.C2, m.D2,
//                m.A3, m.B3, m.C3, m.D3,
//                m.A4, m.B4, m.C4, m.D4
//            );

//            System.Numerics.Matrix4x4.Decompose(matrix,
//                out Vector3 scale,
//                out System.Numerics.Quaternion rotation,
//                out Vector3 translation);

//            return (translation, rotation, scale);
//        }

//        public static DevoidEngine.Engine.Core.Mesh ConvertMesh(Node node, Assimp.Scene scene)
//        {
//            var assimpMesh = scene.Meshes[node.MeshIndices[0]];

//            Vertex[] vertices = new Vertex[assimpMesh.VertexCount];
//            List<int> indices = new();

//            Vector3 min = new Vector3(
//                assimpMesh.Vertices[0].X,
//                assimpMesh.Vertices[0].Y,
//                assimpMesh.Vertices[0].Z);
//            Vector3 max = new Vector3(
//                assimpMesh.Vertices[0].X,
//                assimpMesh.Vertices[0].Y,
//                assimpMesh.Vertices[0].Z);

//            for (int i = 0; i < assimpMesh.VertexCount; i++)
//            {
//                Vector3 position = new Vector3(
//                    assimpMesh.Vertices[i].X,
//                    assimpMesh.Vertices[i].Y,
//                    assimpMesh.Vertices[i].Z);

//                min = Vector3.Min(min, position);
//                max = Vector3.Max(max, position);

//                Vector3 normal = assimpMesh.HasNormals
//                    ? new Vector3(
//                        assimpMesh.Normals[i].X,
//                        assimpMesh.Normals[i].Y,
//                        assimpMesh.Normals[i].Z)
//                    : Vector3.Zero;

//                Vector2 uv = Vector2.Zero;
//                if (assimpMesh.TextureCoordinateChannelCount > 0)
//                {
//                    var tex = assimpMesh.TextureCoordinateChannels[0][i];

//                    uv = new Vector2(tex.X, tex.Y);

//                    // if only 1 component (rare but happens)
//                    if (assimpMesh.UVComponentCount[0] == 1)
//                        uv = new Vector2(tex.X, 0);
//                }

//                Vector3 tangent = assimpMesh.HasTangentBasis
//                    ? new Vector3(
//                        assimpMesh.Tangents[i].X,
//                        assimpMesh.Tangents[i].Y,
//                        assimpMesh.Tangents[i].Z)
//                    : Vector3.Zero;

//                Vector3 bitangent = assimpMesh.HasTangentBasis
//                    ? new Vector3(
//                        assimpMesh.BiTangents[i].X,
//                        assimpMesh.BiTangents[i].Y,
//                        assimpMesh.BiTangents[i].Z)
//                    : Vector3.Zero;

//                vertices[i] = new DevoidGPU.Vertex(position, normal, uv, tangent, bitangent);
//            }

//            foreach (var face in assimpMesh.Faces)
//            {
//                // Triangulate flag ensures 3 indices
//                indices.Add(face.Indices[0]);
//                indices.Add(face.Indices[1]);
//                indices.Add(face.Indices[2]);
//            }

//            var mesh = new DevoidEngine.Engine.Core.Mesh();
//            mesh.LocalBounds = new BoundingBox(min, max);
//            mesh.SetVertices(vertices, false);
//            mesh.SetIndices(indices.ToArray());

//            return mesh;
//        }

//        public static DevoidEngine.Engine.Core.MaterialInstance ConvertMaterial(Node node, Assimp.Scene scene, string baseModelPath)
//        {
//            Assimp.Material assimpMat = GetMaterial(node, scene);
//            if (assimpMat == null) { return null; }

//            DevoidEngine.Engine.Core.MaterialInstance devoidMaterial = RenderingDefaults.GetMaterial();

//            Texture2D diffuseTexture = GetTexture(assimpMat, Assimp.TextureType.Diffuse, baseModelPath);
//            Texture2D roughnessTexture = GetTexture(assimpMat, Assimp.TextureType.Shininess, baseModelPath);
//            Texture2D normalTexture = GetTexture(assimpMat, Assimp.TextureType.Normals, baseModelPath);

//            if (diffuseTexture != null)
//            {
//                devoidMaterial.SetTexture("MAT_AlbedoMap", diffuseTexture);
//            }

//            if (normalTexture != null)
//            {
//                devoidMaterial.SetInt("useNormalMap", 1);
//                if (assimpMat.HasBumpScaling)
//                {
//                    devoidMaterial.SetFloat("NormalStrength", assimpMat.BumpScaling);
//                }
//                else
//                {
//                    devoidMaterial.SetFloat("NormalStrength", 1);
//                }
//                devoidMaterial.SetTexture("MAT_NormalMap", normalTexture);
//            }

//            if (roughnessTexture != null)
//            {
//                devoidMaterial.SetTexture("MAT_RoughnessMap", roughnessTexture);
//            }

//            if (assimpMat.HasColorDiffuse)
//            {
//                var c = assimpMat.ColorDiffuse;
//                devoidMaterial.SetVector4("Albedo", new Vector4(c.R, c.G, c.B, 1f));
//            }
//            else
//            {
//                devoidMaterial.SetVector4("Albedo", new Vector4(1, 1, 1, 1));
//            }

//            if (assimpMat.HasShininess)
//            {
//                // convert phong shininess → roughness
//                float shininess = assimpMat.ShininessStrength;
//                shininess = 1 - (shininess / 100);
//                devoidMaterial.SetFloat("Roughness", shininess);
//            }
//            else
//            {
//                devoidMaterial.SetFloat("Roughness", 0.5f);
//            }

//            //if (assimpMat.HasColorSpecular)
//            //{
//            //    var c = assimpMat.ColorSpecular;
//            //    devoidMaterial.SetVector3("SpecularColor", new Vector3(c.R, c.G, c.B));

//            //}

//            //MaterialProperty[] mps = assimpMat.GetAllProperties();
//            //foreach (var mp in mps)
//            //{
//            //    Console.WriteLine(mp.Name);
//            //    if (mp.Name == "$mat.shininess")
//            //    {
//            //        Console.WriteLine(mp.GetFloatValue());
//            //    }
//            //}

//            ////devoidMaterial.SetVector3("SpecularColor", new Vector3(1, 1, 1));
//            //if (assimpMat.HasShininess)
//            //    devoidMaterial.SetFloat("Metallic", assimpMat.Shininess/100);
//            //else
//            //    devoidMaterial.SetFloat("Metallic", 0f);

//            devoidMaterial.SetFloat("AO", 1f);

//            Vector3 emissiveColor = Vector3.Zero;
//            float emissiveStrength = 1.0f;

//            if (assimpMat.HasColorEmissive)
//            {
//                var e = assimpMat.ColorEmissive;

//                emissiveColor = new Vector3(e.R, e.G, e.B);

//                // optional: normalize color and extract strength
//                emissiveStrength = MathF.Max(e.R, MathF.Max(e.G, e.B));

//                if (emissiveStrength > 0)
//                    emissiveColor /= emissiveStrength;
//            }

//            devoidMaterial.SetVector3("EmissiveColor", emissiveColor);
//            devoidMaterial.SetFloat("EmissiveStrength", emissiveStrength);

//            if (assimpMat.HasOpacity || assimpMat.BlendMode == Assimp.BlendMode.Additive)
//            {
//                devoidMaterial.BaseMaterial.BlendMode = DevoidGPU.BlendMode.AlphaBlend;
//                Console.WriteLine($"{node.Name}: Glass Found!");
//            }

//            return devoidMaterial;
//        }

//        static void SetWrap(Assimp.TextureWrapMode U, Assimp.TextureWrapMode V, Texture texture)
//        {
//            if (U == Assimp.TextureWrapMode.Wrap && V == Assimp.TextureWrapMode.Wrap)
//            {
//                texture.SetWrapMode(DevoidGPU.TextureWrapMode.Repeat, DevoidGPU.TextureWrapMode.Repeat);
//            }
//            else if (U == Assimp.TextureWrapMode.Wrap && V == Assimp.TextureWrapMode.Clamp)
//            {
//                texture.SetWrapMode(DevoidGPU.TextureWrapMode.Repeat, DevoidGPU.TextureWrapMode.ClampToEdge);
//            }
//            else if (U == Assimp.TextureWrapMode.Clamp && V == Assimp.TextureWrapMode.Wrap)
//            {
//                texture.SetWrapMode(DevoidGPU.TextureWrapMode.ClampToEdge, DevoidGPU.TextureWrapMode.Repeat);
//            }
//        }

//        public static Texture2D GetTexture(Assimp.Material mat, Assimp.TextureType type, string basePath)
//        {
//            if (mat.GetMaterialTextureCount(type) > 0)
//            {
//                mat.GetMaterialTexture(type, 0, out TextureSlot slot);
//                if (!Path.IsPathFullyQualified(basePath))
//                {
//                    return null;
//                }

//                string modelDirectory = Path.GetDirectoryName(basePath);
//                string fullPath = Path.Combine(modelDirectory, slot.FilePath);

//                Texture2D tex;
//                if (type == Assimp.TextureType.Normals)
//                {
//                    tex = Helper.LoadNormalMap(fullPath, TextureFilter.Linear);
//                }
//                else if (type != Assimp.TextureType.Diffuse)
//                {
//                    tex = Helper.LoadImageAsDataTex(fullPath, TextureFilter.Linear);
//                }
//                else
//                {
//                    tex = Helper.LoadImageAsTex(fullPath, TextureFilter.Linear);
//                }
//                SetWrap(slot.WrapModeU, slot.WrapModeV, tex);

//                return tex;
//            }

//            return null;
//        }

//        public static Assimp.Material GetMaterial(Node node, Assimp.Scene scene)
//        {
//            if (node.MeshIndices.Count == 0)
//                return null;

//            var mesh = scene.Meshes[node.MeshIndices[0]];

//            if (mesh.MaterialIndex < 0 || mesh.MaterialIndex >= scene.MaterialCount)
//                return null;

//            return scene.Materials[mesh.MaterialIndex];
//        }

//        public static void ApplyTransform(GameObject go, Node node)
//        {
//            var m = node.Transform;

//            System.Numerics.Matrix4x4 matrix = new System.Numerics.Matrix4x4(
//                m.A1, m.B1, m.C1, m.D1,
//                m.A2, m.B2, m.C2, m.D2,
//                m.A3, m.B3, m.C3, m.D3,
//                m.A4, m.B4, m.C4, m.D4
//            );

//            System.Numerics.Matrix4x4.Decompose(matrix,
//                out Vector3 scale,
//                out System.Numerics.Quaternion rotation,
//                out Vector3 translation);

//            //float temp = scale.Z;
//            //scale.Z = scale.Y;
//            //scale.Y = temp;


//            go.transform.Position = translation;
//            go.transform.Rotation = rotation;
//            go.transform.Scale = scale;
//        }
//    }
//}
