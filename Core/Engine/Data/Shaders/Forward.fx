#include "Globals.fx"

#if LIGHTING
#define MAX_DIRECTIONAL_LIGHTS 4
#define MAX_POINT_LIGHTS 8
#define MAX_SPOT_LIGHTS 4

struct DIRECTIONAL_LIGHT_DATA
{
    float4 DiffuseColor;
    float4 LightDirection;
    float SpecularPower;
    float3 padding;
};

struct POINT_LIGHT_DATA
{
    float4 DiffuseColor;
    float4 Position;
    float Range;
    float SpecularPower;
    float2 Padding;
};

struct SPOT_LIGHT_DATA
{
    float4 DiffuseColor;
    float4 Position;
    float4 LightDirection;
    float Range;
    float SpecularPower;
    float2 SpotlightAngles;
};

struct AMBIENT_LIGHT_DATA
{
    float4 DiffuseColor;
    float SpecularPower;
    float Padding;
};

cbuffer LightBuffer : register(b2)
{
    float4 CameraPosition;
    float4 AmbientLight;
    float DirectionalLights;
    float PointLights;
    float SpotLights;
    float Padding;
    
    DIRECTIONAL_LIGHT_DATA DirectionalLightData[MAX_DIRECTIONAL_LIGHTS];
    POINT_LIGHT_DATA PointLightData[MAX_POINT_LIGHTS];
    SPOT_LIGHT_DATA SpotLightData[MAX_SPOT_LIGHTS];
};
#endif

cbuffer MaterialBuffer : register(b3)
{    
    float4 MeshColor;
};

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
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
    
    float4 WorldPosition : POSITION0;
    float3 CamPos : POSITION1;
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
        
    output.WorldPosition = mul(worldMatrix, input.Position);
    output.Position = mul(View, output.WorldPosition);
    output.Position = mul(Projection, output.Position);
    output.TextureCoordinates = input.TextureCoordinates;
    output.Normal = normalize(mul((float3x3)worldMatrix, input.Normal));

    return output;
}

// ---------------------------------
// Pixel Shader
// ---------------------------------
float4 PS(PS_INPUT input) : SV_Target
{
    float4 color = float4(0, 0, 0, 1);
    
#if DIFFUSEMAP == 1
    float4 textureColor = DiffuseMap.Sample(DiffuseSampler, input.TextureCoordinates);
    textureColor.w = 1.0f;
#if ALPHAMAP == 1
    float alpha = AlphaMap.Sample(DiffuseSampler, input.TextureCoordinates).x;
    textureColor.w *= alpha;
#endif
    color = textureColor;
    #if USECOLOR == 1
        color *= MeshColor;
    #endif
#else
    #if USECOLOR == 1
        color = MeshColor;
    #endif
#endif

#if LIGHTING
    float3 lighting = AmbientLight;
    float attenuation = 1.0f;
    
    float3 normal = input.Normal;
    #if NORMALMAP
    // Sample normal ( and decompress )
    normal = normalize(NormalMap.Sample(NormalSampler, input.TextureCoordinates));
    #endif
    
    #if SPECULARMAP
    int3 specularCoord = int3(input.TextureCoordinates.xy, 0);
    float specular = SpecularMap.Load(specularCoord).x;
    return float4(1, 0, 0, 1); // Todo: not implemented
    #endif

    for( int i = 0;i < DirectionalLights; i++ )
    {
        float3 lightColor = CalculateDirectionalLighting(CameraPosition, 
                                                         DirectionalLightData[i].LightDirection.xyz, 
                                                         DirectionalLightData[i].DiffuseColor.xyz, 
                                                         normal, 
                                                         input.WorldPosition, 
                                                         float3(1,1,1), 
                                                         float3(1,1,1), 
                                                         DirectionalLightData[i].SpecularPower);
        
        lighting += lightColor * DirectionalLightData[i].DiffuseColor.a;
    }
    
    for( i = 0;i < PointLights; i++ )
    {
        float3 lightColor = CalculatePointLighting(CameraPosition, 
                                                   PointLightData[i].Position, 
                                                   PointLightData[i].DiffuseColor.xyz, 
                                                   PointLightData[i].Range,
                                                   normal, 
                                                   input.WorldPosition, 
                                                   float3(1,1,1), 
                                                   float3(1,1,1), 
                                                   PointLightData[i].SpecularPower);
        
        lighting += lightColor * PointLightData[i].DiffuseColor.a;
    }
    
    for( i = 0; i < SpotLights; i++ )
    {
        float3 lightColor = CalculateSpotLighting(CameraPosition, 
                                                  SpotLightData[i].Position, 
                                                  SpotLightData[i].LightDirection.xyz, 
                                                  SpotLightData[i].DiffuseColor.xyz, 
                                                  SpotLightData[i].Range,
                                                  SpotLightData[i].SpotlightAngles,
                                                  normal, 
                                                  input.WorldPosition, 
                                                  float3(1,1,1), 
                                                  float3(1,1,1), 
                                                  SpotLightData[i].SpecularPower);
                                                         
        lighting += lightColor * SpotLightData[i].DiffuseColor.a;
    }
#else
    float3 lighting = 1;
#endif

    return color * float4(lighting, 1);
}
