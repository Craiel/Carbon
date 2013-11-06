#include "Globals.fx"

// ---------------------------------
struct VS_INPUT
{
    float4 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
    float4 Tangent : TANGENT;
    
#if INSTANCED == 1
    uint InstanceID : SV_InstanceID;
#endif
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL;
    float3 NormalVS : NORMALVS;
    float2 TextureCoordinates : TEXCOORD;
    float3 Tangent : TANGENTVS;
    float3 BiTangent : BITANGENTVS;
};

struct PS_OUTPUT
{
    float4 Normal : SV_TARGET0;
    float4 DiffuseAlbedo : SV_TARGET1;
    float4 SpecularAlbedo : SV_TARGET2;
};

// ---------------------------------
// Vertex Shader
// ---------------------------------
PS_INPUT VS(in VS_INPUT input)
{
    PS_INPUT output;
    
    matrix worldMatrix = World;
    
#if INSTANCED == 1
    worldMatrix = Instances[input.InstanceID].World;
#endif

    matrix worldView = mul(View, World);
    
    // normals to view space
    output.Normal = normalize(mul((float3x3)worldMatrix, input.Normal));
    output.NormalVS = normalize(mul((float3x3)worldView, input.Normal));
    
    // Calculate Bi-tangent
    float3 tangent = normalize(mul((float3x3)worldView, input.Tangent.xyz));
    float3 biTangent = normalize(cross(output.Normal, tangent)) * input.Tangent.w;
    output.Tangent = tangent;
    output.BiTangent = biTangent;

    // Calculate Clip-space position
    output.Position = mul(worldMatrix, input.Position);
    output.Position = mul(View, output.Position);
    output.Position = mul(Projection, output.Position);

    // Pass along the rest
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

// ---------------------------------
// Pixel Shader
// ---------------------------------
PS_OUTPUT PS(in PS_INPUT input)
{
    PS_OUTPUT output;

    // Sample diffuse
    float3 diffuse = DiffuseMap.Sample(DiffuseSampler, input.TextureCoordinates).rgb;	

#if NORMALMAP
    // Sample normal ( and decompress )
    float3 normal = normalize(NormalMap.Sample(NormalSampler, input.TextureCoordinates).rgb * 2.0f - 1.0f);

    // Normalize tangent and bring into view-space
    float3x3 tangentFrame = float3x3(normalize(input.Tangent), normalize(input.BiTangent), normalize(input.NormalVS));
    normal = mul(normal, tangentFrame);
#else
    float3 normal = input.NormalVS;
#endif
      
	output.Normal = float4(normal, 1.0f);
    //output.Normal = float4(SpheremapEncode(normal), 1.0f, 1.0f);
    output.DiffuseAlbedo = float4(diffuse, 1.0f);
    output.SpecularAlbedo = float4(0.7f, 0.7f, 0.7f, 64.0f / 255.0f);

    return output;
}
