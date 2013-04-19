#include "globals.fx"
#include "ShadowCalculations.fx"

cbuffer LightBuffer : register(b2)
{
    float4 LightColor;
    float4 LightPositionVS;
    float4 LightDirectionVS;
    float2 SpotlightAngles;
    float LightRange;
    float Padding;
    float2 ShadowMapSize;
    float2 Padding2;
    matrix LightViewProjection;
};

cbuffer CameraBuffer : register(b3)
{    
    float4 CameraPosition;
};

// ---------------------------------
struct VS_INPUT
{
    float4 Position : POSITION;
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
    float3 ViewRay : VIEWRAY;
};

// ---------------------------------
// Vertex Shader
// ---------------------------------
PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output = (PS_INPUT)0;
    
    // Just copy the position
    output.Position = input.Position;

    // For a quad we can clamp in the vertex shader, since we only interpolate in the XY direction
    float3 viewSpacePosition = mul(input.Position, InvertedProjection).xyz;
    output.ViewRay = float3(viewSpacePosition.xy / viewSpacePosition.z, 1.0f);
    
    return output;
}

// ---------------------------------
// Pixel Shader
// ---------------------------------


float4 PS(PS_INPUT input) : SV_Target
{	
    // Input position is in Screen Coordinates i.e 1024 x 768
    int3 sampleCoord = int3(input.Position.xy, 0);

    // Get the GBuffer Attributes
    float3 normalVS = NormalMap.Load(sampleCoord).xyz;
    float3 diffuseAlbedoVS = DiffuseMap.Load(sampleCoord).xyz;
    float4 specularVS = SpecularMap.Load(sampleCoord);
    float depthVS = DepthMap.Load(sampleCoord).x;
    
    float3 positionVS = PositionFromDepth(depthVS, input.ViewRay);
    
    float3 specularAlbedo = specularVS.xyz;
    float specularPower = specularVS.w * 255.0f;
    float3 color = 0;

    // Camera is at 0,0,0 in View Space
    float3 cameraPositionVS = 0;
    
    float shadowTerm = 0;
    
#if SHADOWMAPPING
    // Bring the View Space position into world space and then back into clip space for our light
    float4 positionWorld = mul(float4(positionVS, 1.0), InvertedView);
    float4 positionLightCS = mul(positionWorld, LightViewProjection);	
    positionLightCS = (positionLightCS / positionLightCS.w);
    
    // Todo: Inverted, for no reason?????
    float2 shadowMapCoordinates = (positionLightCS.xy * 0.5 + 0.5f);
    shadowMapCoordinates.y = 1.0f - shadowMapCoordinates.y; // Hack to invert it properly
    float shadowMapDepth = ShadowMap.Sample(ShadowMapSampler, shadowMapCoordinates).r;
    
    /*if(positionLightCS.z > shadowMapDepth) // In shadow
    {
        return float4(0,0,0,1);
    }*/

    //shadowTerm = CalcShadowTermPCF(positionLightCS.z, shadowMapCoordinates, ShadowMapSize);
	shadowTerm = CalcShadowTermSoftPCF(positionLightCS.z, shadowMapCoordinates, ShadowMapSize, 4);
#endif
    
#if AMBIENTLIGHT
    color = LightColor;
#elif DIRECTIONALLIGHT	
    color = CalculateDirectionalLighting(cameraPositionVS, LightDirectionVS.xyz, LightColor, normalVS, positionVS, diffuseAlbedoVS, specularAlbedo, specularPower);
#elif POINTLIGHT
    color = CalculatePointLighting(cameraPositionVS, LightPositionVS, LightColor, LightRange.x, normalVS, positionVS, diffuseAlbedoVS, specularAlbedo, specularPower);
#elif SPOTLIGHT
    color = CalculateSpotLighting(cameraPositionVS, LightPositionVS, LightDirectionVS.xyz, LightColor, LightRange.x, SpotlightAngles, normalVS, positionVS, diffuseAlbedoVS, specularAlbedo, specularPower);
#endif
    
    return float4(color, 1.0f) * shadowTerm;
}
