#include "globals.fx"

cbuffer LightBuffer : register(b2)
{
    float4 LightColor;
    float4 LightPosition;
    float4 LightDirection;
    float2 SpotlightAngles;
    float LightRange;
    float Padding;
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
	// This outputs gradient!!!
	// It probably samples right, because GBuffer is Repeating textures
	// return float4(input.Position.x / 2048.0f, 0, 0, 1.0f);

	// HACK PROPER UV
	float2 uv = float2(input.Position.x / (1024.0f * 2), input.Position.y / (768.0f*2));
	//return float4(uv.x, uv.y, 0, 1.0f);
	
	
    int3 sampleCoord = int3(input.Position.xy, 0);
    //return float4(sampleCoord.x, 0, 0, 1.0f);

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
	
	/*
	float4 viewSpaceFragment = mul(position, View);
	float lengthFromTheCamera = viewSpaceFragment.z;
	*/
	
	// TREAT THESE TWO AS A SINGULAR TRANSFORM!!!!
	float4 clipSpacePosition = mul(float4(position, 1.0), InvertedView);
	clipSpacePosition = mul(clipSpacePosition, LightViewProjection);	
	float4 perspectiveDivide = (clipSpacePosition / clipSpacePosition.w);		

	// Lets pretend this is absolutely necessary because 'position' is returning
	// linear depth, not logarithmic one!!!
/*
	float near = 0.999f; // 0
	float far = 0.99999f; // 1
	float z = (perspectiveDivide.z - near) / (far - near);
	perspectiveDivide.z = z;
	*/
	
	// Inverted, for no reason?????
	float2 shadowTexCoord = (perspectiveDivide.xy * 0.5 + 0.5f);
	shadowTexCoord.y = 1.0f - shadowTexCoord.y; // Hack to invert it properly
	float shadowMapDepth = ShadowMap.Sample(ShadowMapSampler, shadowTexCoord).r;
	//float shadowMapDepth = ShadowMap.Load(sampleCoord).x;
	
	//shadowMapDepth = (shadowMapDepth - near) / (far - near);
	//return float4(shadowMapDepth, 0, 0, 1.0f);  // Here we're sure we're sampling properly
	
	
	if(perspectiveDivide.z > shadowMapDepth) // In shadow
		return float4(0,0,0,1);
	//else // Lit
	//	return float4(1,0,0,1);
	
	
	
	/*
	float near = 0.999f; // 0
	float far = 0.99999f; // 1
	float z = (shadowMapDepth - near) / (far - near);
	return float4(z, z, z, 1.0f);
*/
#if SHADOWMAPPING
	// float shadowMapDepth = ShadowMap.Sample(ShadowMapSampler, input.LightPosition.xy).r;
	// float4 viewSpacePosition = mul(position, InvertedProjection);
	// float3 viewSpacePosition = mul(position, InvertedViewProjection);
//	float4 clipSpacePosition = mul(position, LightViewProjection);
	//float2 shadowTexCoord = (clipSpacePosition / clipSpacePosition.w) * 0.5 + float2(0.5f, 0.5f);
	//return float4(clipSpacePosition.w, 0,0,1);
	// shadowTexCoord += (0.5f / float2(1024,768));
	
	// float3 lightVector = LightPosition.xyz - position;
	// float distance = 1.0f - (400 / length(lightVector));
	
	//float shadowMapDepth = ShadowMap.Sample(ShadowMapSampler, shadowTexCoord).r;
	//float shadowMapDepth = ShadowMap.Load(int3(shadowTexCoord.xy, 0)).r;
	//if(shadowMapDepth < distance)
	//{
		//return float4(0, 0, 0, 0);
	//
	//float near = 0.999f; // 0
	//float far = 0.99999f; // 1
	//float z = (shadowMapDepth - near) / (far - near);
	//return float4(z, z, z, 1.0f);
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
