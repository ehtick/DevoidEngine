// HLSL translation of your GLSL compute shader

    // Constants
#define TYPE_POINT 0u
#define TYPE_SPOT  1u
#define TYPE_DIRECTIONAL 2u

static const uint MAX_LIGHTS_PER_TILE = 100;

    // Structs
struct LightGrid
{
    uint offset;
    uint count;
};

struct PointLight
{
    float4 position;
    float4 color;
    float4 range;
};

struct SpotLight
{
    float4 position; // xyz = pos, w = enabled
    float4 color; // xyz = color, w = intensity
    float4 direction; // xyz = dir, w = range
    float innerCutoff;
    float outerCutoff;
};

struct Cluster
{
    float4 minPoint;
    float4 maxPoint;
};

struct GlobalIndexCount
{
    uint globalLightIndexCount;
};

    // Buffers
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

cbuffer LightCounts : register(b1)
{
    uint numPointLights;
    uint numSpotLights;
    uint numDirectionalLights;
    uint _padding2;
};

cbuffer ClusteredRendererData : register(b2)
{
    float4 groupSize;
};

StructuredBuffer<PointLight> pointLights : register(t0);
StructuredBuffer<SpotLight> spotLights : register(t1);

RWStructuredBuffer<LightGrid> lightGrids : register(u3);
RWStructuredBuffer<Cluster> clusters : register(u4);
RWStructuredBuffer<uint> globalIndexCountBuf : register(u5);
RWStructuredBuffer<uint> globalLightIndexList : register(u6);

    // Groupshared memory
groupshared PointLight sharedPointLights[(16 * 9 * 4) / 2];
groupshared SpotLight sharedSpotLights[(16 * 9 * 4) / 2];

    // Helper functions
float sqDistPointAABB(float3 pos, uint tile)
{
    float sqDist = 0.0;
    Cluster currentCell = clusters[tile];

    for (int i = 0; i < 3; ++i)
    {
        float v = pos[i];
        if (v < currentCell.minPoint[i])
        {
            float d = currentCell.minPoint[i] - v;
            sqDist += d * d;
        }
        if (v > currentCell.maxPoint[i])
        {
            float d = v - currentCell.maxPoint[i];
            sqDist += d * d;
        }
    }
    return sqDist;
}

bool testSphereAABB(uint light, uint tile)
{
    float radius = sharedPointLights[light].range.x;
    float3 center = mul(float4(sharedPointLights[light].position.xyz, 1.0), View).xyz;
    float squaredDistance = sqDistPointAABB(center, tile);
    return squaredDistance <= (radius * radius);
}

bool testSphereAABB_Spot(uint light, uint tile)
{
    float radius = sharedSpotLights[light].direction.w;
    float3 center = mul(float4(sharedSpotLights[light].position.xyz, 1.0), View).xyz;
    float squaredDistance = sqDistPointAABB(center, tile);
    return squaredDistance <= (radius * radius);
}

// Thread group size
[numthreads(16, 9, 4)]
void CSMain(uint3 DTid : SV_DispatchThreadID,
            uint3 GTid : SV_GroupThreadID,
            uint3 Gid : SV_GroupID,
            uint GI : SV_GroupIndex)
{
    if (GI == 0 && Gid.x == 0 && Gid.y == 0 && Gid.z == 0)
        globalIndexCountBuf[0] = 0;

    uint threadCount = 16 * 9 * 4; // match GLSL
    uint lightCount = numPointLights;
    uint numBatches = (lightCount + threadCount - 1) / threadCount;

    // Compute tileIndex exactly like GLSL
    uint tileIndex = GI + threadCount * Gid.z;

    uint visibleLightCount = 0;
    uint visibleLightIndices[MAX_LIGHTS_PER_TILE];

    for (uint batch = 0; batch < numBatches; ++batch)
    {
        uint lightIndex = batch * threadCount + GI;
        if (lightIndex < lightCount)
        {
            sharedPointLights[GI] = pointLights[lightIndex];
        }
        GroupMemoryBarrierWithGroupSync();

        for (uint light = 0; light < threadCount; ++light)
        {
            if (sharedPointLights[light].position.w == 1.0f)
            {
                if (testSphereAABB(light, tileIndex))
                {
                    uint encoded = (TYPE_POINT << 30) | (batch * threadCount + light);
                    visibleLightIndices[visibleLightCount++] = encoded;
                }
            }
        }
    }
    
    // ==== Spot Lights ====
    uint spotCount = numSpotLights;
    uint spotBatches = (spotCount + threadCount - 1) / threadCount;

    for (uint batch = 0; batch < spotBatches; ++batch)
    {
        uint lightIndex = batch * threadCount + GI;

    // Load this thread’s light into shared memory
        if (lightIndex < spotCount)
        {
            sharedSpotLights[GI] = spotLights[lightIndex];
        }
        GroupMemoryBarrierWithGroupSync();

    // Test all the lights in this batch
        for (uint light = 0; light < threadCount; ++light)
        {
            if (light < spotCount && sharedSpotLights[light].position.w == 1.0f)
            {
                if (testSphereAABB_Spot(light, tileIndex))
                {
                    uint encoded = (TYPE_SPOT << 30) | (batch * threadCount + light);
                    visibleLightIndices[visibleLightCount++] = encoded;
                }
            }
        }
    }

    
    uint offset;
    InterlockedAdd(globalIndexCountBuf[0], visibleLightCount, offset);

    for (uint i = 0; i < visibleLightCount; ++i)
    {
        globalLightIndexList[offset + i] = visibleLightIndices[i];
    }

    lightGrids[tileIndex].offset = offset;
    lightGrids[tileIndex].count = visibleLightCount;
}
