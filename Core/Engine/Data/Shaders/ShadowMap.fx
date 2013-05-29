#include "globals.fx"

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
    
    //input.Position.w = 1.0f;
    
	output.Position = mul(input.Position, worldMatrix);
	output.Position = mul(output.Position, View);
    output.Position = mul(output.Position, Projection);

    return output;
}

// ---------------------------------
// Pixel Shader
// ---------------------------------
void PS(PS_INPUT input)
{
}
