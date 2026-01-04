layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

const ivec2 _102[4] = ivec2[](ivec2(0), ivec2(1, 0), ivec2(0, 1), ivec2(1));
const ivec2 _109[9] = ivec2[](ivec2(0), ivec2(-1), ivec2(0, -1), ivec2(1, -1), ivec2(-1, 0), ivec2(1, 0), ivec2(-1, 1), ivec2(0, 1), ivec2(1));
const uint _114[4] = uint[](23u, 45u, 209u, 417u);

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

layout(binding = 2, r32f) uniform writeonly highp image2D rw_accumulation;
layout(binding = 1, r32f) uniform writeonly highp image2D rw_new_locks;
layout(binding = 0, rgba8) uniform writeonly highp image2D rw_dilated_reactive_masks;
uniform highp sampler2D SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_dilated_depthSPIRV_Cross_DummySampler;
uniform highp usampler2D SPIRV_Cross_Combinedr_reconstructed_previous_nearest_depthSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_reactive_maskSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_shading_changes_LinearClamp;
uniform highp sampler2D SPIRV_Cross_Combinedr_transparency_and_composition_maskSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_transparency_and_composition_masks_LinearClamp;
uniform highp sampler2D SPIRV_Cross_Combinedr_accumulations_LinearClamp;
uniform highp sampler2D SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler;

vec4 _99;
float _100;

