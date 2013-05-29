#ifndef SHADOW_CALCULATIONS
#include "globals.fx"

// ---------------------------------
// Constants
// ---------------------------------
static const float SHADOWBIAS = 0.00006f;

// ---------------------------------
// Functions
// ---------------------------------
// Calculates the shadow occlusion using bilinear PCF
float CalcShadowTermPCF(float lightDepth, float2 shadowMapCoordinates, float2 shadowMapSize)
{
    float shadowTerm = 0.0f;

    // transform to texel space
    float2 shadowMapCoordinatesTS = shadowMapSize * shadowMapCoordinates;
    
    // Determine the lerp amounts           
    float2 lerps = frac(shadowMapCoordinatesTS);

    // Read in the 4 samples, doing a depth check for each
    float samples[4];
    samples[0] = (ShadowMap.Sample(ShadowMapSampler, shadowMapCoordinates).r + SHADOWBIAS < lightDepth) ? 0.0f: 1.0f;  
    samples[1] = (ShadowMap.Sample(ShadowMapSampler, shadowMapCoordinates + float2(1.0 / shadowMapSize.x, 0)).r + SHADOWBIAS < lightDepth) ? 0.0f: 1.0f;  
    samples[2] = (ShadowMap.Sample(ShadowMapSampler, shadowMapCoordinates + float2(0, 1.0 / shadowMapSize.y)).r + SHADOWBIAS < lightDepth) ? 0.0f: 1.0f;  
    samples[3] = (ShadowMap.Sample(ShadowMapSampler, shadowMapCoordinates + float2(1.0 / shadowMapSize.x, 1.0 / shadowMapSize.y)).r + SHADOWBIAS < lightDepth) ? 0.0f: 1.0f;  

    // lerp between the shadow values to calculate our light amount
    shadowTerm = lerp(lerp(samples[0], samples[1], lerps.x), lerp( samples[2], samples[3], lerps.x), lerps.y);							  
                                
    return shadowTerm;
}

// Calculates the shadow term using PCF soft-shadowing
float CalcShadowTermSoftPCF(float lightDepth, float2 shadowMapCoordinates, float2 shadowMapSize, int sqrtSamples)
{
    float shadowTerm = 0.0f;  
    
    float radius = (sqrtSamples - 1.0f) / 2;
    float weightAccumulator = 0.0f;
    
    for (float y = -radius; y <= radius; y++)
    {
        for (float x = -radius; x <= radius; x++)
        {
            float2 offset = float2(x, y) / shadowMapSize;
            float2 samplePoint = shadowMapCoordinates + offset;			
            float depth = ShadowMap.Sample(ShadowMapSampler, samplePoint).x;
            float sample = (lightDepth <= depth + SHADOWBIAS);
            
            // Edge tap smoothing
            float xWeight = 1;
            float yWeight = 1;
            
            if (x == -radius)
                xWeight = 1 - frac(shadowMapCoordinates.x * shadowMapSize.x);
            else if (x == radius)
                xWeight = frac(shadowMapCoordinates.x * shadowMapSize.x);
                
            if (y == -radius)
                yWeight = 1 - frac(shadowMapCoordinates.y * shadowMapSize.y);
            else if (y == radius)
                yWeight = frac(shadowMapCoordinates.y * shadowMapSize.y);
                
            shadowTerm += sample * xWeight * yWeight;
            weightAccumulator = xWeight * yWeight;
        }											
    }		
    
    shadowTerm /= (sqrtSamples * sqrtSamples);
    shadowTerm *= 1.55f;    
    return shadowTerm;
}

#endif
