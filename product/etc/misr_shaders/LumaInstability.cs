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

layout(binding = 0, rgba8) uniform writeonly highp image2D rw_luma_history;
layout(binding = 1, r32f) uniform writeonly highp image2D rw_luma_instability;
uniform highp sampler2D SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_dilated_reactive_maskss_LinearClamp;
uniform highp sampler2D SPIRV_Cross_Combinedr_current_lumas_LinearClamp;
uniform highp sampler2D SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_luma_historys_LinearClamp;

void main()
{
    uvec2 _68 = uvec2(ivec3(gl_GlobalInvocationID).xy);
    vec2 _71 = texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(_68), int(0u)).xy;
    vec2 _76 = vec2(cbFSR3Upscaler.iRenderSize);
    vec2 _77 = (vec2(ivec3(gl_GlobalInvocationID).xy) + vec2(0.5)) / _76;
    vec2 _86 = vec2(cbFSR3Upscaler.iPreviousFrameRenderSize);
    vec2 _89 = (_77 + (cbFSR3Upscaler.fPreviousFrameJitter / _86)) + _71;
    float _90 = _89.x;
    float _94 = _89.y;
    float _226;
    vec4 _227;
    if (((_90 >= 0.0) && (_90 <= 1.0)) && ((_94 >= 0.0) && (_94 <= 1.0)))
    {
        vec2 _107 = vec2(cbFSR3Upscaler.iMaxRenderSize);
        vec2 _108 = max(vec2(0.5), min((_77 + (cbFSR3Upscaler.fJitter / _76)) * _76, _76 - vec2(0.5))) / _107;
        vec4 _112 = textureLod(SPIRV_Cross_Combinedr_dilated_reactive_maskss_LinearClamp, _108, 0.0);
        float _224;
        vec4 _225;
        if (clamp(_112.w, 0.0, 1.0) > 0.89999997615814208984375)
        {
            vec4 _129 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
            float _130 = _129.x;
            float _133 = textureLod(SPIRV_Cross_Combinedr_current_lumas_LinearClamp, _108, 0.0).x * ((_130 == 0.0) ? 1.0 : _130);
            vec4 _145 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
            float _146 = _145.x;
            vec4 _65 = (textureLod(SPIRV_Cross_Combinedr_luma_historys_LinearClamp, max(vec2(0.5), min(_89 * _86, _86 - vec2(0.5))) / _107, 0.0) * cbFSR3Upscaler.fDeltaPreExposure) * ((_146 == 0.0) ? 1.0 : _146);
            float _154 = max(_133, _65.x);
            float _158 = (_154 != 0.0) ? (min(_133, _65.x) / _154) : 1.0;
            float _191;
            if (_158 < 1.0)
            {
                float _163;
                _163 = _158;
                float _164;
                for (int _166 = 1; _166 <= 3; _163 = _164, _166++)
                {
                    uint _171 = uint(_166);
                    float _176 = max(_133, _65[_171]);
                    if (int(sign(_133 - _65.x)) == int(sign(_133 - _65[_171])))
                    {
                        _164 = max(_163, (_176 != 0.0) ? (min(_133, _65[_171]) / _176) : 0.0);
                    }
                    else
                    {
                        _164 = _163;
                    }
                }
                _191 = float(_163 > _158);
            }
            else
            {
                _191 = 0.0;
            }
            _65.w = _65.z;
            _65.z = _65.y;
            _65.y = _65.x;
            _65.x = _133;
            vec4 _198 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
            float _199 = _198.x;
            vec4 _203 = _65;
            vec4 _204 = _203 / vec4((_199 == 0.0) ? 1.0 : _199);
            _65 = _204;
            _224 = (_191 * float(_65.w != 0.0)) * ((((1.0 - clamp((length(_71 * vec2(3840.0, 2160.0)) * cbFSR3Upscaler.fVelocityFactor) * 0.0500000007450580596923828125, 0.0, 1.0)) * (1.0 - clamp(_112.y, 0.0, 1.0))) * (1.0 - clamp(_112.x, 0.0, 1.0))) * (1.0 - clamp(_112.z, 0.0, 1.0)));
            _225 = _204;
        }
        else
        {
            _224 = 0.0;
            _225 = vec4(0.0);
        }
        _226 = _224;
        _227 = _225;
    }
    else
    {
        _226 = 0.0;
        _227 = vec4(0.0);
    }
    imageStore(rw_luma_history, ivec2(_68), _227);
    imageStore(rw_luma_instability, ivec2(_68), vec4(_226));
}

