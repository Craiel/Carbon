#include "globals.fx"

cbuffer LightBuffer : register(b2)
{
    float4 LightColor;
    float4 LightPosition;
    float4 LightDirection;
    float2 SpotlightAngles;
    float LightRange;
    float Padding;
    matrix LightView;
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
    int3 sampleCoord = int3(input.Position.xy, 0);

    // Get the GBuffer Attributes
    //float3 normal = SpheremapDecode(NormalMap.Load(sampleCoord));
	float3 normal = NormalMap.Load(sampleCoord).xyz;
    float3 diffuseAlbedo = DiffuseMap.Load(sampleCoord).xyz;
    float4 specular = SpecularMap.Load(sampleCoord);    
    float depth = DepthMap.Load(sampleCoord).x;
    
    float3 position = PositionFromDepth(depth, input.ViewRay);
        
    float3 specularAlbedo = specular.xyz;
    float specularPower = specular.w * 255.0f;
    float3 color = 0;

	// Camera is at 0,0,0 in View Space
	float3 cameraPosition = 0;
	
	float shadowFactor = 1;
#if SHADOWMAPPING
	//float shadowMapDepth = ShadowMap.Sample(ShadowMapSampler, input.LightPosition.xy).r;
	//float4 viewSpacePosition = mul(position, InvertedProjection);
	//float3 viewSpacePosition = mul(position, InvertedViewProjection);
	float4 viewSpacePosition = mul(position, LightView);
	viewSpacePosition = mul(position, Projection);
	float2 vShadowTexCoord = 0.5 * viewSpacePosition.xy / viewSpacePosition.w + float2(0.5f, 0.5f);
	vShadowTexCoord += (0.5f / float2(1024,768));
	float3 lightVector = LightPosition.xyz - position;
	float distance = 1.0f - (400 / length(lightVector));
	
	float shadowMapDepth = ShadowMap.Sample(ShadowMapSampler, vShadowTexCoord).r;
	if(shadowMapDepth < distance)
	{
		return float4(0, 0, 0, 0);
	}
	
	float near = 0.999f; // 0
	float far = 0.99999f; // 1
	float z = (shadowMapDepth - near) / (far - near);
	return float4(z, z, z, 1.0f);
#endif
	
#if AMBIENTLIGHT
	color = LightColor;
#elif DIRECTIONALLIGHT	
	color = CalculateDirectionalLighting(cameraPosition, LightDirection.xyz, LightColor, normal, position, diffuseAlbedo, specularAlbedo, specularPower);
#elif POINTLIGHT
	color = CalculatePointLighting(cameraPosition, LightPosition, LightColor, LightRange.x, normal, position, diffuseAlbedo, specularAlbedo, specularPower);
#elif SPOTLIGHT
	color = CalculateSpotLighting(cameraPosition, LightPosition, LightDirection.xyz, LightColor, LightRange.x, SpotlightAngles, normal, position, diffuseAlbedo, specularAlbedo, specularPower);
#endif
	
    return float4(color, 1.0f);
}
