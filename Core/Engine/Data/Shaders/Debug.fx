#include "Globals.fx"

// ---------------------------------
struct VS_INPUT
{
    float4 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
   
#if INSTANCED == 1
    uint InstanceID : SV_InstanceID;
#endif
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    
#if RENDERNORMALS
    float4 Depth : COLOR;
#endif
};

// ---------------------------------
// Vertex Shader
// ---------------------------------
PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output = (PS_INPUT)0;
    
    matrix worldMatrix = World;
    
#if INSTANCED == 1
    worldMatrix = Instances[input.InstanceID].World;
#endif
    
    input.Position.w = 1.0f;
    
    output.Position = mul(worldMatrix, input.Position);
    output.Position = mul(View, output.Position);
    output.Position = mul(Projection, output.Position);
    output.TextureCoordinates = input.TextureCoordinates;

#if RENDERNORMALS
    float4 normal = mul(worldMatrix, float4(input.Normal, 0));
    normal = mul(View, normal);
    
    output.Depth.xyz = normal.xyz * 0.5f + 0.5f;
    output.Depth.w = output.Position.w / 25.0f;
#endif

    return output;
}

// ---------------------------------
// Pixel Shader
// ---------------------------------
float4 PS(PS_INPUT input) : SV_Target
{	
#if RENDERNORMALS
    return input.Depth;
#endif

#if RENDERDEPTH
    float4 color = DiffuseMap.Sample(DiffuseSampler, input.TextureCoordinates);
    float z = BringDepthIntoView(color.r, 0.999f, 0.99999f);	
    return float4(z, z, z, 1);
#endif
    
    return float4(0, 0, 0, 0);
}
