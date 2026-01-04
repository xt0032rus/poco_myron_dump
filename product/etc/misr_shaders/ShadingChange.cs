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

layout(binding = 0, r32f) uniform writeonly highp image2D rw_shading_change;
uniform highp sampler2D SPIRV_Cross_Combinedr_spd_mipsSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_spd_mipss_LinearClamp;

void main()
{
    vec2 _52 = vec2(cbFSR3Upscaler.iRenderSize);
    ivec2 _54 = ivec2(_52 * 0.5);
    uvec2 _55 = uvec2(ivec3(gl_GlobalInvocationID).xy);
    if (all(lessThan(_55, uvec2(_54))))
    {
        vec2 _63 = vec2(_54);
        uvec2 _70 = uvec2(textureSize(SPIRV_Cross_Combinedr_spd_mipsSPIRV_Cross_DummySampler, int(0u)));
        float _47[3];
        for (int _83 = 0; _83 < 3; )
        {
            vec4 _92 = textureLod(SPIRV_Cross_Combinedr_spd_mipss_LinearClamp, max(vec2(0.5), min((((vec2(ivec3(gl_GlobalInvocationID).xy) + vec2(0.5)) / _63) + (cbFSR3Upscaler.fJitter / _52)) * _63, _63 - vec2(0.5))) / vec2(ivec2(int(_70.x), int(_70.y))), float(uint(_83)));
            _47[_83] = abs(_92.x * _92.y);
            _83++;
            continue;
        }
        float _46[3] = _47;
        float _100;
        _100 = 0.0;
        float _101;
        for (int _103 = 0; _103 < 3; _100 = _101, _103++)
        {
            if (_46[_103] > 0.0)
            {
                _101 = max(_100, _46[_103]);
            }
            else
            {
                _101 = _100;
            }
        }
        imageStore(rw_shading_change, ivec2(_55), vec4(clamp(_100, 0.0, 1.0)));
    }
}

