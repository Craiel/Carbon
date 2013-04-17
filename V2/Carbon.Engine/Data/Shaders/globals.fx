// ---------------------------------
// Structs
// ---------------------------------
struct INSTANCE_DATA
{
    matrix World;
};

// ---------------------------------
// Textures and Samplers
// ---------------------------------
Texture2D DiffuseMap : register(t0);
Texture2D NormalMap : register(t1);
Texture2D SpecularMap : register(t2);
Texture2D DepthMap : register(t3);
Texture2D ShadowMap : register(t4);
Texture2D AlphaMap : register(t5);
SamplerState DiffuseSampler : register(s0);
SamplerState NormalSampler : register(s1);
SamplerState SpecularSampler : register(s2);
SamplerState ShadowMapSampler : register(s3);

// ---------------------------------
// Registers
// ---------------------------------
cbuffer Constant : register(b0)
{
    matrix World;
    matrix View;
    matrix Projection;
    matrix InvertedView;
    matrix InvertedProjection;
    matrix InvertedViewProjection;
};

#if INSTANCED == 1
#define MAX_INSTANCE_COUNT 1000
cbuffer InstanceBuffer : register(b1)
{
    INSTANCE_DATA Instances[MAX_INSTANCE_COUNT];
}
#endif

// ---------------------------------
// Constants
// ---------------------------------
static const float DiffuseNormalizationFactor = 1.0f / 3.14159265f;

// ---------------------------------
// Functions
// ---------------------------------
#if LIGHTING
float3 CalculateSpecularTerm(in float3 cameraPosition, in float intensity, in float3 normal, in float3 position, in float3 lightVector, in float3 specularAlbedo, in float specularPower, in float3 lightColor)
{
    // Calculate the specular term
    float3 cameraVector = cameraPosition - position;
    float3 normalizedCameraVector = normalize(lightVector + cameraVector);
    float specularNormalizationFactor = ((specularPower + 8.0f) / (8.0f * 3.14159265f));
    return pow(saturate(dot(normal, normalizedCameraVector)), specularPower) * specularNormalizationFactor * lightColor * specularAlbedo.xyz * intensity;
}

float3 CalculateDirectionalLighting(in float3 cameraPosition, in float3 direction, in float3 lightColor, in float3 normal, in float3 position, in float3 diffuseAlbedo, in float3 specularAlbedo, in float specularPower)
{
    float3 lightVector = -direction;
    float intensity = saturate(dot(normal, lightVector));
    float3 diffuse = intensity * lightColor * DiffuseNormalizationFactor; //diffuseAlbedo

    float3 specular = CalculateSpecularTerm(cameraPosition, intensity, normal, position, lightVector, specularAlbedo, specularPower, lightColor);
    return diffuse + specular;
}

float3 CalculatePointLighting(in float3 cameraPosition, in float4 lightPosition, in float3 lightColor, in float lightRange, in float3 normal, in float3 position, in float3 diffuseAlbedo, in float3 specularAlbedo, in float specularPower)
{
	float3 lightVector = lightPosition.xyz - position;
	float distance = length(lightVector);
	float attenuation = max(0, 1.0f - (distance / lightRange.x));

	lightVector /= distance;

	float intensity = saturate(dot(normal, lightVector));
	float3 diffuse = intensity * lightColor * DiffuseNormalizationFactor; // diffuseAlbedo
	float3 specular = CalculateSpecularTerm(cameraPosition, intensity, normal, position, lightVector, specularAlbedo, specularPower, lightColor);

    return (diffuse + specular) * attenuation;
}

float3 CalculateSpotLighting(in float3 cameraPosition, in float4 lightPosition, in float3 direction, in float3 lightColor, in float lightRange, float2 angles, in float3 normal, in float3 position, in float3 diffuseAlbedo, in float3 specularAlbedo, in float specularPower)
{
	float3 lightVector = lightPosition.xyz - position;
	float distance = length(lightVector);
	float attenuation = max(0, 1.0f - (distance / lightRange.x));
	lightVector /= distance;
	
	float spotDot = dot(-lightVector, direction);
	attenuation *= saturate((spotDot - angles.y) / (angles.x - angles.y));
	
	float intensity = saturate(dot(normal, lightVector));
	float3 diffuse = intensity * lightColor * DiffuseNormalizationFactor; // diffuseAlbedo
	float3 specular = CalculateSpecularTerm(cameraPosition, intensity, normal, position, lightVector, specularAlbedo, specularPower, lightColor);

    return (diffuse + specular) * attenuation;
}
#endif

// OUTPUTS POSITION IN VIEW SPACE!!!
// Uses linear depth to figure out where z is
float3 PositionFromDepth(in float zBufferDepth, in float3 viewRay)
{
    #if VOLUMERENDERING
        // Clamp the view space position to the plane at Z = 1
        viewRay = float3(viewRay.xy / viewRay.z, 1.0f);
    #else
        // For a quad we already clamped in the vertex shader
        viewRay = viewRay.xyz;
    #endif

    // Convert to a linear depth value using the projection matrix
    float linearDepth = Projection[3][2] / (zBufferDepth - Projection[2][2]);
    return viewRay * linearDepth;
}

float2 SpheremapEncode(float3 normal)
{
    half2 encoded = normalize(normal.xy) * (sqrt(-normal.z * 0.5 + 0.5));
    return encoded * 0.5 + 0.5;
}

float3 SpheremapDecode(float4 encoded)
{
    float4 nn = encoded * float4(2, 2, 0, 0) + float4(-1, -1, 1, -1);
    float l = dot(nn.xyz, -nn.xyw);
    nn.z = l;
    nn.xy *= sqrt(l);
    return nn.xyz * 2 + float3(0, 0, -1);
}

// Performs advanced "over"-type blending
// (blended is the result of advanced blending 
// (for ex, overlying.rgb * underlying.rgb))
float4 Blend(float3 blended, float4 overlying, float4 underlying)
{
    // Convert to premultiplied format 
    overlying.rgb *= overlying.a;
    underlying.rgb *= underlying.a;

    float4 result = 
        ((1-underlying.a)*overlying) + 
        ((1-overlying.a)*underlying) + 
        (overlying.a * underlying.a) * float4(blended, 1);

    return float4(result.rgb / result.a, result.a);
}
