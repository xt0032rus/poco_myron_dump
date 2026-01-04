layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

const ivec2 _66[9] = ivec2[](ivec2(0), ivec2(1, 0), ivec2(0, 1), ivec2(0, -1), ivec2(-1, 0), ivec2(-1, 1), ivec2(1), ivec2(-1), ivec2(1, -1));

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

layout(binding = 1, std140) uniform type_cbSceneInfo
{
    mat4 transform;
} cbSceneInfo;

layout(binding = 0, rgba8) uniform writeonly highp image2D rw_dilated_motion_vectors;
uniform highp sampler2D SPIRV_Cross_Combinedr_input_depthSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_input_motion_vectorsSPIRV_Cross_DummySampler;

void main()
{
    float _74[9];
    for (int _78 = 0; _78 < 9; )
    {
        _74[_78] = texelFetch(SPIRV_Cross_Combinedr_input_depthSPIRV_Cross_DummySampler, ivec2(uvec2(ivec3(gl_GlobalInvocationID).xy + _66[_78])), int(0u)).x;
        _78++;
        continue;
    }
    float _94;
    ivec2 _97;
    _94 = _74[0];
    _97 = ivec3(gl_GlobalInvocationID).xy;
    float _95;
    ivec2 _98;
    for (int _99 = 1; _99 < 9; _94 = _95, _97 = _98, _99++)
    {
        ivec2 _106 = ivec3(gl_GlobalInvocationID).xy + _66[_99];
        if (all(lessThan(uvec2(_106), uvec2(cbFSR3Upscaler.iRenderSize))))
        {
            bool _117 = _74[_99] < _94;
            _95 = _117 ? _74[_99] : _94;
            _98 = mix(_97, _106, bvec2(_117));
        }
        else
        {
            _95 = _94;
            _98 = _97;
        }
    }
    uvec2 _121 = uvec2(_97);
    vec2 _168;
    switch (0u)
    {
        default:
        {
            if (all(lessThan(_121, uvec2(cbFSR3Upscaler.iRenderSize))))
            {
                vec4 _132 = texelFetch(SPIRV_Cross_Combinedr_input_motion_vectorsSPIRV_Cross_DummySampler, ivec2(_121), int(0u));
                vec3 _166;
                if (_132.z < 0.001000000047497451305389404296875)
                {
                    vec2 _141 = (vec2(_121) + vec2(0.5)) / vec2(cbFSR3Upscaler.iRenderSize);
                    vec4 _151 = cbSceneInfo.transform * vec4((_141 * vec2(2.0)) - vec2(1.0), (2.0 * _94) - 1.0, 1.0);
                    vec2 _158 = (((_151.xy / vec2(_151.w)) + vec2(1.0)) * vec2(0.5)) - _141;
                    _166 = vec3(_158.x, _158.y, _132.z);
                }
                else
                {
                    vec2 _164 = ((_132.xy - vec2(0.4999924004077911376953125)) * vec2(4.008016109466552734375)).xy * vec2(-0.5);
                    _166 = vec3(_164.x, _164.y, _132.z);
                }
                _168 = _166.xy;
                break;
            }
            _168 = vec2(0.0);
            break;
        }
    }
    imageStore(rw_dilated_motion_vectors, ivec2(uvec2(ivec3(gl_GlobalInvocationID).xy)), vec4(_168, 0.0, 0.0));
}