void main()
{
    vec2 _126 = vec2(ivec3(gl_GlobalInvocationID).xy) + vec2(0.5);
    vec2 _129 = vec2(cbFSR3Upscaler.iRenderSize);
    vec2 _130 = _126 / _129;
    uvec2 _131 = uvec2(ivec3(gl_GlobalInvocationID).xy);
    vec2 _134 = texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(_131), int(0u)).xy;
    float _139 = pow(texelFetch(SPIRV_Cross_Combinedr_dilated_depthSPIRV_Cross_DummySampler, ivec2(_131), int(0u)).x, 5.0) * 1000.0;
    float _142 = _139 * cbFSR3Upscaler.fViewSpaceToMetersFactor;
    float _151 = length(_134 * vec2(3840.0, 2160.0)) * cbFSR3Upscaler.fVelocityFactor;
    vec2 _157 = ((_130 + (_134 * float(_151 > mix(0.25, 0.75, clamp(min(_142, 65504.0) * 0.00999999977648258209228515625, 0.0, 1.0))))) * _129) - vec2(0.5);
    vec2 _158 = floor(_157);
    vec2 _160 = _157 - _158;
    float _161 = _160.x;
    float _162 = 1.0 - _161;
    float _163 = _160.y;
    float _164 = 1.0 - _163;
    float _118[4] = float[](_162 * _164, _161 * _164, _162 * _163, _161 * _163);
    float _171;
    float _174;
    bool _176;
    _171 = 0.0;
    _174 = 0.0;
    _176 = true;
    float _172;
    float _175;
    bool _177;
    for (int _178 = 0; (_178 < 4) && _176; _171 = _172, _174 = _175, _176 = _177, _178++)
    {
        ivec2 _186 = ivec2(_158) + _102[_178];
        ivec2 _192 = _186;
        _192.x = max(0, min(_186.x, (cbFSR3Upscaler.iRenderSize.x - 1)));
        ivec2 _198 = _192;
        _198.y = max(0, min(_186.y, (cbFSR3Upscaler.iRenderSize.y - 1)));
        uvec2 _199 = uvec2(_198);
        if (all(lessThan(_199, uvec2(cbFSR3Upscaler.iRenderSize))))
        {
            float _231;
            float _232;
            bool _233;
            if (_118[_178] > 0.0006099999882280826568603515625)
            {
                float _213 = float(texelFetch(SPIRV_Cross_Combinedr_reconstructed_previous_nearest_depthSPIRV_Cross_DummySampler, ivec2(_199), int(0u)).x);
                float _214 = _139 - _213;
                bool _216 = _176 && (_214 > 1.1754943508222875079687365372222e-38);
                float _229;
                float _230;
                if (_216)
                {
                    _229 = _171 + (clamp(((1.3699999726668465882539749145508e-05 * length(_129 * 0.5)) * max(_139, _213)) / _214, 0.0, 1.0) * _118[_178]);
                    _230 = _174 + _118[_178];
                }
                else
                {
                    _229 = _171;
                    _230 = _174;
                }
                _231 = _229;
                _232 = _230;
                _233 = _216;
            }
            else
            {
                _231 = _171;
                _232 = _174;
                _233 = _176;
            }
            _172 = _231;
            _175 = _232;
            _177 = _233;
        }
        else
        {
            _172 = _171;
            _175 = _174;
            _177 = _176;
        }
    }
    float _239 = (_176 && (_174 > 0.0)) ? clamp(1.0 - (_171 / _174), 0.0, 1.0) : 0.0;
    float _241;
    int _244;
    _241 = 0.0;
    _244 = -1;
    float _242;
    for (; _244 <= 1; _241 = _242, _244++)
    {
        _242 = _241;
        for (int _252 = -1; _252 <= 1; )
        {
            ivec2 _257 = ivec3(gl_GlobalInvocationID).xy + ivec2(_252, _244);
            ivec2 _263 = _257;
            _263.x = max(0, min(_257.x, (cbFSR3Upscaler.iRenderSize.x - 1)));
            ivec2 _269 = _263;
            _269.y = max(0, min(_257.y, (cbFSR3Upscaler.iRenderSize.y - 1)));
            _242 = max(_242, texelFetch(SPIRV_Cross_Combinedr_reactive_maskSPIRV_Cross_DummySampler, ivec2(uvec2(_269)), int(0u)).x);
            _252++;
            continue;
        }
    }
    vec2 _282 = vec2(cbFSR3Upscaler.iMaxRenderSize);
    vec2 _285 = vec2(ivec2(_129 * 0.5));
    float _298 = max(_241, clamp(textureLod(SPIRV_Cross_Combinedr_shading_changes_LinearClamp, max(vec2(0.5), min((_130 - (cbFSR3Upscaler.fJitter / _129)) * _285, _285 - vec2(0.5))) / vec2(ivec2(_282 * 0.5)), 0.0).x, 0.0, 1.0));
    vec2 _299 = _130 + _134;
    uvec2 _302 = uvec2(ivec2(_299 * _129));
    float _312 = (pow(texelFetch(SPIRV_Cross_Combinedr_dilated_depthSPIRV_Cross_DummySampler, ivec2(_302), int(0u)).x, 5.0) * 1000.0) * cbFSR3Upscaler.fViewSpaceToMetersFactor;
    float _313 = max(_312, _142);
    uvec2 _326 = uvec2(textureSize(SPIRV_Cross_Combinedr_transparency_and_composition_maskSPIRV_Cross_DummySampler, 0));
    float _341 = _299.x;
    float _345 = _299.y;
    float _365;
    if (((_341 >= 0.0) && (_341 <= 1.0)) && ((_345 >= 0.0) && (_345 <= 1.0)))
    {
        vec2 _354 = vec2(cbFSR3Upscaler.iPreviousFrameRenderSize);
        _365 = clamp(textureLod(SPIRV_Cross_Combinedr_accumulations_LinearClamp, max(vec2(0.5), min(_299 * _354, _354 - vec2(0.5))) / _282, 0.0).x, 0.0, 1.0);
    }
    else
    {
        _365 = 0.0;
    }
    float _366 = mix(_365, 0.0, _298);
    float _368 = mix(_366, min(_366, 0.25), _239);
    float _373 = _368 * float(round(_368 * 100.0) > 1.0);
    imageStore(rw_accumulation, ivec2(_131), vec4(clamp(_373 + 0.3333333432674407958984375, 0.0, 1.0)));
    vec4 _377 = _99;
    _377.x = max(((1.0 - clamp((length(texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(_302), int(0u)).xy * vec2(3840.0, 2160.0)) * cbFSR3Upscaler.fVelocityFactor) / _151, 0.0, 1.0)) * ((_313 != 0.0) ? (min(_312, _142) / _313) : 0.0)) * clamp(_151 * 0.100000001490116119384765625, 0.0, 1.0), textureLod(SPIRV_Cross_Combinedr_transparency_and_composition_masks_LinearClamp, max(vec2(0.5), min(_126, _129 - vec2(0.5))) / vec2(ivec2(int(_326.x), int(_326.y))), 0.0).x);
    vec4 _378 = _377;
    _378.y = _239;
    vec4 _379 = _378;
    _379.z = _298;
    vec4 _380 = _379;
    _380.w = _373;
    imageStore(rw_dilated_reactive_masks, ivec2(_131), _380);
    float _486;
    switch (0u)
    {
        default:
        {
            float _385;
            float _388;
            _385 = 1.1754943508222875079687365372222e-38;
            _388 = 3.4028234663852885981170418348452e+38;
            float _121[9];
            for (int _390 = 0; _390 < 9; )
            {
                ivec2 _396 = ivec3(gl_GlobalInvocationID).xy + _109[_390];
                ivec2 _402 = _396;
                _402.x = max(0, min(_396.x, (cbFSR3Upscaler.iRenderSize.x - 1)));
                ivec2 _408 = _402;
                _408.y = max(0, min(_396.y, (cbFSR3Upscaler.iRenderSize.y - 1)));
                vec4 _414 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                float _415 = _414.x;
                _121[_390] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_408)), int(0u)).x * ((_415 == 0.0) ? 1.0 : _415);
                _385 = max(_385, _121[_390]);
                _388 = min(_388, _121[_390]);
                _390++;
                continue;
            }
            uint _426;
            float _428;
            float _430;
            _426 = 1u;
            _428 = 3.4028234663852885981170418348452e+38;
            _430 = 0.0;
            uint _427;
            float _429;
            float _431;
            for (int _423 = 1, _432 = 1; _432 < 9; _423++, _426 = _427, _428 = _429, _430 = _431, _432++)
            {
                if ((abs(_121[_432] - _121[0]) / (_385 - _388)) < 0.89999997615814208984375)
                {
                    _427 = _426 | (1u << (uint(_423) & 31u));
                    _429 = _428;
                    _431 = _430;
                }
                else
                {
                    _427 = _426;
                    _429 = min(_428, _121[_432]);
                    _431 = max(_430, _121[_432]);
                }
            }
            if (false == ((_121[0] > _430) || (_121[0] < _428)))
            {
                _486 = 0.0;
                break;
            }
            float _481;
            bool _482;
            int _467 = 0;
            for (;;)
            {
                if (uint(_467) < 4u)
                {
                    if ((_426 & _114[_467]) == _114[_467])
                    {
                        _481 = 0.0;
                        _482 = true;
                        break;
                    }
                    _467++;
                    continue;
                }
                else
                {
                    _481 = _100;
                    _482 = false;
                    break;
                }
            }
            _486 = 1.0 - (_388 / _385);
            break;
        }
    }
    if (_486 > 0.00999999977648258209228515625)
    {
        imageStore(rw_new_locks, ivec2(uvec2(ivec2(floor(((_126 - cbFSR3Upscaler.fJitter) / _129) * vec2(cbFSR3Upscaler.iUpscaleSize))))), vec4(_486));
    }
}

