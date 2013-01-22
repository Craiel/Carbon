#include "globals.fx"

// ---------------------------------
struct VS_INPUT
{
    float4 Position : POSITION;
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
};

// ---------------------------------
// Vertex Shader
// ---------------------------------
PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output = (PS_INPUT)0;
    
    // Just copy the position
    output.Position = input.Position;

    return output;
}

// ---------------------------------
// Pixel Shader
// ---------------------------------
float4 PS(PS_INPUT input) : SV_Target
{
    int3 sampleCoord = int3(input.Position.xy, 0);

    float3 diffuse = DiffuseMap.Load(sampleCoord).xyz;
	float3 specularLight = SpecularMap.Load(sampleCoord).xyz;

	float3 color = diffuse * specularLight;

    return float4(color, 1.0f);
}
