//=====================================================
// HLSL Compute Shader (converted from GLSL)
//=====================================================

struct Cluster
{
    float4 minPoint;
    float4 maxPoint;
};

// ----------------------------------------------------
// Buffers
// ----------------------------------------------------

// Cluster buffer (like GLSL SSBO)
RWStructuredBuffer<Cluster> clusters : register(u4); // binding = 4 in GLSL

// Screen / view data buffer
struct ScreenViewData
{
    float4x4 inverseProjectionMatrix; // matches mat4
    float4 tileSizes;
    uint screenWidth;
    uint screenHeight;
    float sliceScaling;
    float sliceBias;
};

StructuredBuffer<ScreenViewData> screenViewData : register(t7); // binding = 7
// (Or make this a cbuffer with matching fields if it’s constant for all threads)

// ----------------------------------------------------
// Constants
// ----------------------------------------------------
cbuffer CameraData : register(b0)
{
    float4x4 View;
    float4x4 Projection;
    float3 Position;
    float _padding;
    float zNear;
    float zFar;
    float2 _padding1;
};

cbuffer ClusteredRendererData : register(b1)
{
    float4 groupSize;
};

float3 lineIntersectionToZPlane(float3 A, float3 B, float zDistance);
float4 clipToView(float4 clip);
float4 screen2View(float4 screen);
float4x4 inverse(float4x4 m);

// ----------------------------------------------------
// Thread group size — matches layout(local_size_x =1,...)
// ----------------------------------------------------
[numthreads(1, 1, 1)]
void CSMain(uint3 groupID : SV_GroupID,
          uint3 dispatchThreadID : SV_DispatchThreadID,
          uint3 groupThreadID : SV_GroupThreadID,
          uint groupIndex : SV_GroupIndex)
{
    float3 eyePos = float3(0.0, 0.0, 0.0);

    uint tileSizePx = screenViewData[0].tileSizes.w;

    // Compute tile index exactly as GLSL did
    uint tileIndex =
        groupID.x +
        groupID.y * groupSize.x + // you’ll need to pass total counts
        groupID.z * (groupSize.x * groupSize.y);

    // Screen space coordinates for min/max corners
    float4 maxPoint_sS = float4(
        (float2(groupID.x + 1, groupID.y + 1) * tileSizePx),
        -1.0, 1.0); // Top Right

    float4 minPoint_sS = float4(
        (float2(groupID.xy) * tileSizePx),
        -1.0, 1.0); // Bottom Left

    //Pass min and max to view space
    float3 maxPoint_vS = screen2View(maxPoint_sS).xyz;
    float3 minPoint_vS = screen2View(minPoint_sS).xyz;

    //Near and far values of the cluster in view space
    float tileNear = -zNear * pow(zFar / zNear,
                    (float) groupID.z / (float) groupSize.z);
    float tileFar = -zNear * pow(zFar / zNear,
                    (float) (groupID.z + 1) / (float) groupSize.z);

    //Find intersection points with Z planes
    float3 minPointNear = lineIntersectionToZPlane(eyePos, minPoint_vS, tileNear);
    float3 minPointFar = lineIntersectionToZPlane(eyePos, minPoint_vS, tileFar);
    float3 maxPointNear = lineIntersectionToZPlane(eyePos, maxPoint_vS, tileNear);
    float3 maxPointFar = lineIntersectionToZPlane(eyePos, maxPoint_vS, tileFar);

    float3 minPointAABB = min(min(minPointNear, minPointFar),
                              min(maxPointNear, maxPointFar));
    float3 maxPointAABB = max(max(minPointNear, minPointFar),
                              max(maxPointNear, maxPointFar));

    //clusters[tileIndex].minPoint = float4(420.0, 420.0, 420.0, 420.0);
    //clusters[tileIndex].maxPoint = float4(69.0, 69.0, 69.0, 69.0);
    
    //clusters[tileIndex].minPoint = mul(inverse(View), float4(minPointAABB, 0.0));
    //clusters[tileIndex].maxPoint = mul(inverse(View), float4(maxPointAABB, 0.0));
    
    clusters[tileIndex].minPoint = float4(minPointAABB, 0.0);
    clusters[tileIndex].maxPoint = float4(maxPointAABB, 0.0);

}

// ----------------------------------------------------
// Helper Functions
// ----------------------------------------------------

// Intersection of line with a Z plane
float3 lineIntersectionToZPlane(float3 A, float3 B, float zDistance)
{
    float3 normal = float3(0.0, 0.0, 1.0);
    float3 ab = B - A;
    float t = (zDistance - dot(normal, A)) / dot(normal, ab);
    return A + t * ab;
}

// Clip to View
float4 clipToView(float4 clip)
{
    float4 view = mul(screenViewData[0].inverseProjectionMatrix, clip);
    view /= view.w;
    return view;
}

// Screen to View
float4 screen2View(float4 screen)
{
    float2 texCoord = screen.xy / float2(screenViewData[0].screenWidth,
                                         screenViewData[0].screenHeight);

    float4 clip = float4(texCoord * 2.0 - 1.0, screen.z, screen.w);
    return clipToView(clip);
}


float4x4 inverse(float4x4 m)
{
    float n11 = m[0][0], n12 = m[1][0], n13 = m[2][0], n14 = m[3][0];
    float n21 = m[0][1], n22 = m[1][1], n23 = m[2][1], n24 = m[3][1];
    float n31 = m[0][2], n32 = m[1][2], n33 = m[2][2], n34 = m[3][2];
    float n41 = m[0][3], n42 = m[1][3], n43 = m[2][3], n44 = m[3][3];

    float t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
    float t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
    float t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
    float t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

    float det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;
    float idet = 1.0f / det;

    float4x4 ret;

    ret[0][0] = t11 * idet;
    ret[0][1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * idet;
    ret[0][2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * idet;
    ret[0][3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * idet;

    ret[1][0] = t12 * idet;
    ret[1][1] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * idet;
    ret[1][2] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * idet;
    ret[1][3] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * idet;

    ret[2][0] = t13 * idet;
    ret[2][1] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * idet;
    ret[2][2] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * idet;
    ret[2][3] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * idet;

    ret[3][0] = t14 * idet;
    ret[3][1] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * idet;
    ret[3][2] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * idet;
    ret[3][3] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * idet;

    return ret;
}