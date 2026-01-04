layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(binding = 0, std140) uniform type_cbFSR3Upscaler
{
    ivec2 iRenderSize;
    ivec2 iPreviousFrameRenderSize;
    ivec2 iUpscaleSize;
    ivec2 iPreviousFrameUpscaleSize;
    ivec2 iMaxRenderSize;
    ivec2 iMaxUpscaleSize;
    vec4 fDeviceToViewDepth;
    vec2 fJitter;
    vec2 fPreviousFrameJitter;
    vec2 fMotionVectorScale;
    vec2 fDownscaleFactor;
    vec2 fMotionVectorJitterCancellation;
    float fTanHalfFOV;
    float fJitterSequenceLength;
    float fDeltaTime;
    float fDeltaPreExposure;
    float fViewSpaceToMetersFactor;
    float fFrameIndex;
    float fVelocityFactor;
} cbFSR3Upscaler;

layout(binding = 3, rgba8) uniform writeonly highp image2D BlendSceneColor;
uniform highp sampler2D SPIRV_Cross_CombinedSceneColorTextureSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_CombinedUIColorTextureSPIRV_Cross_DummySampler;

void main()
{
    if ((uint(cbFSR3Upscaler.iUpscaleSize.x) > gl_GlobalInvocationID.x) && (uint(cbFSR3Upscaler.iUpscaleSize.y) > gl_GlobalInvocationID.y))
    {
        vec4 _53 = texelFetch(SPIRV_Cross_CombinedUIColorTextureSPIRV_Cross_DummySampler, ivec2(gl_GlobalInvocationID.xy), int(0u));
        imageStore(BlendSceneColor, ivec2(gl_GlobalInvocationID.xy), vec4((texelFetch(SPIRV_Cross_CombinedSceneColorTextureSPIRV_Cross_DummySampler, ivec2(gl_GlobalInvocationID.xy), int(0u)).xyz * (1.0 - _53.w)) + _53.xyz, 1.0));
    }
}

