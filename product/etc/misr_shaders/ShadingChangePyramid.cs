#extension GL_OES_shader_image_atomic : require
layout(local_size_x = 256, local_size_y = 1, local_size_z = 1) in;

const ivec2 _116[5] = ivec2[](ivec2(0), ivec2(-1, 0), ivec2(1, 0), ivec2(0, -1), ivec2(0, 1));

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

layout(binding = 1, std140) uniform type_cbSPD
{
    uint mips;
    uint numWorkGroups;
    uvec2 workGroupOffset;
    uvec2 renderSize;
} cbSPD;

layout(binding = 1, rgba8) uniform writeonly highp image2D rw_spd_mip0;
layout(binding = 2, rgba8) uniform writeonly highp image2D rw_spd_mip1;
layout(binding = 3, rgba8) uniform writeonly highp image2D rw_spd_mip2;
layout(binding = 4, rgba8) uniform writeonly highp image2D rw_spd_mip3;
layout(binding = 5, rgba8) uniform writeonly highp image2D rw_spd_mip4;
layout(binding = 6, rgba8) uniform readonly highp image2D r_spd_mip5;
layout(binding = 6, rgba8) uniform writeonly highp image2D w_spd_mip5;
layout(binding = 0, r32ui) uniform highp uimage2D rw_spd_global_atomic;
uniform highp sampler2D SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler;

shared uint spdCounter;
shared float spdIntermediateR[16][16];
shared float spdIntermediateG[16][16];
shared float spdIntermediateB[16][16];
shared float spdIntermediateA[16][16];

float _122;

void main()
{
    uvec2 _198 = gl_WorkGroupID.xy + cbSPD.workGroupOffset;
    switch (0u)
    {
        default:
        {
            uint _225;
            uint _227;
            ivec2 _235;
            uint _201 = gl_LocalInvocationIndex % 64u;
            uint _217 = ((_201 & 1u) | (((_201 >> 2u) & 7u) & 4294967294u)) + (8u * ((gl_LocalInvocationIndex >> 6u) % 2u));
            uint _220 = ((((_201 >> 1u) & 3u) & 3u) | (((_201 >> 3u) & 7u) & 4294967292u)) + (8u * (gl_LocalInvocationIndex >> 7u));
            switch (0u)
            {
                default:
                {
                    ivec2 _224 = ivec2(_198 * uvec2(64u));
                    _225 = _217 * 2u;
                    int _226 = int(_225);
                    _227 = _220 * 2u;
                    int _228 = int(_227);
                    ivec2 _232 = ivec2(_198 * uvec2(32u));
                    int _233 = int(_217);
                    int _234 = int(_220);
                    _235 = ivec2(_233, _234);
                    uvec2 _237 = uvec2(_224 + ivec2(_226, _228));
                    ivec2 _243 = ivec2(vec2(ivec2(_237)));
                    int _248 = cbFSR3Upscaler.iRenderSize.x - 1;
                    ivec2 _251 = _243;
                    _251.x = max(0, min(_243.x, _248));
                    int _254 = cbFSR3Upscaler.iRenderSize.y - 1;
                    ivec2 _257 = _251;
                    _257.y = max(0, min(_243.y, _254));
                    vec2 _264 = vec2(cbFSR3Upscaler.iRenderSize);
                    vec2 _265 = (vec2(_257) + vec2(0.5)) / _264;
                    vec2 _268 = cbFSR3Upscaler.fJitter / _264;
                    float _142[5];
                    for (int _274 = 0; _274 < 5; )
                    {
                        ivec2 _281 = ivec2(floor((_265 + _268) * _264)) + _116[_274];
                        ivec2 _285 = _281;
                        _285.x = max(0, min(_281.x, _248));
                        ivec2 _289 = _285;
                        _289.y = max(0, min(_281.y, _254));
                        vec4 _295 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _296 = _295.x;
                        _142[_274] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_289)), int(0u)).x * ((_296 == 0.0) ? 1.0 : _296);
                        _142[_274] = pow(_142[_274], 1.0);
                        _142[_274] = max(_142[_274], 6.099999882280826568603515625e-05);
                        _274++;
                        continue;
                    }
                    vec2 _310 = vec2(cbFSR3Upscaler.iPreviousFrameRenderSize);
                    vec2 _311 = cbFSR3Upscaler.fPreviousFrameJitter / _310;
                    vec2 _313 = (_265 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_257)), int(0u)).xy;
                    float _314 = _313.x;
                    float _318 = _313.y;
                    bool _322 = ((_314 >= 0.0) && (_314 <= 1.0)) && ((_318 >= 0.0) && (_318 <= 1.0));
                    float _124[5];
                    if (_322)
                    {
                        for (int _329 = 0; _329 < 5; )
                        {
                            ivec2 _336 = ivec2(floor(_313 * _310)) + _116[_329];
                            ivec2 _342 = _336;
                            _342.x = max(0, min(_336.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _348 = _342;
                            _348.y = max(0, min(_336.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _357 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _358 = _357.x;
                            _124[_329] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_348)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_358 == 0.0) ? 1.0 : _358);
                            _124[_329] = pow(_124[_329], 1.0);
                            _124[_329] = max(_124[_329], 6.099999882280826568603515625e-05);
                            _329++;
                            continue;
                        }
                    }
                    float _550;
                    if (_322)
                    {
                        float _141[5] = _142;
                        float _140[5] = _124;
                        float _371 = _141[0];
                        float _373 = _141[3];
                        _141[3] = max(_141[0], _141[3]);
                        _141[0] = min(_371, _373);
                        float _379 = _141[1];
                        float _381 = _141[4];
                        _141[4] = max(_141[1], _141[4]);
                        _141[1] = min(_379, _381);
                        float _386 = _141[0];
                        float _388 = _141[2];
                        _141[2] = max(_141[0], _141[2]);
                        _141[0] = min(_386, _388);
                        float _393 = _141[1];
                        float _394 = _141[3];
                        _141[3] = max(_141[1], _141[3]);
                        _141[1] = min(_393, _394);
                        float _399 = _141[0];
                        float _400 = _141[1];
                        _141[1] = max(_141[0], _141[1]);
                        _141[0] = min(_399, _400);
                        float _405 = _141[2];
                        float _406 = _141[4];
                        _141[4] = max(_141[2], _141[4]);
                        _141[2] = min(_405, _406);
                        float _411 = _141[1];
                        float _412 = _141[2];
                        _141[2] = max(_141[1], _141[2]);
                        _141[1] = min(_411, _412);
                        float _417 = _141[3];
                        float _418 = _141[4];
                        _141[4] = max(_141[3], _141[4]);
                        _141[3] = min(_417, _418);
                        float _423 = _141[2];
                        float _424 = _141[3];
                        _141[3] = max(_141[2], _141[3]);
                        _141[2] = min(_423, _424);
                        float _430 = _140[0];
                        float _432 = _140[3];
                        _140[3] = max(_140[0], _140[3]);
                        _140[0] = min(_430, _432);
                        float _438 = _140[1];
                        float _440 = _140[4];
                        _140[4] = max(_140[1], _140[4]);
                        _140[1] = min(_438, _440);
                        float _445 = _140[0];
                        float _447 = _140[2];
                        _140[2] = max(_140[0], _140[2]);
                        _140[0] = min(_445, _447);
                        float _452 = _140[1];
                        float _453 = _140[3];
                        _140[3] = max(_140[1], _140[3]);
                        _140[1] = min(_452, _453);
                        float _458 = _140[0];
                        float _459 = _140[1];
                        _140[1] = max(_140[0], _140[1]);
                        _140[0] = min(_458, _459);
                        float _464 = _140[2];
                        float _465 = _140[4];
                        _140[4] = max(_140[2], _140[4]);
                        _140[2] = min(_464, _465);
                        float _470 = _140[1];
                        float _471 = _140[2];
                        _140[2] = max(_140[1], _140[2]);
                        _140[1] = min(_470, _471);
                        float _476 = _140[3];
                        float _477 = _140[4];
                        _140[4] = max(_140[3], _140[4]);
                        _140[3] = min(_476, _477);
                        float _482 = _140[2];
                        float _483 = _140[3];
                        _140[3] = max(_140[2], _140[3]);
                        _140[2] = min(_482, _483);
                        float _546;
                        if (min(_141[4], _140[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _500;
                            _500 = 65503.0;
                            int _496;
                            int _499;
                            float _501;
                            for (int _495 = 0, _498 = 0, _502 = 0; (_502 < 5) && (_500 < 65504.0); _495 = _496, _498 = _499, _500 = _501, _502++)
                            {
                                float _513 = _141[_498] - _140[_495];
                                if (abs(_513) > 6.099999882280826568603515625e-05)
                                {
                                    float _524 = max(_141[_498], _140[_495]);
                                    float _530 = float(int(sign(_513))) * (1.0 - ((_524 != 0.0) ? (min(_141[_498], _140[_495]) / _524) : 0.0));
                                    int _539 = _498 + int(_141[_498] < _140[_495]);
                                    _496 = _495 + int(_141[_539] >= _140[_495]);
                                    _499 = _539;
                                    _501 = (abs(_530) < abs(_500)) ? _530 : _500;
                                }
                                else
                                {
                                    _496 = _495;
                                    _499 = _498;
                                    _501 = 65504.0;
                                }
                            }
                            _546 = _500;
                        }
                        else
                        {
                            _546 = 65503.0;
                        }
                        _550 = _546 * float(_546 < 65503.0);
                    }
                    else
                    {
                        _550 = 0.0;
                    }
                    vec4 _551 = vec4(0.0);
                    _551.x = _550;
                    vec4 _557 = _551;
                    _557.y = (_550 != 0.0) ? float(int(sign(_550))) : 0.0;
                    vec4 _558 = _557;
                    _558.z = 1.0;
                    ivec2 _561 = ivec2(vec2(ivec2(_237 + uvec2(0u, 1u))));
                    ivec2 _565 = _561;
                    _565.x = max(0, min(_561.x, _248));
                    ivec2 _569 = _565;
                    _569.y = max(0, min(_561.y, _254));
                    vec2 _575 = (vec2(_569) + vec2(0.5)) / _264;
                    float _145[5];
                    for (int _581 = 0; _581 < 5; )
                    {
                        ivec2 _588 = ivec2(floor((_575 + _268) * _264)) + _116[_581];
                        ivec2 _592 = _588;
                        _592.x = max(0, min(_588.x, _248));
                        ivec2 _596 = _592;
                        _596.y = max(0, min(_588.y, _254));
                        vec4 _602 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _603 = _602.x;
                        _145[_581] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_596)), int(0u)).x * ((_603 == 0.0) ? 1.0 : _603);
                        _145[_581] = pow(_145[_581], 1.0);
                        _145[_581] = max(_145[_581], 6.099999882280826568603515625e-05);
                        _581++;
                        continue;
                    }
                    vec2 _614 = (_575 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_569)), int(0u)).xy;
                    float _615 = _614.x;
                    float _619 = _614.y;
                    bool _623 = ((_615 >= 0.0) && (_615 <= 1.0)) && ((_619 >= 0.0) && (_619 <= 1.0));
                    float _125[5];
                    if (_623)
                    {
                        for (int _630 = 0; _630 < 5; )
                        {
                            ivec2 _637 = ivec2(floor(_614 * _310)) + _116[_630];
                            ivec2 _643 = _637;
                            _643.x = max(0, min(_637.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _649 = _643;
                            _649.y = max(0, min(_637.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _658 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _659 = _658.x;
                            _125[_630] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_649)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_659 == 0.0) ? 1.0 : _659);
                            _125[_630] = pow(_125[_630], 1.0);
                            _125[_630] = max(_125[_630], 6.099999882280826568603515625e-05);
                            _630++;
                            continue;
                        }
                    }
                    float _851;
                    if (_623)
                    {
                        float _144[5] = _145;
                        float _143[5] = _125;
                        float _672 = _144[0];
                        float _674 = _144[3];
                        _144[3] = max(_144[0], _144[3]);
                        _144[0] = min(_672, _674);
                        float _680 = _144[1];
                        float _682 = _144[4];
                        _144[4] = max(_144[1], _144[4]);
                        _144[1] = min(_680, _682);
                        float _687 = _144[0];
                        float _689 = _144[2];
                        _144[2] = max(_144[0], _144[2]);
                        _144[0] = min(_687, _689);
                        float _694 = _144[1];
                        float _695 = _144[3];
                        _144[3] = max(_144[1], _144[3]);
                        _144[1] = min(_694, _695);
                        float _700 = _144[0];
                        float _701 = _144[1];
                        _144[1] = max(_144[0], _144[1]);
                        _144[0] = min(_700, _701);
                        float _706 = _144[2];
                        float _707 = _144[4];
                        _144[4] = max(_144[2], _144[4]);
                        _144[2] = min(_706, _707);
                        float _712 = _144[1];
                        float _713 = _144[2];
                        _144[2] = max(_144[1], _144[2]);
                        _144[1] = min(_712, _713);
                        float _718 = _144[3];
                        float _719 = _144[4];
                        _144[4] = max(_144[3], _144[4]);
                        _144[3] = min(_718, _719);
                        float _724 = _144[2];
                        float _725 = _144[3];
                        _144[3] = max(_144[2], _144[3]);
                        _144[2] = min(_724, _725);
                        float _731 = _143[0];
                        float _733 = _143[3];
                        _143[3] = max(_143[0], _143[3]);
                        _143[0] = min(_731, _733);
                        float _739 = _143[1];
                        float _741 = _143[4];
                        _143[4] = max(_143[1], _143[4]);
                        _143[1] = min(_739, _741);
                        float _746 = _143[0];
                        float _748 = _143[2];
                        _143[2] = max(_143[0], _143[2]);
                        _143[0] = min(_746, _748);
                        float _753 = _143[1];
                        float _754 = _143[3];
                        _143[3] = max(_143[1], _143[3]);
                        _143[1] = min(_753, _754);
                        float _759 = _143[0];
                        float _760 = _143[1];
                        _143[1] = max(_143[0], _143[1]);
                        _143[0] = min(_759, _760);
                        float _765 = _143[2];
                        float _766 = _143[4];
                        _143[4] = max(_143[2], _143[4]);
                        _143[2] = min(_765, _766);
                        float _771 = _143[1];
                        float _772 = _143[2];
                        _143[2] = max(_143[1], _143[2]);
                        _143[1] = min(_771, _772);
                        float _777 = _143[3];
                        float _778 = _143[4];
                        _143[4] = max(_143[3], _143[4]);
                        _143[3] = min(_777, _778);
                        float _783 = _143[2];
                        float _784 = _143[3];
                        _143[3] = max(_143[2], _143[3]);
                        _143[2] = min(_783, _784);
                        float _847;
                        if (min(_144[4], _143[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _801;
                            _801 = 65503.0;
                            int _797;
                            int _800;
                            float _802;
                            for (int _796 = 0, _799 = 0, _803 = 0; (_803 < 5) && (_801 < 65504.0); _796 = _797, _799 = _800, _801 = _802, _803++)
                            {
                                float _814 = _144[_799] - _143[_796];
                                if (abs(_814) > 6.099999882280826568603515625e-05)
                                {
                                    float _825 = max(_144[_799], _143[_796]);
                                    float _831 = float(int(sign(_814))) * (1.0 - ((_825 != 0.0) ? (min(_144[_799], _143[_796]) / _825) : 0.0));
                                    int _840 = _799 + int(_144[_799] < _143[_796]);
                                    _797 = _796 + int(_144[_840] >= _143[_796]);
                                    _800 = _840;
                                    _802 = (abs(_831) < abs(_801)) ? _831 : _801;
                                }
                                else
                                {
                                    _797 = _796;
                                    _800 = _799;
                                    _802 = 65504.0;
                                }
                            }
                            _847 = _801;
                        }
                        else
                        {
                            _847 = 65503.0;
                        }
                        _851 = _847 * float(_847 < 65503.0);
                    }
                    else
                    {
                        _851 = 0.0;
                    }
                    vec4 _852 = vec4(0.0);
                    _852.x = _851;
                    vec4 _858 = _852;
                    _858.y = (_851 != 0.0) ? float(int(sign(_851))) : 0.0;
                    vec4 _859 = _858;
                    _859.z = 1.0;
                    ivec2 _862 = ivec2(vec2(ivec2(_237 + uvec2(1u, 0u))));
                    ivec2 _866 = _862;
                    _866.x = max(0, min(_862.x, _248));
                    ivec2 _870 = _866;
                    _870.y = max(0, min(_862.y, _254));
                    vec2 _876 = (vec2(_870) + vec2(0.5)) / _264;
                    float _148[5];
                    for (int _882 = 0; _882 < 5; )
                    {
                        ivec2 _889 = ivec2(floor((_876 + _268) * _264)) + _116[_882];
                        ivec2 _893 = _889;
                        _893.x = max(0, min(_889.x, _248));
                        ivec2 _897 = _893;
                        _897.y = max(0, min(_889.y, _254));
                        vec4 _903 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _904 = _903.x;
                        _148[_882] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_897)), int(0u)).x * ((_904 == 0.0) ? 1.0 : _904);
                        _148[_882] = pow(_148[_882], 1.0);
                        _148[_882] = max(_148[_882], 6.099999882280826568603515625e-05);
                        _882++;
                        continue;
                    }
                    vec2 _915 = (_876 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_870)), int(0u)).xy;
                    float _916 = _915.x;
                    float _920 = _915.y;
                    bool _924 = ((_916 >= 0.0) && (_916 <= 1.0)) && ((_920 >= 0.0) && (_920 <= 1.0));
                    float _126[5];
                    if (_924)
                    {
                        for (int _931 = 0; _931 < 5; )
                        {
                            ivec2 _938 = ivec2(floor(_915 * _310)) + _116[_931];
                            ivec2 _944 = _938;
                            _944.x = max(0, min(_938.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _950 = _944;
                            _950.y = max(0, min(_938.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _959 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _960 = _959.x;
                            _126[_931] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_950)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_960 == 0.0) ? 1.0 : _960);
                            _126[_931] = pow(_126[_931], 1.0);
                            _126[_931] = max(_126[_931], 6.099999882280826568603515625e-05);
                            _931++;
                            continue;
                        }
                    }
                    float _1152;
                    if (_924)
                    {
                        float _147[5] = _148;
                        float _146[5] = _126;
                        float _973 = _147[0];
                        float _975 = _147[3];
                        _147[3] = max(_147[0], _147[3]);
                        _147[0] = min(_973, _975);
                        float _981 = _147[1];
                        float _983 = _147[4];
                        _147[4] = max(_147[1], _147[4]);
                        _147[1] = min(_981, _983);
                        float _988 = _147[0];
                        float _990 = _147[2];
                        _147[2] = max(_147[0], _147[2]);
                        _147[0] = min(_988, _990);
                        float _995 = _147[1];
                        float _996 = _147[3];
                        _147[3] = max(_147[1], _147[3]);
                        _147[1] = min(_995, _996);
                        float _1001 = _147[0];
                        float _1002 = _147[1];
                        _147[1] = max(_147[0], _147[1]);
                        _147[0] = min(_1001, _1002);
                        float _1007 = _147[2];
                        float _1008 = _147[4];
                        _147[4] = max(_147[2], _147[4]);
                        _147[2] = min(_1007, _1008);
                        float _1013 = _147[1];
                        float _1014 = _147[2];
                        _147[2] = max(_147[1], _147[2]);
                        _147[1] = min(_1013, _1014);
                        float _1019 = _147[3];
                        float _1020 = _147[4];
                        _147[4] = max(_147[3], _147[4]);
                        _147[3] = min(_1019, _1020);
                        float _1025 = _147[2];
                        float _1026 = _147[3];
                        _147[3] = max(_147[2], _147[3]);
                        _147[2] = min(_1025, _1026);
                        float _1032 = _146[0];
                        float _1034 = _146[3];
                        _146[3] = max(_146[0], _146[3]);
                        _146[0] = min(_1032, _1034);
                        float _1040 = _146[1];
                        float _1042 = _146[4];
                        _146[4] = max(_146[1], _146[4]);
                        _146[1] = min(_1040, _1042);
                        float _1047 = _146[0];
                        float _1049 = _146[2];
                        _146[2] = max(_146[0], _146[2]);
                        _146[0] = min(_1047, _1049);
                        float _1054 = _146[1];
                        float _1055 = _146[3];
                        _146[3] = max(_146[1], _146[3]);
                        _146[1] = min(_1054, _1055);
                        float _1060 = _146[0];
                        float _1061 = _146[1];
                        _146[1] = max(_146[0], _146[1]);
                        _146[0] = min(_1060, _1061);
                        float _1066 = _146[2];
                        float _1067 = _146[4];
                        _146[4] = max(_146[2], _146[4]);
                        _146[2] = min(_1066, _1067);
                        float _1072 = _146[1];
                        float _1073 = _146[2];
                        _146[2] = max(_146[1], _146[2]);
                        _146[1] = min(_1072, _1073);
                        float _1078 = _146[3];
                        float _1079 = _146[4];
                        _146[4] = max(_146[3], _146[4]);
                        _146[3] = min(_1078, _1079);
                        float _1084 = _146[2];
                        float _1085 = _146[3];
                        _146[3] = max(_146[2], _146[3]);
                        _146[2] = min(_1084, _1085);
                        float _1148;
                        if (min(_147[4], _146[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _1102;
                            _1102 = 65503.0;
                            int _1098;
                            int _1101;
                            float _1103;
                            for (int _1097 = 0, _1100 = 0, _1104 = 0; (_1104 < 5) && (_1102 < 65504.0); _1097 = _1098, _1100 = _1101, _1102 = _1103, _1104++)
                            {
                                float _1115 = _147[_1100] - _146[_1097];
                                if (abs(_1115) > 6.099999882280826568603515625e-05)
                                {
                                    float _1126 = max(_147[_1100], _146[_1097]);
                                    float _1132 = float(int(sign(_1115))) * (1.0 - ((_1126 != 0.0) ? (min(_147[_1100], _146[_1097]) / _1126) : 0.0));
                                    int _1141 = _1100 + int(_147[_1100] < _146[_1097]);
                                    _1098 = _1097 + int(_147[_1141] >= _146[_1097]);
                                    _1101 = _1141;
                                    _1103 = (abs(_1132) < abs(_1102)) ? _1132 : _1102;
                                }
                                else
                                {
                                    _1098 = _1097;
                                    _1101 = _1100;
                                    _1103 = 65504.0;
                                }
                            }
                            _1148 = _1102;
                        }
                        else
                        {
                            _1148 = 65503.0;
                        }
                        _1152 = _1148 * float(_1148 < 65503.0);
                    }
                    else
                    {
                        _1152 = 0.0;
                    }
                    vec4 _1153 = vec4(0.0);
                    _1153.x = _1152;
                    vec4 _1159 = _1153;
                    _1159.y = (_1152 != 0.0) ? float(int(sign(_1152))) : 0.0;
                    vec4 _1160 = _1159;
                    _1160.z = 1.0;
                    ivec2 _1163 = ivec2(vec2(ivec2(_237 + uvec2(1u))));
                    ivec2 _1167 = _1163;
                    _1167.x = max(0, min(_1163.x, _248));
                    ivec2 _1171 = _1167;
                    _1171.y = max(0, min(_1163.y, _254));
                    vec2 _1177 = (vec2(_1171) + vec2(0.5)) / _264;
                    float _151[5];
                    for (int _1183 = 0; _1183 < 5; )
                    {
                        ivec2 _1190 = ivec2(floor((_1177 + _268) * _264)) + _116[_1183];
                        ivec2 _1194 = _1190;
                        _1194.x = max(0, min(_1190.x, _248));
                        ivec2 _1198 = _1194;
                        _1198.y = max(0, min(_1190.y, _254));
                        vec4 _1204 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _1205 = _1204.x;
                        _151[_1183] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_1198)), int(0u)).x * ((_1205 == 0.0) ? 1.0 : _1205);
                        _151[_1183] = pow(_151[_1183], 1.0);
                        _151[_1183] = max(_151[_1183], 6.099999882280826568603515625e-05);
                        _1183++;
                        continue;
                    }
                    vec2 _1216 = (_1177 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_1171)), int(0u)).xy;
                    float _1217 = _1216.x;
                    float _1221 = _1216.y;
                    bool _1225 = ((_1217 >= 0.0) && (_1217 <= 1.0)) && ((_1221 >= 0.0) && (_1221 <= 1.0));
                    float _127[5];
                    if (_1225)
                    {
                        for (int _1232 = 0; _1232 < 5; )
                        {
                            ivec2 _1239 = ivec2(floor(_1216 * _310)) + _116[_1232];
                            ivec2 _1245 = _1239;
                            _1245.x = max(0, min(_1239.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _1251 = _1245;
                            _1251.y = max(0, min(_1239.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _1260 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _1261 = _1260.x;
                            _127[_1232] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_1251)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_1261 == 0.0) ? 1.0 : _1261);
                            _127[_1232] = pow(_127[_1232], 1.0);
                            _127[_1232] = max(_127[_1232], 6.099999882280826568603515625e-05);
                            _1232++;
                            continue;
                        }
                    }
                    float _1453;
                    if (_1225)
                    {
                        float _150[5] = _151;
                        float _149[5] = _127;
                        float _1274 = _150[0];
                        float _1276 = _150[3];
                        _150[3] = max(_150[0], _150[3]);
                        _150[0] = min(_1274, _1276);
                        float _1282 = _150[1];
                        float _1284 = _150[4];
                        _150[4] = max(_150[1], _150[4]);
                        _150[1] = min(_1282, _1284);
                        float _1289 = _150[0];
                        float _1291 = _150[2];
                        _150[2] = max(_150[0], _150[2]);
                        _150[0] = min(_1289, _1291);
                        float _1296 = _150[1];
                        float _1297 = _150[3];
                        _150[3] = max(_150[1], _150[3]);
                        _150[1] = min(_1296, _1297);
                        float _1302 = _150[0];
                        float _1303 = _150[1];
                        _150[1] = max(_150[0], _150[1]);
                        _150[0] = min(_1302, _1303);
                        float _1308 = _150[2];
                        float _1309 = _150[4];
                        _150[4] = max(_150[2], _150[4]);
                        _150[2] = min(_1308, _1309);
                        float _1314 = _150[1];
                        float _1315 = _150[2];
                        _150[2] = max(_150[1], _150[2]);
                        _150[1] = min(_1314, _1315);
                        float _1320 = _150[3];
                        float _1321 = _150[4];
                        _150[4] = max(_150[3], _150[4]);
                        _150[3] = min(_1320, _1321);
                        float _1326 = _150[2];
                        float _1327 = _150[3];
                        _150[3] = max(_150[2], _150[3]);
                        _150[2] = min(_1326, _1327);
                        float _1333 = _149[0];
                        float _1335 = _149[3];
                        _149[3] = max(_149[0], _149[3]);
                        _149[0] = min(_1333, _1335);
                        float _1341 = _149[1];
                        float _1343 = _149[4];
                        _149[4] = max(_149[1], _149[4]);
                        _149[1] = min(_1341, _1343);
                        float _1348 = _149[0];
                        float _1350 = _149[2];
                        _149[2] = max(_149[0], _149[2]);
                        _149[0] = min(_1348, _1350);
                        float _1355 = _149[1];
                        float _1356 = _149[3];
                        _149[3] = max(_149[1], _149[3]);
                        _149[1] = min(_1355, _1356);
                        float _1361 = _149[0];
                        float _1362 = _149[1];
                        _149[1] = max(_149[0], _149[1]);
                        _149[0] = min(_1361, _1362);
                        float _1367 = _149[2];
                        float _1368 = _149[4];
                        _149[4] = max(_149[2], _149[4]);
                        _149[2] = min(_1367, _1368);
                        float _1373 = _149[1];
                        float _1374 = _149[2];
                        _149[2] = max(_149[1], _149[2]);
                        _149[1] = min(_1373, _1374);
                        float _1379 = _149[3];
                        float _1380 = _149[4];
                        _149[4] = max(_149[3], _149[4]);
                        _149[3] = min(_1379, _1380);
                        float _1385 = _149[2];
                        float _1386 = _149[3];
                        _149[3] = max(_149[2], _149[3]);
                        _149[2] = min(_1385, _1386);
                        float _1449;
                        if (min(_150[4], _149[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _1403;
                            _1403 = 65503.0;
                            int _1399;
                            int _1402;
                            float _1404;
                            for (int _1398 = 0, _1401 = 0, _1405 = 0; (_1405 < 5) && (_1403 < 65504.0); _1398 = _1399, _1401 = _1402, _1403 = _1404, _1405++)
                            {
                                float _1416 = _150[_1401] - _149[_1398];
                                if (abs(_1416) > 6.099999882280826568603515625e-05)
                                {
                                    float _1427 = max(_150[_1401], _149[_1398]);
                                    float _1433 = float(int(sign(_1416))) * (1.0 - ((_1427 != 0.0) ? (min(_150[_1401], _149[_1398]) / _1427) : 0.0));
                                    int _1442 = _1401 + int(_150[_1401] < _149[_1398]);
                                    _1399 = _1398 + int(_150[_1442] >= _149[_1398]);
                                    _1402 = _1442;
                                    _1404 = (abs(_1433) < abs(_1403)) ? _1433 : _1403;
                                }
                                else
                                {
                                    _1399 = _1398;
                                    _1402 = _1401;
                                    _1404 = 65504.0;
                                }
                            }
                            _1449 = _1403;
                        }
                        else
                        {
                            _1449 = 65503.0;
                        }
                        _1453 = _1449 * float(_1449 < 65503.0);
                    }
                    else
                    {
                        _1453 = 0.0;
                    }
                    vec4 _1454 = vec4(0.0);
                    _1454.x = _1453;
                    vec4 _1460 = _1454;
                    _1460.y = (_1453 != 0.0) ? float(int(sign(_1453))) : 0.0;
                    vec4 _1461 = _1460;
                    _1461.z = 1.0;
                    vec4 _188[4];
                    _188[0] = (((_558 + _859) + _1160) + _1461) * 0.25;
                    imageStore(rw_spd_mip0, ivec2(uvec2(_232 + _235)), vec4(_188[0].xy, 0.0, 0.0));
                    int _1474 = int(_225 + 32u);
                    int _1478 = int(_217 + 16u);
                    uvec2 _1481 = uvec2(_224 + ivec2(_1474, _228));
                    ivec2 _1487 = ivec2(vec2(ivec2(_1481)));
                    ivec2 _1491 = _1487;
                    _1491.x = max(0, min(_1487.x, _248));
                    ivec2 _1495 = _1491;
                    _1495.y = max(0, min(_1487.y, _254));
                    vec2 _1501 = (vec2(_1495) + vec2(0.5)) / _264;
                    float _154[5];
                    for (int _1507 = 0; _1507 < 5; )
                    {
                        ivec2 _1514 = ivec2(floor((_1501 + _268) * _264)) + _116[_1507];
                        ivec2 _1518 = _1514;
                        _1518.x = max(0, min(_1514.x, _248));
                        ivec2 _1522 = _1518;
                        _1522.y = max(0, min(_1514.y, _254));
                        vec4 _1528 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _1529 = _1528.x;
                        _154[_1507] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_1522)), int(0u)).x * ((_1529 == 0.0) ? 1.0 : _1529);
                        _154[_1507] = pow(_154[_1507], 1.0);
                        _154[_1507] = max(_154[_1507], 6.099999882280826568603515625e-05);
                        _1507++;
                        continue;
                    }
                    vec2 _1540 = (_1501 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_1495)), int(0u)).xy;
                    float _1541 = _1540.x;
                    float _1545 = _1540.y;
                    bool _1549 = ((_1541 >= 0.0) && (_1541 <= 1.0)) && ((_1545 >= 0.0) && (_1545 <= 1.0));
                    float _128[5];
                    if (_1549)
                    {
                        for (int _1556 = 0; _1556 < 5; )
                        {
                            ivec2 _1563 = ivec2(floor(_1540 * _310)) + _116[_1556];
                            ivec2 _1569 = _1563;
                            _1569.x = max(0, min(_1563.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _1575 = _1569;
                            _1575.y = max(0, min(_1563.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _1584 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _1585 = _1584.x;
                            _128[_1556] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_1575)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_1585 == 0.0) ? 1.0 : _1585);
                            _128[_1556] = pow(_128[_1556], 1.0);
                            _128[_1556] = max(_128[_1556], 6.099999882280826568603515625e-05);
                            _1556++;
                            continue;
                        }
                    }
                    float _1777;
                    if (_1549)
                    {
                        float _153[5] = _154;
                        float _152[5] = _128;
                        float _1598 = _153[0];
                        float _1600 = _153[3];
                        _153[3] = max(_153[0], _153[3]);
                        _153[0] = min(_1598, _1600);
                        float _1606 = _153[1];
                        float _1608 = _153[4];
                        _153[4] = max(_153[1], _153[4]);
                        _153[1] = min(_1606, _1608);
                        float _1613 = _153[0];
                        float _1615 = _153[2];
                        _153[2] = max(_153[0], _153[2]);
                        _153[0] = min(_1613, _1615);
                        float _1620 = _153[1];
                        float _1621 = _153[3];
                        _153[3] = max(_153[1], _153[3]);
                        _153[1] = min(_1620, _1621);
                        float _1626 = _153[0];
                        float _1627 = _153[1];
                        _153[1] = max(_153[0], _153[1]);
                        _153[0] = min(_1626, _1627);
                        float _1632 = _153[2];
                        float _1633 = _153[4];
                        _153[4] = max(_153[2], _153[4]);
                        _153[2] = min(_1632, _1633);
                        float _1638 = _153[1];
                        float _1639 = _153[2];
                        _153[2] = max(_153[1], _153[2]);
                        _153[1] = min(_1638, _1639);
                        float _1644 = _153[3];
                        float _1645 = _153[4];
                        _153[4] = max(_153[3], _153[4]);
                        _153[3] = min(_1644, _1645);
                        float _1650 = _153[2];
                        float _1651 = _153[3];
                        _153[3] = max(_153[2], _153[3]);
                        _153[2] = min(_1650, _1651);
                        float _1657 = _152[0];
                        float _1659 = _152[3];
                        _152[3] = max(_152[0], _152[3]);
                        _152[0] = min(_1657, _1659);
                        float _1665 = _152[1];
                        float _1667 = _152[4];
                        _152[4] = max(_152[1], _152[4]);
                        _152[1] = min(_1665, _1667);
                        float _1672 = _152[0];
                        float _1674 = _152[2];
                        _152[2] = max(_152[0], _152[2]);
                        _152[0] = min(_1672, _1674);
                        float _1679 = _152[1];
                        float _1680 = _152[3];
                        _152[3] = max(_152[1], _152[3]);
                        _152[1] = min(_1679, _1680);
                        float _1685 = _152[0];
                        float _1686 = _152[1];
                        _152[1] = max(_152[0], _152[1]);
                        _152[0] = min(_1685, _1686);
                        float _1691 = _152[2];
                        float _1692 = _152[4];
                        _152[4] = max(_152[2], _152[4]);
                        _152[2] = min(_1691, _1692);
                        float _1697 = _152[1];
                        float _1698 = _152[2];
                        _152[2] = max(_152[1], _152[2]);
                        _152[1] = min(_1697, _1698);
                        float _1703 = _152[3];
                        float _1704 = _152[4];
                        _152[4] = max(_152[3], _152[4]);
                        _152[3] = min(_1703, _1704);
                        float _1709 = _152[2];
                        float _1710 = _152[3];
                        _152[3] = max(_152[2], _152[3]);
                        _152[2] = min(_1709, _1710);
                        float _1773;
                        if (min(_153[4], _152[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _1727;
                            _1727 = 65503.0;
                            int _1723;
                            int _1726;
                            float _1728;
                            for (int _1722 = 0, _1725 = 0, _1729 = 0; (_1729 < 5) && (_1727 < 65504.0); _1722 = _1723, _1725 = _1726, _1727 = _1728, _1729++)
                            {
                                float _1740 = _153[_1725] - _152[_1722];
                                if (abs(_1740) > 6.099999882280826568603515625e-05)
                                {
                                    float _1751 = max(_153[_1725], _152[_1722]);
                                    float _1757 = float(int(sign(_1740))) * (1.0 - ((_1751 != 0.0) ? (min(_153[_1725], _152[_1722]) / _1751) : 0.0));
                                    int _1766 = _1725 + int(_153[_1725] < _152[_1722]);
                                    _1723 = _1722 + int(_153[_1766] >= _152[_1722]);
                                    _1726 = _1766;
                                    _1728 = (abs(_1757) < abs(_1727)) ? _1757 : _1727;
                                }
                                else
                                {
                                    _1723 = _1722;
                                    _1726 = _1725;
                                    _1728 = 65504.0;
                                }
                            }
                            _1773 = _1727;
                        }
                        else
                        {
                            _1773 = 65503.0;
                        }
                        _1777 = _1773 * float(_1773 < 65503.0);
                    }
                    else
                    {
                        _1777 = 0.0;
                    }
                    vec4 _1778 = vec4(0.0);
                    _1778.x = _1777;
                    vec4 _1784 = _1778;
                    _1784.y = (_1777 != 0.0) ? float(int(sign(_1777))) : 0.0;
                    vec4 _1785 = _1784;
                    _1785.z = 1.0;
                    ivec2 _1788 = ivec2(vec2(ivec2(_1481 + uvec2(0u, 1u))));
                    ivec2 _1792 = _1788;
                    _1792.x = max(0, min(_1788.x, _248));
                    ivec2 _1796 = _1792;
                    _1796.y = max(0, min(_1788.y, _254));
                    vec2 _1802 = (vec2(_1796) + vec2(0.5)) / _264;
                    float _157[5];
                    for (int _1808 = 0; _1808 < 5; )
                    {
                        ivec2 _1815 = ivec2(floor((_1802 + _268) * _264)) + _116[_1808];
                        ivec2 _1819 = _1815;
                        _1819.x = max(0, min(_1815.x, _248));
                        ivec2 _1823 = _1819;
                        _1823.y = max(0, min(_1815.y, _254));
                        vec4 _1829 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _1830 = _1829.x;
                        _157[_1808] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_1823)), int(0u)).x * ((_1830 == 0.0) ? 1.0 : _1830);
                        _157[_1808] = pow(_157[_1808], 1.0);
                        _157[_1808] = max(_157[_1808], 6.099999882280826568603515625e-05);
                        _1808++;
                        continue;
                    }
                    vec2 _1841 = (_1802 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_1796)), int(0u)).xy;
                    float _1842 = _1841.x;
                    float _1846 = _1841.y;
                    bool _1850 = ((_1842 >= 0.0) && (_1842 <= 1.0)) && ((_1846 >= 0.0) && (_1846 <= 1.0));
                    float _129[5];
                    if (_1850)
                    {
                        for (int _1857 = 0; _1857 < 5; )
                        {
                            ivec2 _1864 = ivec2(floor(_1841 * _310)) + _116[_1857];
                            ivec2 _1870 = _1864;
                            _1870.x = max(0, min(_1864.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _1876 = _1870;
                            _1876.y = max(0, min(_1864.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _1885 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _1886 = _1885.x;
                            _129[_1857] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_1876)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_1886 == 0.0) ? 1.0 : _1886);
                            _129[_1857] = pow(_129[_1857], 1.0);
                            _129[_1857] = max(_129[_1857], 6.099999882280826568603515625e-05);
                            _1857++;
                            continue;
                        }
                    }
                    float _2078;
                    if (_1850)
                    {
                        float _156[5] = _157;
                        float _155[5] = _129;
                        float _1899 = _156[0];
                        float _1901 = _156[3];
                        _156[3] = max(_156[0], _156[3]);
                        _156[0] = min(_1899, _1901);
                        float _1907 = _156[1];
                        float _1909 = _156[4];
                        _156[4] = max(_156[1], _156[4]);
                        _156[1] = min(_1907, _1909);
                        float _1914 = _156[0];
                        float _1916 = _156[2];
                        _156[2] = max(_156[0], _156[2]);
                        _156[0] = min(_1914, _1916);
                        float _1921 = _156[1];
                        float _1922 = _156[3];
                        _156[3] = max(_156[1], _156[3]);
                        _156[1] = min(_1921, _1922);
                        float _1927 = _156[0];
                        float _1928 = _156[1];
                        _156[1] = max(_156[0], _156[1]);
                        _156[0] = min(_1927, _1928);
                        float _1933 = _156[2];
                        float _1934 = _156[4];
                        _156[4] = max(_156[2], _156[4]);
                        _156[2] = min(_1933, _1934);
                        float _1939 = _156[1];
                        float _1940 = _156[2];
                        _156[2] = max(_156[1], _156[2]);
                        _156[1] = min(_1939, _1940);
                        float _1945 = _156[3];
                        float _1946 = _156[4];
                        _156[4] = max(_156[3], _156[4]);
                        _156[3] = min(_1945, _1946);
                        float _1951 = _156[2];
                        float _1952 = _156[3];
                        _156[3] = max(_156[2], _156[3]);
                        _156[2] = min(_1951, _1952);
                        float _1958 = _155[0];
                        float _1960 = _155[3];
                        _155[3] = max(_155[0], _155[3]);
                        _155[0] = min(_1958, _1960);
                        float _1966 = _155[1];
                        float _1968 = _155[4];
                        _155[4] = max(_155[1], _155[4]);
                        _155[1] = min(_1966, _1968);
                        float _1973 = _155[0];
                        float _1975 = _155[2];
                        _155[2] = max(_155[0], _155[2]);
                        _155[0] = min(_1973, _1975);
                        float _1980 = _155[1];
                        float _1981 = _155[3];
                        _155[3] = max(_155[1], _155[3]);
                        _155[1] = min(_1980, _1981);
                        float _1986 = _155[0];
                        float _1987 = _155[1];
                        _155[1] = max(_155[0], _155[1]);
                        _155[0] = min(_1986, _1987);
                        float _1992 = _155[2];
                        float _1993 = _155[4];
                        _155[4] = max(_155[2], _155[4]);
                        _155[2] = min(_1992, _1993);
                        float _1998 = _155[1];
                        float _1999 = _155[2];
                        _155[2] = max(_155[1], _155[2]);
                        _155[1] = min(_1998, _1999);
                        float _2004 = _155[3];
                        float _2005 = _155[4];
                        _155[4] = max(_155[3], _155[4]);
                        _155[3] = min(_2004, _2005);
                        float _2010 = _155[2];
                        float _2011 = _155[3];
                        _155[3] = max(_155[2], _155[3]);
                        _155[2] = min(_2010, _2011);
                        float _2074;
                        if (min(_156[4], _155[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _2028;
                            _2028 = 65503.0;
                            int _2024;
                            int _2027;
                            float _2029;
                            for (int _2023 = 0, _2026 = 0, _2030 = 0; (_2030 < 5) && (_2028 < 65504.0); _2023 = _2024, _2026 = _2027, _2028 = _2029, _2030++)
                            {
                                float _2041 = _156[_2026] - _155[_2023];
                                if (abs(_2041) > 6.099999882280826568603515625e-05)
                                {
                                    float _2052 = max(_156[_2026], _155[_2023]);
                                    float _2058 = float(int(sign(_2041))) * (1.0 - ((_2052 != 0.0) ? (min(_156[_2026], _155[_2023]) / _2052) : 0.0));
                                    int _2067 = _2026 + int(_156[_2026] < _155[_2023]);
                                    _2024 = _2023 + int(_156[_2067] >= _155[_2023]);
                                    _2027 = _2067;
                                    _2029 = (abs(_2058) < abs(_2028)) ? _2058 : _2028;
                                }
                                else
                                {
                                    _2024 = _2023;
                                    _2027 = _2026;
                                    _2029 = 65504.0;
                                }
                            }
                            _2074 = _2028;
                        }
                        else
                        {
                            _2074 = 65503.0;
                        }
                        _2078 = _2074 * float(_2074 < 65503.0);
                    }
                    else
                    {
                        _2078 = 0.0;
                    }
                    vec4 _2079 = vec4(0.0);
                    _2079.x = _2078;
                    vec4 _2085 = _2079;
                    _2085.y = (_2078 != 0.0) ? float(int(sign(_2078))) : 0.0;
                    vec4 _2086 = _2085;
                    _2086.z = 1.0;
                    ivec2 _2089 = ivec2(vec2(ivec2(_1481 + uvec2(1u, 0u))));
                    ivec2 _2093 = _2089;
                    _2093.x = max(0, min(_2089.x, _248));
                    ivec2 _2097 = _2093;
                    _2097.y = max(0, min(_2089.y, _254));
                    vec2 _2103 = (vec2(_2097) + vec2(0.5)) / _264;
                    float _160[5];
                    for (int _2109 = 0; _2109 < 5; )
                    {
                        ivec2 _2116 = ivec2(floor((_2103 + _268) * _264)) + _116[_2109];
                        ivec2 _2120 = _2116;
                        _2120.x = max(0, min(_2116.x, _248));
                        ivec2 _2124 = _2120;
                        _2124.y = max(0, min(_2116.y, _254));
                        vec4 _2130 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _2131 = _2130.x;
                        _160[_2109] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_2124)), int(0u)).x * ((_2131 == 0.0) ? 1.0 : _2131);
                        _160[_2109] = pow(_160[_2109], 1.0);
                        _160[_2109] = max(_160[_2109], 6.099999882280826568603515625e-05);
                        _2109++;
                        continue;
                    }
                    vec2 _2142 = (_2103 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_2097)), int(0u)).xy;
                    float _2143 = _2142.x;
                    float _2147 = _2142.y;
                    bool _2151 = ((_2143 >= 0.0) && (_2143 <= 1.0)) && ((_2147 >= 0.0) && (_2147 <= 1.0));
                    float _130[5];
                    if (_2151)
                    {
                        for (int _2158 = 0; _2158 < 5; )
                        {
                            ivec2 _2165 = ivec2(floor(_2142 * _310)) + _116[_2158];
                            ivec2 _2171 = _2165;
                            _2171.x = max(0, min(_2165.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _2177 = _2171;
                            _2177.y = max(0, min(_2165.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _2186 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _2187 = _2186.x;
                            _130[_2158] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_2177)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_2187 == 0.0) ? 1.0 : _2187);
                            _130[_2158] = pow(_130[_2158], 1.0);
                            _130[_2158] = max(_130[_2158], 6.099999882280826568603515625e-05);
                            _2158++;
                            continue;
                        }
                    }
                    float _2379;
                    if (_2151)
                    {
                        float _159[5] = _160;
                        float _158[5] = _130;
                        float _2200 = _159[0];
                        float _2202 = _159[3];
                        _159[3] = max(_159[0], _159[3]);
                        _159[0] = min(_2200, _2202);
                        float _2208 = _159[1];
                        float _2210 = _159[4];
                        _159[4] = max(_159[1], _159[4]);
                        _159[1] = min(_2208, _2210);
                        float _2215 = _159[0];
                        float _2217 = _159[2];
                        _159[2] = max(_159[0], _159[2]);
                        _159[0] = min(_2215, _2217);
                        float _2222 = _159[1];
                        float _2223 = _159[3];
                        _159[3] = max(_159[1], _159[3]);
                        _159[1] = min(_2222, _2223);
                        float _2228 = _159[0];
                        float _2229 = _159[1];
                        _159[1] = max(_159[0], _159[1]);
                        _159[0] = min(_2228, _2229);
                        float _2234 = _159[2];
                        float _2235 = _159[4];
                        _159[4] = max(_159[2], _159[4]);
                        _159[2] = min(_2234, _2235);
                        float _2240 = _159[1];
                        float _2241 = _159[2];
                        _159[2] = max(_159[1], _159[2]);
                        _159[1] = min(_2240, _2241);
                        float _2246 = _159[3];
                        float _2247 = _159[4];
                        _159[4] = max(_159[3], _159[4]);
                        _159[3] = min(_2246, _2247);
                        float _2252 = _159[2];
                        float _2253 = _159[3];
                        _159[3] = max(_159[2], _159[3]);
                        _159[2] = min(_2252, _2253);
                        float _2259 = _158[0];
                        float _2261 = _158[3];
                        _158[3] = max(_158[0], _158[3]);
                        _158[0] = min(_2259, _2261);
                        float _2267 = _158[1];
                        float _2269 = _158[4];
                        _158[4] = max(_158[1], _158[4]);
                        _158[1] = min(_2267, _2269);
                        float _2274 = _158[0];
                        float _2276 = _158[2];
                        _158[2] = max(_158[0], _158[2]);
                        _158[0] = min(_2274, _2276);
                        float _2281 = _158[1];
                        float _2282 = _158[3];
                        _158[3] = max(_158[1], _158[3]);
                        _158[1] = min(_2281, _2282);
                        float _2287 = _158[0];
                        float _2288 = _158[1];
                        _158[1] = max(_158[0], _158[1]);
                        _158[0] = min(_2287, _2288);
                        float _2293 = _158[2];
                        float _2294 = _158[4];
                        _158[4] = max(_158[2], _158[4]);
                        _158[2] = min(_2293, _2294);
                        float _2299 = _158[1];
                        float _2300 = _158[2];
                        _158[2] = max(_158[1], _158[2]);
                        _158[1] = min(_2299, _2300);
                        float _2305 = _158[3];
                        float _2306 = _158[4];
                        _158[4] = max(_158[3], _158[4]);
                        _158[3] = min(_2305, _2306);
                        float _2311 = _158[2];
                        float _2312 = _158[3];
                        _158[3] = max(_158[2], _158[3]);
                        _158[2] = min(_2311, _2312);
                        float _2375;
                        if (min(_159[4], _158[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _2329;
                            _2329 = 65503.0;
                            int _2325;
                            int _2328;
                            float _2330;
                            for (int _2324 = 0, _2327 = 0, _2331 = 0; (_2331 < 5) && (_2329 < 65504.0); _2324 = _2325, _2327 = _2328, _2329 = _2330, _2331++)
                            {
                                float _2342 = _159[_2327] - _158[_2324];
                                if (abs(_2342) > 6.099999882280826568603515625e-05)
                                {
                                    float _2353 = max(_159[_2327], _158[_2324]);
                                    float _2359 = float(int(sign(_2342))) * (1.0 - ((_2353 != 0.0) ? (min(_159[_2327], _158[_2324]) / _2353) : 0.0));
                                    int _2368 = _2327 + int(_159[_2327] < _158[_2324]);
                                    _2325 = _2324 + int(_159[_2368] >= _158[_2324]);
                                    _2328 = _2368;
                                    _2330 = (abs(_2359) < abs(_2329)) ? _2359 : _2329;
                                }
                                else
                                {
                                    _2325 = _2324;
                                    _2328 = _2327;
                                    _2330 = 65504.0;
                                }
                            }
                            _2375 = _2329;
                        }
                        else
                        {
                            _2375 = 65503.0;
                        }
                        _2379 = _2375 * float(_2375 < 65503.0);
                    }
                    else
                    {
                        _2379 = 0.0;
                    }
                    vec4 _2380 = vec4(0.0);
                    _2380.x = _2379;
                    vec4 _2386 = _2380;
                    _2386.y = (_2379 != 0.0) ? float(int(sign(_2379))) : 0.0;
                    vec4 _2387 = _2386;
                    _2387.z = 1.0;
                    ivec2 _2390 = ivec2(vec2(ivec2(_1481 + uvec2(1u))));
                    ivec2 _2394 = _2390;
                    _2394.x = max(0, min(_2390.x, _248));
                    ivec2 _2398 = _2394;
                    _2398.y = max(0, min(_2390.y, _254));
                    vec2 _2404 = (vec2(_2398) + vec2(0.5)) / _264;
                    float _163[5];
                    for (int _2410 = 0; _2410 < 5; )
                    {
                        ivec2 _2417 = ivec2(floor((_2404 + _268) * _264)) + _116[_2410];
                        ivec2 _2421 = _2417;
                        _2421.x = max(0, min(_2417.x, _248));
                        ivec2 _2425 = _2421;
                        _2425.y = max(0, min(_2417.y, _254));
                        vec4 _2431 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _2432 = _2431.x;
                        _163[_2410] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_2425)), int(0u)).x * ((_2432 == 0.0) ? 1.0 : _2432);
                        _163[_2410] = pow(_163[_2410], 1.0);
                        _163[_2410] = max(_163[_2410], 6.099999882280826568603515625e-05);
                        _2410++;
                        continue;
                    }
                    vec2 _2443 = (_2404 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_2398)), int(0u)).xy;
                    float _2444 = _2443.x;
                    float _2448 = _2443.y;
                    bool _2452 = ((_2444 >= 0.0) && (_2444 <= 1.0)) && ((_2448 >= 0.0) && (_2448 <= 1.0));
                    float _131[5];
                    if (_2452)
                    {
                        for (int _2459 = 0; _2459 < 5; )
                        {
                            ivec2 _2466 = ivec2(floor(_2443 * _310)) + _116[_2459];
                            ivec2 _2472 = _2466;
                            _2472.x = max(0, min(_2466.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _2478 = _2472;
                            _2478.y = max(0, min(_2466.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _2487 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _2488 = _2487.x;
                            _131[_2459] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_2478)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_2488 == 0.0) ? 1.0 : _2488);
                            _131[_2459] = pow(_131[_2459], 1.0);
                            _131[_2459] = max(_131[_2459], 6.099999882280826568603515625e-05);
                            _2459++;
                            continue;
                        }
                    }
                    float _2680;
                    if (_2452)
                    {
                        float _162[5] = _163;
                        float _161[5] = _131;
                        float _2501 = _162[0];
                        float _2503 = _162[3];
                        _162[3] = max(_162[0], _162[3]);
                        _162[0] = min(_2501, _2503);
                        float _2509 = _162[1];
                        float _2511 = _162[4];
                        _162[4] = max(_162[1], _162[4]);
                        _162[1] = min(_2509, _2511);
                        float _2516 = _162[0];
                        float _2518 = _162[2];
                        _162[2] = max(_162[0], _162[2]);
                        _162[0] = min(_2516, _2518);
                        float _2523 = _162[1];
                        float _2524 = _162[3];
                        _162[3] = max(_162[1], _162[3]);
                        _162[1] = min(_2523, _2524);
                        float _2529 = _162[0];
                        float _2530 = _162[1];
                        _162[1] = max(_162[0], _162[1]);
                        _162[0] = min(_2529, _2530);
                        float _2535 = _162[2];
                        float _2536 = _162[4];
                        _162[4] = max(_162[2], _162[4]);
                        _162[2] = min(_2535, _2536);
                        float _2541 = _162[1];
                        float _2542 = _162[2];
                        _162[2] = max(_162[1], _162[2]);
                        _162[1] = min(_2541, _2542);
                        float _2547 = _162[3];
                        float _2548 = _162[4];
                        _162[4] = max(_162[3], _162[4]);
                        _162[3] = min(_2547, _2548);
                        float _2553 = _162[2];
                        float _2554 = _162[3];
                        _162[3] = max(_162[2], _162[3]);
                        _162[2] = min(_2553, _2554);
                        float _2560 = _161[0];
                        float _2562 = _161[3];
                        _161[3] = max(_161[0], _161[3]);
                        _161[0] = min(_2560, _2562);
                        float _2568 = _161[1];
                        float _2570 = _161[4];
                        _161[4] = max(_161[1], _161[4]);
                        _161[1] = min(_2568, _2570);
                        float _2575 = _161[0];
                        float _2577 = _161[2];
                        _161[2] = max(_161[0], _161[2]);
                        _161[0] = min(_2575, _2577);
                        float _2582 = _161[1];
                        float _2583 = _161[3];
                        _161[3] = max(_161[1], _161[3]);
                        _161[1] = min(_2582, _2583);
                        float _2588 = _161[0];
                        float _2589 = _161[1];
                        _161[1] = max(_161[0], _161[1]);
                        _161[0] = min(_2588, _2589);
                        float _2594 = _161[2];
                        float _2595 = _161[4];
                        _161[4] = max(_161[2], _161[4]);
                        _161[2] = min(_2594, _2595);
                        float _2600 = _161[1];
                        float _2601 = _161[2];
                        _161[2] = max(_161[1], _161[2]);
                        _161[1] = min(_2600, _2601);
                        float _2606 = _161[3];
                        float _2607 = _161[4];
                        _161[4] = max(_161[3], _161[4]);
                        _161[3] = min(_2606, _2607);
                        float _2612 = _161[2];
                        float _2613 = _161[3];
                        _161[3] = max(_161[2], _161[3]);
                        _161[2] = min(_2612, _2613);
                        float _2676;
                        if (min(_162[4], _161[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _2630;
                            _2630 = 65503.0;
                            int _2626;
                            int _2629;
                            float _2631;
                            for (int _2625 = 0, _2628 = 0, _2632 = 0; (_2632 < 5) && (_2630 < 65504.0); _2625 = _2626, _2628 = _2629, _2630 = _2631, _2632++)
                            {
                                float _2643 = _162[_2628] - _161[_2625];
                                if (abs(_2643) > 6.099999882280826568603515625e-05)
                                {
                                    float _2654 = max(_162[_2628], _161[_2625]);
                                    float _2660 = float(int(sign(_2643))) * (1.0 - ((_2654 != 0.0) ? (min(_162[_2628], _161[_2625]) / _2654) : 0.0));
                                    int _2669 = _2628 + int(_162[_2628] < _161[_2625]);
                                    _2626 = _2625 + int(_162[_2669] >= _161[_2625]);
                                    _2629 = _2669;
                                    _2631 = (abs(_2660) < abs(_2630)) ? _2660 : _2630;
                                }
                                else
                                {
                                    _2626 = _2625;
                                    _2629 = _2628;
                                    _2631 = 65504.0;
                                }
                            }
                            _2676 = _2630;
                        }
                        else
                        {
                            _2676 = 65503.0;
                        }
                        _2680 = _2676 * float(_2676 < 65503.0);
                    }
                    else
                    {
                        _2680 = 0.0;
                    }
                    vec4 _2681 = vec4(0.0);
                    _2681.x = _2680;
                    vec4 _2687 = _2681;
                    _2687.y = (_2680 != 0.0) ? float(int(sign(_2680))) : 0.0;
                    vec4 _2688 = _2687;
                    _2688.z = 1.0;
                    _188[1] = (((_1785 + _2086) + _2387) + _2688) * 0.25;
                    imageStore(rw_spd_mip0, ivec2(uvec2(_232 + ivec2(_1478, _234))), vec4(_188[1].xy, 0.0, 0.0));
                    int _2701 = int(_227 + 32u);
                    int _2705 = int(_220 + 16u);
                    uvec2 _2708 = uvec2(_224 + ivec2(_226, _2701));
                    ivec2 _2714 = ivec2(vec2(ivec2(_2708)));
                    ivec2 _2718 = _2714;
                    _2718.x = max(0, min(_2714.x, _248));
                    ivec2 _2722 = _2718;
                    _2722.y = max(0, min(_2714.y, _254));
                    vec2 _2728 = (vec2(_2722) + vec2(0.5)) / _264;
                    float _166[5];
                    for (int _2734 = 0; _2734 < 5; )
                    {
                        ivec2 _2741 = ivec2(floor((_2728 + _268) * _264)) + _116[_2734];
                        ivec2 _2745 = _2741;
                        _2745.x = max(0, min(_2741.x, _248));
                        ivec2 _2749 = _2745;
                        _2749.y = max(0, min(_2741.y, _254));
                        vec4 _2755 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _2756 = _2755.x;
                        _166[_2734] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_2749)), int(0u)).x * ((_2756 == 0.0) ? 1.0 : _2756);
                        _166[_2734] = pow(_166[_2734], 1.0);
                        _166[_2734] = max(_166[_2734], 6.099999882280826568603515625e-05);
                        _2734++;
                        continue;
                    }
                    vec2 _2767 = (_2728 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_2722)), int(0u)).xy;
                    float _2768 = _2767.x;
                    float _2772 = _2767.y;
                    bool _2776 = ((_2768 >= 0.0) && (_2768 <= 1.0)) && ((_2772 >= 0.0) && (_2772 <= 1.0));
                    float _132[5];
                    if (_2776)
                    {
                        for (int _2783 = 0; _2783 < 5; )
                        {
                            ivec2 _2790 = ivec2(floor(_2767 * _310)) + _116[_2783];
                            ivec2 _2796 = _2790;
                            _2796.x = max(0, min(_2790.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _2802 = _2796;
                            _2802.y = max(0, min(_2790.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _2811 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _2812 = _2811.x;
                            _132[_2783] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_2802)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_2812 == 0.0) ? 1.0 : _2812);
                            _132[_2783] = pow(_132[_2783], 1.0);
                            _132[_2783] = max(_132[_2783], 6.099999882280826568603515625e-05);
                            _2783++;
                            continue;
                        }
                    }
                    float _3004;
                    if (_2776)
                    {
                        float _165[5] = _166;
                        float _164[5] = _132;
                        float _2825 = _165[0];
                        float _2827 = _165[3];
                        _165[3] = max(_165[0], _165[3]);
                        _165[0] = min(_2825, _2827);
                        float _2833 = _165[1];
                        float _2835 = _165[4];
                        _165[4] = max(_165[1], _165[4]);
                        _165[1] = min(_2833, _2835);
                        float _2840 = _165[0];
                        float _2842 = _165[2];
                        _165[2] = max(_165[0], _165[2]);
                        _165[0] = min(_2840, _2842);
                        float _2847 = _165[1];
                        float _2848 = _165[3];
                        _165[3] = max(_165[1], _165[3]);
                        _165[1] = min(_2847, _2848);
                        float _2853 = _165[0];
                        float _2854 = _165[1];
                        _165[1] = max(_165[0], _165[1]);
                        _165[0] = min(_2853, _2854);
                        float _2859 = _165[2];
                        float _2860 = _165[4];
                        _165[4] = max(_165[2], _165[4]);
                        _165[2] = min(_2859, _2860);
                        float _2865 = _165[1];
                        float _2866 = _165[2];
                        _165[2] = max(_165[1], _165[2]);
                        _165[1] = min(_2865, _2866);
                        float _2871 = _165[3];
                        float _2872 = _165[4];
                        _165[4] = max(_165[3], _165[4]);
                        _165[3] = min(_2871, _2872);
                        float _2877 = _165[2];
                        float _2878 = _165[3];
                        _165[3] = max(_165[2], _165[3]);
                        _165[2] = min(_2877, _2878);
                        float _2884 = _164[0];
                        float _2886 = _164[3];
                        _164[3] = max(_164[0], _164[3]);
                        _164[0] = min(_2884, _2886);
                        float _2892 = _164[1];
                        float _2894 = _164[4];
                        _164[4] = max(_164[1], _164[4]);
                        _164[1] = min(_2892, _2894);
                        float _2899 = _164[0];
                        float _2901 = _164[2];
                        _164[2] = max(_164[0], _164[2]);
                        _164[0] = min(_2899, _2901);
                        float _2906 = _164[1];
                        float _2907 = _164[3];
                        _164[3] = max(_164[1], _164[3]);
                        _164[1] = min(_2906, _2907);
                        float _2912 = _164[0];
                        float _2913 = _164[1];
                        _164[1] = max(_164[0], _164[1]);
                        _164[0] = min(_2912, _2913);
                        float _2918 = _164[2];
                        float _2919 = _164[4];
                        _164[4] = max(_164[2], _164[4]);
                        _164[2] = min(_2918, _2919);
                        float _2924 = _164[1];
                        float _2925 = _164[2];
                        _164[2] = max(_164[1], _164[2]);
                        _164[1] = min(_2924, _2925);
                        float _2930 = _164[3];
                        float _2931 = _164[4];
                        _164[4] = max(_164[3], _164[4]);
                        _164[3] = min(_2930, _2931);
                        float _2936 = _164[2];
                        float _2937 = _164[3];
                        _164[3] = max(_164[2], _164[3]);
                        _164[2] = min(_2936, _2937);
                        float _3000;
                        if (min(_165[4], _164[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _2954;
                            _2954 = 65503.0;
                            int _2950;
                            int _2953;
                            float _2955;
                            for (int _2949 = 0, _2952 = 0, _2956 = 0; (_2956 < 5) && (_2954 < 65504.0); _2949 = _2950, _2952 = _2953, _2954 = _2955, _2956++)
                            {
                                float _2967 = _165[_2952] - _164[_2949];
                                if (abs(_2967) > 6.099999882280826568603515625e-05)
                                {
                                    float _2978 = max(_165[_2952], _164[_2949]);
                                    float _2984 = float(int(sign(_2967))) * (1.0 - ((_2978 != 0.0) ? (min(_165[_2952], _164[_2949]) / _2978) : 0.0));
                                    int _2993 = _2952 + int(_165[_2952] < _164[_2949]);
                                    _2950 = _2949 + int(_165[_2993] >= _164[_2949]);
                                    _2953 = _2993;
                                    _2955 = (abs(_2984) < abs(_2954)) ? _2984 : _2954;
                                }
                                else
                                {
                                    _2950 = _2949;
                                    _2953 = _2952;
                                    _2955 = 65504.0;
                                }
                            }
                            _3000 = _2954;
                        }
                        else
                        {
                            _3000 = 65503.0;
                        }
                        _3004 = _3000 * float(_3000 < 65503.0);
                    }
                    else
                    {
                        _3004 = 0.0;
                    }
                    vec4 _3005 = vec4(0.0);
                    _3005.x = _3004;
                    vec4 _3011 = _3005;
                    _3011.y = (_3004 != 0.0) ? float(int(sign(_3004))) : 0.0;
                    vec4 _3012 = _3011;
                    _3012.z = 1.0;
                    ivec2 _3015 = ivec2(vec2(ivec2(_2708 + uvec2(0u, 1u))));
                    ivec2 _3019 = _3015;
                    _3019.x = max(0, min(_3015.x, _248));
                    ivec2 _3023 = _3019;
                    _3023.y = max(0, min(_3015.y, _254));
                    vec2 _3029 = (vec2(_3023) + vec2(0.5)) / _264;
                    float _169[5];
                    for (int _3035 = 0; _3035 < 5; )
                    {
                        ivec2 _3042 = ivec2(floor((_3029 + _268) * _264)) + _116[_3035];
                        ivec2 _3046 = _3042;
                        _3046.x = max(0, min(_3042.x, _248));
                        ivec2 _3050 = _3046;
                        _3050.y = max(0, min(_3042.y, _254));
                        vec4 _3056 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _3057 = _3056.x;
                        _169[_3035] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_3050)), int(0u)).x * ((_3057 == 0.0) ? 1.0 : _3057);
                        _169[_3035] = pow(_169[_3035], 1.0);
                        _169[_3035] = max(_169[_3035], 6.099999882280826568603515625e-05);
                        _3035++;
                        continue;
                    }
                    vec2 _3068 = (_3029 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_3023)), int(0u)).xy;
                    float _3069 = _3068.x;
                    float _3073 = _3068.y;
                    bool _3077 = ((_3069 >= 0.0) && (_3069 <= 1.0)) && ((_3073 >= 0.0) && (_3073 <= 1.0));
                    float _133[5];
                    if (_3077)
                    {
                        for (int _3084 = 0; _3084 < 5; )
                        {
                            ivec2 _3091 = ivec2(floor(_3068 * _310)) + _116[_3084];
                            ivec2 _3097 = _3091;
                            _3097.x = max(0, min(_3091.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _3103 = _3097;
                            _3103.y = max(0, min(_3091.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _3112 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _3113 = _3112.x;
                            _133[_3084] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_3103)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_3113 == 0.0) ? 1.0 : _3113);
                            _133[_3084] = pow(_133[_3084], 1.0);
                            _133[_3084] = max(_133[_3084], 6.099999882280826568603515625e-05);
                            _3084++;
                            continue;
                        }
                    }
                    float _3305;
                    if (_3077)
                    {
                        float _168[5] = _169;
                        float _167[5] = _133;
                        float _3126 = _168[0];
                        float _3128 = _168[3];
                        _168[3] = max(_168[0], _168[3]);
                        _168[0] = min(_3126, _3128);
                        float _3134 = _168[1];
                        float _3136 = _168[4];
                        _168[4] = max(_168[1], _168[4]);
                        _168[1] = min(_3134, _3136);
                        float _3141 = _168[0];
                        float _3143 = _168[2];
                        _168[2] = max(_168[0], _168[2]);
                        _168[0] = min(_3141, _3143);
                        float _3148 = _168[1];
                        float _3149 = _168[3];
                        _168[3] = max(_168[1], _168[3]);
                        _168[1] = min(_3148, _3149);
                        float _3154 = _168[0];
                        float _3155 = _168[1];
                        _168[1] = max(_168[0], _168[1]);
                        _168[0] = min(_3154, _3155);
                        float _3160 = _168[2];
                        float _3161 = _168[4];
                        _168[4] = max(_168[2], _168[4]);
                        _168[2] = min(_3160, _3161);
                        float _3166 = _168[1];
                        float _3167 = _168[2];
                        _168[2] = max(_168[1], _168[2]);
                        _168[1] = min(_3166, _3167);
                        float _3172 = _168[3];
                        float _3173 = _168[4];
                        _168[4] = max(_168[3], _168[4]);
                        _168[3] = min(_3172, _3173);
                        float _3178 = _168[2];
                        float _3179 = _168[3];
                        _168[3] = max(_168[2], _168[3]);
                        _168[2] = min(_3178, _3179);
                        float _3185 = _167[0];
                        float _3187 = _167[3];
                        _167[3] = max(_167[0], _167[3]);
                        _167[0] = min(_3185, _3187);
                        float _3193 = _167[1];
                        float _3195 = _167[4];
                        _167[4] = max(_167[1], _167[4]);
                        _167[1] = min(_3193, _3195);
                        float _3200 = _167[0];
                        float _3202 = _167[2];
                        _167[2] = max(_167[0], _167[2]);
                        _167[0] = min(_3200, _3202);
                        float _3207 = _167[1];
                        float _3208 = _167[3];
                        _167[3] = max(_167[1], _167[3]);
                        _167[1] = min(_3207, _3208);
                        float _3213 = _167[0];
                        float _3214 = _167[1];
                        _167[1] = max(_167[0], _167[1]);
                        _167[0] = min(_3213, _3214);
                        float _3219 = _167[2];
                        float _3220 = _167[4];
                        _167[4] = max(_167[2], _167[4]);
                        _167[2] = min(_3219, _3220);
                        float _3225 = _167[1];
                        float _3226 = _167[2];
                        _167[2] = max(_167[1], _167[2]);
                        _167[1] = min(_3225, _3226);
                        float _3231 = _167[3];
                        float _3232 = _167[4];
                        _167[4] = max(_167[3], _167[4]);
                        _167[3] = min(_3231, _3232);
                        float _3237 = _167[2];
                        float _3238 = _167[3];
                        _167[3] = max(_167[2], _167[3]);
                        _167[2] = min(_3237, _3238);
                        float _3301;
                        if (min(_168[4], _167[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _3255;
                            _3255 = 65503.0;
                            int _3251;
                            int _3254;
                            float _3256;
                            for (int _3250 = 0, _3253 = 0, _3257 = 0; (_3257 < 5) && (_3255 < 65504.0); _3250 = _3251, _3253 = _3254, _3255 = _3256, _3257++)
                            {
                                float _3268 = _168[_3253] - _167[_3250];
                                if (abs(_3268) > 6.099999882280826568603515625e-05)
                                {
                                    float _3279 = max(_168[_3253], _167[_3250]);
                                    float _3285 = float(int(sign(_3268))) * (1.0 - ((_3279 != 0.0) ? (min(_168[_3253], _167[_3250]) / _3279) : 0.0));
                                    int _3294 = _3253 + int(_168[_3253] < _167[_3250]);
                                    _3251 = _3250 + int(_168[_3294] >= _167[_3250]);
                                    _3254 = _3294;
                                    _3256 = (abs(_3285) < abs(_3255)) ? _3285 : _3255;
                                }
                                else
                                {
                                    _3251 = _3250;
                                    _3254 = _3253;
                                    _3256 = 65504.0;
                                }
                            }
                            _3301 = _3255;
                        }
                        else
                        {
                            _3301 = 65503.0;
                        }
                        _3305 = _3301 * float(_3301 < 65503.0);
                    }
                    else
                    {
                        _3305 = 0.0;
                    }
                    vec4 _3306 = vec4(0.0);
                    _3306.x = _3305;
                    vec4 _3312 = _3306;
                    _3312.y = (_3305 != 0.0) ? float(int(sign(_3305))) : 0.0;
                    vec4 _3313 = _3312;
                    _3313.z = 1.0;
                    ivec2 _3316 = ivec2(vec2(ivec2(_2708 + uvec2(1u, 0u))));
                    ivec2 _3320 = _3316;
                    _3320.x = max(0, min(_3316.x, _248));
                    ivec2 _3324 = _3320;
                    _3324.y = max(0, min(_3316.y, _254));
                    vec2 _3330 = (vec2(_3324) + vec2(0.5)) / _264;
                    float _172[5];
                    for (int _3336 = 0; _3336 < 5; )
                    {
                        ivec2 _3343 = ivec2(floor((_3330 + _268) * _264)) + _116[_3336];
                        ivec2 _3347 = _3343;
                        _3347.x = max(0, min(_3343.x, _248));
                        ivec2 _3351 = _3347;
                        _3351.y = max(0, min(_3343.y, _254));
                        vec4 _3357 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _3358 = _3357.x;
                        _172[_3336] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_3351)), int(0u)).x * ((_3358 == 0.0) ? 1.0 : _3358);
                        _172[_3336] = pow(_172[_3336], 1.0);
                        _172[_3336] = max(_172[_3336], 6.099999882280826568603515625e-05);
                        _3336++;
                        continue;
                    }
                    vec2 _3369 = (_3330 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_3324)), int(0u)).xy;
                    float _3370 = _3369.x;
                    float _3374 = _3369.y;
                    bool _3378 = ((_3370 >= 0.0) && (_3370 <= 1.0)) && ((_3374 >= 0.0) && (_3374 <= 1.0));
                    float _134[5];
                    if (_3378)
                    {
                        for (int _3385 = 0; _3385 < 5; )
                        {
                            ivec2 _3392 = ivec2(floor(_3369 * _310)) + _116[_3385];
                            ivec2 _3398 = _3392;
                            _3398.x = max(0, min(_3392.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _3404 = _3398;
                            _3404.y = max(0, min(_3392.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _3413 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _3414 = _3413.x;
                            _134[_3385] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_3404)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_3414 == 0.0) ? 1.0 : _3414);
                            _134[_3385] = pow(_134[_3385], 1.0);
                            _134[_3385] = max(_134[_3385], 6.099999882280826568603515625e-05);
                            _3385++;
                            continue;
                        }
                    }
                    float _3606;
                    if (_3378)
                    {
                        float _171[5] = _172;
                        float _170[5] = _134;
                        float _3427 = _171[0];
                        float _3429 = _171[3];
                        _171[3] = max(_171[0], _171[3]);
                        _171[0] = min(_3427, _3429);
                        float _3435 = _171[1];
                        float _3437 = _171[4];
                        _171[4] = max(_171[1], _171[4]);
                        _171[1] = min(_3435, _3437);
                        float _3442 = _171[0];
                        float _3444 = _171[2];
                        _171[2] = max(_171[0], _171[2]);
                        _171[0] = min(_3442, _3444);
                        float _3449 = _171[1];
                        float _3450 = _171[3];
                        _171[3] = max(_171[1], _171[3]);
                        _171[1] = min(_3449, _3450);
                        float _3455 = _171[0];
                        float _3456 = _171[1];
                        _171[1] = max(_171[0], _171[1]);
                        _171[0] = min(_3455, _3456);
                        float _3461 = _171[2];
                        float _3462 = _171[4];
                        _171[4] = max(_171[2], _171[4]);
                        _171[2] = min(_3461, _3462);
                        float _3467 = _171[1];
                        float _3468 = _171[2];
                        _171[2] = max(_171[1], _171[2]);
                        _171[1] = min(_3467, _3468);
                        float _3473 = _171[3];
                        float _3474 = _171[4];
                        _171[4] = max(_171[3], _171[4]);
                        _171[3] = min(_3473, _3474);
                        float _3479 = _171[2];
                        float _3480 = _171[3];
                        _171[3] = max(_171[2], _171[3]);
                        _171[2] = min(_3479, _3480);
                        float _3486 = _170[0];
                        float _3488 = _170[3];
                        _170[3] = max(_170[0], _170[3]);
                        _170[0] = min(_3486, _3488);
                        float _3494 = _170[1];
                        float _3496 = _170[4];
                        _170[4] = max(_170[1], _170[4]);
                        _170[1] = min(_3494, _3496);
                        float _3501 = _170[0];
                        float _3503 = _170[2];
                        _170[2] = max(_170[0], _170[2]);
                        _170[0] = min(_3501, _3503);
                        float _3508 = _170[1];
                        float _3509 = _170[3];
                        _170[3] = max(_170[1], _170[3]);
                        _170[1] = min(_3508, _3509);
                        float _3514 = _170[0];
                        float _3515 = _170[1];
                        _170[1] = max(_170[0], _170[1]);
                        _170[0] = min(_3514, _3515);
                        float _3520 = _170[2];
                        float _3521 = _170[4];
                        _170[4] = max(_170[2], _170[4]);
                        _170[2] = min(_3520, _3521);
                        float _3526 = _170[1];
                        float _3527 = _170[2];
                        _170[2] = max(_170[1], _170[2]);
                        _170[1] = min(_3526, _3527);
                        float _3532 = _170[3];
                        float _3533 = _170[4];
                        _170[4] = max(_170[3], _170[4]);
                        _170[3] = min(_3532, _3533);
                        float _3538 = _170[2];
                        float _3539 = _170[3];
                        _170[3] = max(_170[2], _170[3]);
                        _170[2] = min(_3538, _3539);
                        float _3602;
                        if (min(_171[4], _170[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _3556;
                            _3556 = 65503.0;
                            int _3552;
                            int _3555;
                            float _3557;
                            for (int _3551 = 0, _3554 = 0, _3558 = 0; (_3558 < 5) && (_3556 < 65504.0); _3551 = _3552, _3554 = _3555, _3556 = _3557, _3558++)
                            {
                                float _3569 = _171[_3554] - _170[_3551];
                                if (abs(_3569) > 6.099999882280826568603515625e-05)
                                {
                                    float _3580 = max(_171[_3554], _170[_3551]);
                                    float _3586 = float(int(sign(_3569))) * (1.0 - ((_3580 != 0.0) ? (min(_171[_3554], _170[_3551]) / _3580) : 0.0));
                                    int _3595 = _3554 + int(_171[_3554] < _170[_3551]);
                                    _3552 = _3551 + int(_171[_3595] >= _170[_3551]);
                                    _3555 = _3595;
                                    _3557 = (abs(_3586) < abs(_3556)) ? _3586 : _3556;
                                }
                                else
                                {
                                    _3552 = _3551;
                                    _3555 = _3554;
                                    _3557 = 65504.0;
                                }
                            }
                            _3602 = _3556;
                        }
                        else
                        {
                            _3602 = 65503.0;
                        }
                        _3606 = _3602 * float(_3602 < 65503.0);
                    }
                    else
                    {
                        _3606 = 0.0;
                    }
                    vec4 _3607 = vec4(0.0);
                    _3607.x = _3606;
                    vec4 _3613 = _3607;
                    _3613.y = (_3606 != 0.0) ? float(int(sign(_3606))) : 0.0;
                    vec4 _3614 = _3613;
                    _3614.z = 1.0;
                    ivec2 _3617 = ivec2(vec2(ivec2(_2708 + uvec2(1u))));
                    ivec2 _3621 = _3617;
                    _3621.x = max(0, min(_3617.x, _248));
                    ivec2 _3625 = _3621;
                    _3625.y = max(0, min(_3617.y, _254));
                    vec2 _3631 = (vec2(_3625) + vec2(0.5)) / _264;
                    float _175[5];
                    for (int _3637 = 0; _3637 < 5; )
                    {
                        ivec2 _3644 = ivec2(floor((_3631 + _268) * _264)) + _116[_3637];
                        ivec2 _3648 = _3644;
                        _3648.x = max(0, min(_3644.x, _248));
                        ivec2 _3652 = _3648;
                        _3652.y = max(0, min(_3644.y, _254));
                        vec4 _3658 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _3659 = _3658.x;
                        _175[_3637] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_3652)), int(0u)).x * ((_3659 == 0.0) ? 1.0 : _3659);
                        _175[_3637] = pow(_175[_3637], 1.0);
                        _175[_3637] = max(_175[_3637], 6.099999882280826568603515625e-05);
                        _3637++;
                        continue;
                    }
                    vec2 _3670 = (_3631 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_3625)), int(0u)).xy;
                    float _3671 = _3670.x;
                    float _3675 = _3670.y;
                    bool _3679 = ((_3671 >= 0.0) && (_3671 <= 1.0)) && ((_3675 >= 0.0) && (_3675 <= 1.0));
                    float _135[5];
                    if (_3679)
                    {
                        for (int _3686 = 0; _3686 < 5; )
                        {
                            ivec2 _3693 = ivec2(floor(_3670 * _310)) + _116[_3686];
                            ivec2 _3699 = _3693;
                            _3699.x = max(0, min(_3693.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _3705 = _3699;
                            _3705.y = max(0, min(_3693.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _3714 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _3715 = _3714.x;
                            _135[_3686] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_3705)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_3715 == 0.0) ? 1.0 : _3715);
                            _135[_3686] = pow(_135[_3686], 1.0);
                            _135[_3686] = max(_135[_3686], 6.099999882280826568603515625e-05);
                            _3686++;
                            continue;
                        }
                    }
                    float _3907;
                    if (_3679)
                    {
                        float _174[5] = _175;
                        float _173[5] = _135;
                        float _3728 = _174[0];
                        float _3730 = _174[3];
                        _174[3] = max(_174[0], _174[3]);
                        _174[0] = min(_3728, _3730);
                        float _3736 = _174[1];
                        float _3738 = _174[4];
                        _174[4] = max(_174[1], _174[4]);
                        _174[1] = min(_3736, _3738);
                        float _3743 = _174[0];
                        float _3745 = _174[2];
                        _174[2] = max(_174[0], _174[2]);
                        _174[0] = min(_3743, _3745);
                        float _3750 = _174[1];
                        float _3751 = _174[3];
                        _174[3] = max(_174[1], _174[3]);
                        _174[1] = min(_3750, _3751);
                        float _3756 = _174[0];
                        float _3757 = _174[1];
                        _174[1] = max(_174[0], _174[1]);
                        _174[0] = min(_3756, _3757);
                        float _3762 = _174[2];
                        float _3763 = _174[4];
                        _174[4] = max(_174[2], _174[4]);
                        _174[2] = min(_3762, _3763);
                        float _3768 = _174[1];
                        float _3769 = _174[2];
                        _174[2] = max(_174[1], _174[2]);
                        _174[1] = min(_3768, _3769);
                        float _3774 = _174[3];
                        float _3775 = _174[4];
                        _174[4] = max(_174[3], _174[4]);
                        _174[3] = min(_3774, _3775);
                        float _3780 = _174[2];
                        float _3781 = _174[3];
                        _174[3] = max(_174[2], _174[3]);
                        _174[2] = min(_3780, _3781);
                        float _3787 = _173[0];
                        float _3789 = _173[3];
                        _173[3] = max(_173[0], _173[3]);
                        _173[0] = min(_3787, _3789);
                        float _3795 = _173[1];
                        float _3797 = _173[4];
                        _173[4] = max(_173[1], _173[4]);
                        _173[1] = min(_3795, _3797);
                        float _3802 = _173[0];
                        float _3804 = _173[2];
                        _173[2] = max(_173[0], _173[2]);
                        _173[0] = min(_3802, _3804);
                        float _3809 = _173[1];
                        float _3810 = _173[3];
                        _173[3] = max(_173[1], _173[3]);
                        _173[1] = min(_3809, _3810);
                        float _3815 = _173[0];
                        float _3816 = _173[1];
                        _173[1] = max(_173[0], _173[1]);
                        _173[0] = min(_3815, _3816);
                        float _3821 = _173[2];
                        float _3822 = _173[4];
                        _173[4] = max(_173[2], _173[4]);
                        _173[2] = min(_3821, _3822);
                        float _3827 = _173[1];
                        float _3828 = _173[2];
                        _173[2] = max(_173[1], _173[2]);
                        _173[1] = min(_3827, _3828);
                        float _3833 = _173[3];
                        float _3834 = _173[4];
                        _173[4] = max(_173[3], _173[4]);
                        _173[3] = min(_3833, _3834);
                        float _3839 = _173[2];
                        float _3840 = _173[3];
                        _173[3] = max(_173[2], _173[3]);
                        _173[2] = min(_3839, _3840);
                        float _3903;
                        if (min(_174[4], _173[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _3857;
                            _3857 = 65503.0;
                            int _3853;
                            int _3856;
                            float _3858;
                            for (int _3852 = 0, _3855 = 0, _3859 = 0; (_3859 < 5) && (_3857 < 65504.0); _3852 = _3853, _3855 = _3856, _3857 = _3858, _3859++)
                            {
                                float _3870 = _174[_3855] - _173[_3852];
                                if (abs(_3870) > 6.099999882280826568603515625e-05)
                                {
                                    float _3881 = max(_174[_3855], _173[_3852]);
                                    float _3887 = float(int(sign(_3870))) * (1.0 - ((_3881 != 0.0) ? (min(_174[_3855], _173[_3852]) / _3881) : 0.0));
                                    int _3896 = _3855 + int(_174[_3855] < _173[_3852]);
                                    _3853 = _3852 + int(_174[_3896] >= _173[_3852]);
                                    _3856 = _3896;
                                    _3858 = (abs(_3887) < abs(_3857)) ? _3887 : _3857;
                                }
                                else
                                {
                                    _3853 = _3852;
                                    _3856 = _3855;
                                    _3858 = 65504.0;
                                }
                            }
                            _3903 = _3857;
                        }
                        else
                        {
                            _3903 = 65503.0;
                        }
                        _3907 = _3903 * float(_3903 < 65503.0);
                    }
                    else
                    {
                        _3907 = 0.0;
                    }
                    vec4 _3908 = vec4(0.0);
                    _3908.x = _3907;
                    vec4 _3914 = _3908;
                    _3914.y = (_3907 != 0.0) ? float(int(sign(_3907))) : 0.0;
                    vec4 _3915 = _3914;
                    _3915.z = 1.0;
                    _188[2] = (((_3012 + _3313) + _3614) + _3915) * 0.25;
                    imageStore(rw_spd_mip0, ivec2(uvec2(_232 + ivec2(_233, _2705))), vec4(_188[2].xy, 0.0, 0.0));
                    uvec2 _3931 = uvec2(_224 + ivec2(_1474, _2701));
                    ivec2 _3937 = ivec2(vec2(ivec2(_3931)));
                    ivec2 _3941 = _3937;
                    _3941.x = max(0, min(_3937.x, _248));
                    ivec2 _3945 = _3941;
                    _3945.y = max(0, min(_3937.y, _254));
                    vec2 _3951 = (vec2(_3945) + vec2(0.5)) / _264;
                    float _178[5];
                    for (int _3957 = 0; _3957 < 5; )
                    {
                        ivec2 _3964 = ivec2(floor((_3951 + _268) * _264)) + _116[_3957];
                        ivec2 _3968 = _3964;
                        _3968.x = max(0, min(_3964.x, _248));
                        ivec2 _3972 = _3968;
                        _3972.y = max(0, min(_3964.y, _254));
                        vec4 _3978 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _3979 = _3978.x;
                        _178[_3957] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_3972)), int(0u)).x * ((_3979 == 0.0) ? 1.0 : _3979);
                        _178[_3957] = pow(_178[_3957], 1.0);
                        _178[_3957] = max(_178[_3957], 6.099999882280826568603515625e-05);
                        _3957++;
                        continue;
                    }
                    vec2 _3990 = (_3951 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_3945)), int(0u)).xy;
                    float _3991 = _3990.x;
                    float _3995 = _3990.y;
                    bool _3999 = ((_3991 >= 0.0) && (_3991 <= 1.0)) && ((_3995 >= 0.0) && (_3995 <= 1.0));
                    float _136[5];
                    if (_3999)
                    {
                        for (int _4006 = 0; _4006 < 5; )
                        {
                            ivec2 _4013 = ivec2(floor(_3990 * _310)) + _116[_4006];
                            ivec2 _4019 = _4013;
                            _4019.x = max(0, min(_4013.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _4025 = _4019;
                            _4025.y = max(0, min(_4013.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _4034 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _4035 = _4034.x;
                            _136[_4006] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_4025)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_4035 == 0.0) ? 1.0 : _4035);
                            _136[_4006] = pow(_136[_4006], 1.0);
                            _136[_4006] = max(_136[_4006], 6.099999882280826568603515625e-05);
                            _4006++;
                            continue;
                        }
                    }
                    float _4227;
                    if (_3999)
                    {
                        float _177[5] = _178;
                        float _176[5] = _136;
                        float _4048 = _177[0];
                        float _4050 = _177[3];
                        _177[3] = max(_177[0], _177[3]);
                        _177[0] = min(_4048, _4050);
                        float _4056 = _177[1];
                        float _4058 = _177[4];
                        _177[4] = max(_177[1], _177[4]);
                        _177[1] = min(_4056, _4058);
                        float _4063 = _177[0];
                        float _4065 = _177[2];
                        _177[2] = max(_177[0], _177[2]);
                        _177[0] = min(_4063, _4065);
                        float _4070 = _177[1];
                        float _4071 = _177[3];
                        _177[3] = max(_177[1], _177[3]);
                        _177[1] = min(_4070, _4071);
                        float _4076 = _177[0];
                        float _4077 = _177[1];
                        _177[1] = max(_177[0], _177[1]);
                        _177[0] = min(_4076, _4077);
                        float _4082 = _177[2];
                        float _4083 = _177[4];
                        _177[4] = max(_177[2], _177[4]);
                        _177[2] = min(_4082, _4083);
                        float _4088 = _177[1];
                        float _4089 = _177[2];
                        _177[2] = max(_177[1], _177[2]);
                        _177[1] = min(_4088, _4089);
                        float _4094 = _177[3];
                        float _4095 = _177[4];
                        _177[4] = max(_177[3], _177[4]);
                        _177[3] = min(_4094, _4095);
                        float _4100 = _177[2];
                        float _4101 = _177[3];
                        _177[3] = max(_177[2], _177[3]);
                        _177[2] = min(_4100, _4101);
                        float _4107 = _176[0];
                        float _4109 = _176[3];
                        _176[3] = max(_176[0], _176[3]);
                        _176[0] = min(_4107, _4109);
                        float _4115 = _176[1];
                        float _4117 = _176[4];
                        _176[4] = max(_176[1], _176[4]);
                        _176[1] = min(_4115, _4117);
                        float _4122 = _176[0];
                        float _4124 = _176[2];
                        _176[2] = max(_176[0], _176[2]);
                        _176[0] = min(_4122, _4124);
                        float _4129 = _176[1];
                        float _4130 = _176[3];
                        _176[3] = max(_176[1], _176[3]);
                        _176[1] = min(_4129, _4130);
                        float _4135 = _176[0];
                        float _4136 = _176[1];
                        _176[1] = max(_176[0], _176[1]);
                        _176[0] = min(_4135, _4136);
                        float _4141 = _176[2];
                        float _4142 = _176[4];
                        _176[4] = max(_176[2], _176[4]);
                        _176[2] = min(_4141, _4142);
                        float _4147 = _176[1];
                        float _4148 = _176[2];
                        _176[2] = max(_176[1], _176[2]);
                        _176[1] = min(_4147, _4148);
                        float _4153 = _176[3];
                        float _4154 = _176[4];
                        _176[4] = max(_176[3], _176[4]);
                        _176[3] = min(_4153, _4154);
                        float _4159 = _176[2];
                        float _4160 = _176[3];
                        _176[3] = max(_176[2], _176[3]);
                        _176[2] = min(_4159, _4160);
                        float _4223;
                        if (min(_177[4], _176[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _4177;
                            _4177 = 65503.0;
                            int _4173;
                            int _4176;
                            float _4178;
                            for (int _4172 = 0, _4175 = 0, _4179 = 0; (_4179 < 5) && (_4177 < 65504.0); _4172 = _4173, _4175 = _4176, _4177 = _4178, _4179++)
                            {
                                float _4190 = _177[_4175] - _176[_4172];
                                if (abs(_4190) > 6.099999882280826568603515625e-05)
                                {
                                    float _4201 = max(_177[_4175], _176[_4172]);
                                    float _4207 = float(int(sign(_4190))) * (1.0 - ((_4201 != 0.0) ? (min(_177[_4175], _176[_4172]) / _4201) : 0.0));
                                    int _4216 = _4175 + int(_177[_4175] < _176[_4172]);
                                    _4173 = _4172 + int(_177[_4216] >= _176[_4172]);
                                    _4176 = _4216;
                                    _4178 = (abs(_4207) < abs(_4177)) ? _4207 : _4177;
                                }
                                else
                                {
                                    _4173 = _4172;
                                    _4176 = _4175;
                                    _4178 = 65504.0;
                                }
                            }
                            _4223 = _4177;
                        }
                        else
                        {
                            _4223 = 65503.0;
                        }
                        _4227 = _4223 * float(_4223 < 65503.0);
                    }
                    else
                    {
                        _4227 = 0.0;
                    }
                    vec4 _4228 = vec4(0.0);
                    _4228.x = _4227;
                    vec4 _4234 = _4228;
                    _4234.y = (_4227 != 0.0) ? float(int(sign(_4227))) : 0.0;
                    vec4 _4235 = _4234;
                    _4235.z = 1.0;
                    ivec2 _4238 = ivec2(vec2(ivec2(_3931 + uvec2(0u, 1u))));
                    ivec2 _4242 = _4238;
                    _4242.x = max(0, min(_4238.x, _248));
                    ivec2 _4246 = _4242;
                    _4246.y = max(0, min(_4238.y, _254));
                    vec2 _4252 = (vec2(_4246) + vec2(0.5)) / _264;
                    float _181[5];
                    for (int _4258 = 0; _4258 < 5; )
                    {
                        ivec2 _4265 = ivec2(floor((_4252 + _268) * _264)) + _116[_4258];
                        ivec2 _4269 = _4265;
                        _4269.x = max(0, min(_4265.x, _248));
                        ivec2 _4273 = _4269;
                        _4273.y = max(0, min(_4265.y, _254));
                        vec4 _4279 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _4280 = _4279.x;
                        _181[_4258] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_4273)), int(0u)).x * ((_4280 == 0.0) ? 1.0 : _4280);
                        _181[_4258] = pow(_181[_4258], 1.0);
                        _181[_4258] = max(_181[_4258], 6.099999882280826568603515625e-05);
                        _4258++;
                        continue;
                    }
                    vec2 _4291 = (_4252 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_4246)), int(0u)).xy;
                    float _4292 = _4291.x;
                    float _4296 = _4291.y;
                    bool _4300 = ((_4292 >= 0.0) && (_4292 <= 1.0)) && ((_4296 >= 0.0) && (_4296 <= 1.0));
                    float _137[5];
                    if (_4300)
                    {
                        for (int _4307 = 0; _4307 < 5; )
                        {
                            ivec2 _4314 = ivec2(floor(_4291 * _310)) + _116[_4307];
                            ivec2 _4320 = _4314;
                            _4320.x = max(0, min(_4314.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _4326 = _4320;
                            _4326.y = max(0, min(_4314.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _4335 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _4336 = _4335.x;
                            _137[_4307] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_4326)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_4336 == 0.0) ? 1.0 : _4336);
                            _137[_4307] = pow(_137[_4307], 1.0);
                            _137[_4307] = max(_137[_4307], 6.099999882280826568603515625e-05);
                            _4307++;
                            continue;
                        }
                    }
                    float _4528;
                    if (_4300)
                    {
                        float _180[5] = _181;
                        float _179[5] = _137;
                        float _4349 = _180[0];
                        float _4351 = _180[3];
                        _180[3] = max(_180[0], _180[3]);
                        _180[0] = min(_4349, _4351);
                        float _4357 = _180[1];
                        float _4359 = _180[4];
                        _180[4] = max(_180[1], _180[4]);
                        _180[1] = min(_4357, _4359);
                        float _4364 = _180[0];
                        float _4366 = _180[2];
                        _180[2] = max(_180[0], _180[2]);
                        _180[0] = min(_4364, _4366);
                        float _4371 = _180[1];
                        float _4372 = _180[3];
                        _180[3] = max(_180[1], _180[3]);
                        _180[1] = min(_4371, _4372);
                        float _4377 = _180[0];
                        float _4378 = _180[1];
                        _180[1] = max(_180[0], _180[1]);
                        _180[0] = min(_4377, _4378);
                        float _4383 = _180[2];
                        float _4384 = _180[4];
                        _180[4] = max(_180[2], _180[4]);
                        _180[2] = min(_4383, _4384);
                        float _4389 = _180[1];
                        float _4390 = _180[2];
                        _180[2] = max(_180[1], _180[2]);
                        _180[1] = min(_4389, _4390);
                        float _4395 = _180[3];
                        float _4396 = _180[4];
                        _180[4] = max(_180[3], _180[4]);
                        _180[3] = min(_4395, _4396);
                        float _4401 = _180[2];
                        float _4402 = _180[3];
                        _180[3] = max(_180[2], _180[3]);
                        _180[2] = min(_4401, _4402);
                        float _4408 = _179[0];
                        float _4410 = _179[3];
                        _179[3] = max(_179[0], _179[3]);
                        _179[0] = min(_4408, _4410);
                        float _4416 = _179[1];
                        float _4418 = _179[4];
                        _179[4] = max(_179[1], _179[4]);
                        _179[1] = min(_4416, _4418);
                        float _4423 = _179[0];
                        float _4425 = _179[2];
                        _179[2] = max(_179[0], _179[2]);
                        _179[0] = min(_4423, _4425);
                        float _4430 = _179[1];
                        float _4431 = _179[3];
                        _179[3] = max(_179[1], _179[3]);
                        _179[1] = min(_4430, _4431);
                        float _4436 = _179[0];
                        float _4437 = _179[1];
                        _179[1] = max(_179[0], _179[1]);
                        _179[0] = min(_4436, _4437);
                        float _4442 = _179[2];
                        float _4443 = _179[4];
                        _179[4] = max(_179[2], _179[4]);
                        _179[2] = min(_4442, _4443);
                        float _4448 = _179[1];
                        float _4449 = _179[2];
                        _179[2] = max(_179[1], _179[2]);
                        _179[1] = min(_4448, _4449);
                        float _4454 = _179[3];
                        float _4455 = _179[4];
                        _179[4] = max(_179[3], _179[4]);
                        _179[3] = min(_4454, _4455);
                        float _4460 = _179[2];
                        float _4461 = _179[3];
                        _179[3] = max(_179[2], _179[3]);
                        _179[2] = min(_4460, _4461);
                        float _4524;
                        if (min(_180[4], _179[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _4478;
                            _4478 = 65503.0;
                            int _4474;
                            int _4477;
                            float _4479;
                            for (int _4473 = 0, _4476 = 0, _4480 = 0; (_4480 < 5) && (_4478 < 65504.0); _4473 = _4474, _4476 = _4477, _4478 = _4479, _4480++)
                            {
                                float _4491 = _180[_4476] - _179[_4473];
                                if (abs(_4491) > 6.099999882280826568603515625e-05)
                                {
                                    float _4502 = max(_180[_4476], _179[_4473]);
                                    float _4508 = float(int(sign(_4491))) * (1.0 - ((_4502 != 0.0) ? (min(_180[_4476], _179[_4473]) / _4502) : 0.0));
                                    int _4517 = _4476 + int(_180[_4476] < _179[_4473]);
                                    _4474 = _4473 + int(_180[_4517] >= _179[_4473]);
                                    _4477 = _4517;
                                    _4479 = (abs(_4508) < abs(_4478)) ? _4508 : _4478;
                                }
                                else
                                {
                                    _4474 = _4473;
                                    _4477 = _4476;
                                    _4479 = 65504.0;
                                }
                            }
                            _4524 = _4478;
                        }
                        else
                        {
                            _4524 = 65503.0;
                        }
                        _4528 = _4524 * float(_4524 < 65503.0);
                    }
                    else
                    {
                        _4528 = 0.0;
                    }
                    vec4 _4529 = vec4(0.0);
                    _4529.x = _4528;
                    vec4 _4535 = _4529;
                    _4535.y = (_4528 != 0.0) ? float(int(sign(_4528))) : 0.0;
                    vec4 _4536 = _4535;
                    _4536.z = 1.0;
                    ivec2 _4539 = ivec2(vec2(ivec2(_3931 + uvec2(1u, 0u))));
                    ivec2 _4543 = _4539;
                    _4543.x = max(0, min(_4539.x, _248));
                    ivec2 _4547 = _4543;
                    _4547.y = max(0, min(_4539.y, _254));
                    vec2 _4553 = (vec2(_4547) + vec2(0.5)) / _264;
                    float _184[5];
                    for (int _4559 = 0; _4559 < 5; )
                    {
                        ivec2 _4566 = ivec2(floor((_4553 + _268) * _264)) + _116[_4559];
                        ivec2 _4570 = _4566;
                        _4570.x = max(0, min(_4566.x, _248));
                        ivec2 _4574 = _4570;
                        _4574.y = max(0, min(_4566.y, _254));
                        vec4 _4580 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _4581 = _4580.x;
                        _184[_4559] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_4574)), int(0u)).x * ((_4581 == 0.0) ? 1.0 : _4581);
                        _184[_4559] = pow(_184[_4559], 1.0);
                        _184[_4559] = max(_184[_4559], 6.099999882280826568603515625e-05);
                        _4559++;
                        continue;
                    }
                    vec2 _4592 = (_4553 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_4547)), int(0u)).xy;
                    float _4593 = _4592.x;
                    float _4597 = _4592.y;
                    bool _4601 = ((_4593 >= 0.0) && (_4593 <= 1.0)) && ((_4597 >= 0.0) && (_4597 <= 1.0));
                    float _138[5];
                    if (_4601)
                    {
                        for (int _4608 = 0; _4608 < 5; )
                        {
                            ivec2 _4615 = ivec2(floor(_4592 * _310)) + _116[_4608];
                            ivec2 _4621 = _4615;
                            _4621.x = max(0, min(_4615.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _4627 = _4621;
                            _4627.y = max(0, min(_4615.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _4636 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _4637 = _4636.x;
                            _138[_4608] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_4627)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_4637 == 0.0) ? 1.0 : _4637);
                            _138[_4608] = pow(_138[_4608], 1.0);
                            _138[_4608] = max(_138[_4608], 6.099999882280826568603515625e-05);
                            _4608++;
                            continue;
                        }
                    }
                    float _4829;
                    if (_4601)
                    {
                        float _183[5] = _184;
                        float _182[5] = _138;
                        float _4650 = _183[0];
                        float _4652 = _183[3];
                        _183[3] = max(_183[0], _183[3]);
                        _183[0] = min(_4650, _4652);
                        float _4658 = _183[1];
                        float _4660 = _183[4];
                        _183[4] = max(_183[1], _183[4]);
                        _183[1] = min(_4658, _4660);
                        float _4665 = _183[0];
                        float _4667 = _183[2];
                        _183[2] = max(_183[0], _183[2]);
                        _183[0] = min(_4665, _4667);
                        float _4672 = _183[1];
                        float _4673 = _183[3];
                        _183[3] = max(_183[1], _183[3]);
                        _183[1] = min(_4672, _4673);
                        float _4678 = _183[0];
                        float _4679 = _183[1];
                        _183[1] = max(_183[0], _183[1]);
                        _183[0] = min(_4678, _4679);
                        float _4684 = _183[2];
                        float _4685 = _183[4];
                        _183[4] = max(_183[2], _183[4]);
                        _183[2] = min(_4684, _4685);
                        float _4690 = _183[1];
                        float _4691 = _183[2];
                        _183[2] = max(_183[1], _183[2]);
                        _183[1] = min(_4690, _4691);
                        float _4696 = _183[3];
                        float _4697 = _183[4];
                        _183[4] = max(_183[3], _183[4]);
                        _183[3] = min(_4696, _4697);
                        float _4702 = _183[2];
                        float _4703 = _183[3];
                        _183[3] = max(_183[2], _183[3]);
                        _183[2] = min(_4702, _4703);
                        float _4709 = _182[0];
                        float _4711 = _182[3];
                        _182[3] = max(_182[0], _182[3]);
                        _182[0] = min(_4709, _4711);
                        float _4717 = _182[1];
                        float _4719 = _182[4];
                        _182[4] = max(_182[1], _182[4]);
                        _182[1] = min(_4717, _4719);
                        float _4724 = _182[0];
                        float _4726 = _182[2];
                        _182[2] = max(_182[0], _182[2]);
                        _182[0] = min(_4724, _4726);
                        float _4731 = _182[1];
                        float _4732 = _182[3];
                        _182[3] = max(_182[1], _182[3]);
                        _182[1] = min(_4731, _4732);
                        float _4737 = _182[0];
                        float _4738 = _182[1];
                        _182[1] = max(_182[0], _182[1]);
                        _182[0] = min(_4737, _4738);
                        float _4743 = _182[2];
                        float _4744 = _182[4];
                        _182[4] = max(_182[2], _182[4]);
                        _182[2] = min(_4743, _4744);
                        float _4749 = _182[1];
                        float _4750 = _182[2];
                        _182[2] = max(_182[1], _182[2]);
                        _182[1] = min(_4749, _4750);
                        float _4755 = _182[3];
                        float _4756 = _182[4];
                        _182[4] = max(_182[3], _182[4]);
                        _182[3] = min(_4755, _4756);
                        float _4761 = _182[2];
                        float _4762 = _182[3];
                        _182[3] = max(_182[2], _182[3]);
                        _182[2] = min(_4761, _4762);
                        float _4825;
                        if (min(_183[4], _182[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _4779;
                            _4779 = 65503.0;
                            int _4775;
                            int _4778;
                            float _4780;
                            for (int _4774 = 0, _4777 = 0, _4781 = 0; (_4781 < 5) && (_4779 < 65504.0); _4774 = _4775, _4777 = _4778, _4779 = _4780, _4781++)
                            {
                                float _4792 = _183[_4777] - _182[_4774];
                                if (abs(_4792) > 6.099999882280826568603515625e-05)
                                {
                                    float _4803 = max(_183[_4777], _182[_4774]);
                                    float _4809 = float(int(sign(_4792))) * (1.0 - ((_4803 != 0.0) ? (min(_183[_4777], _182[_4774]) / _4803) : 0.0));
                                    int _4818 = _4777 + int(_183[_4777] < _182[_4774]);
                                    _4775 = _4774 + int(_183[_4818] >= _182[_4774]);
                                    _4778 = _4818;
                                    _4780 = (abs(_4809) < abs(_4779)) ? _4809 : _4779;
                                }
                                else
                                {
                                    _4775 = _4774;
                                    _4778 = _4777;
                                    _4780 = 65504.0;
                                }
                            }
                            _4825 = _4779;
                        }
                        else
                        {
                            _4825 = 65503.0;
                        }
                        _4829 = _4825 * float(_4825 < 65503.0);
                    }
                    else
                    {
                        _4829 = 0.0;
                    }
                    vec4 _4830 = vec4(0.0);
                    _4830.x = _4829;
                    vec4 _4836 = _4830;
                    _4836.y = (_4829 != 0.0) ? float(int(sign(_4829))) : 0.0;
                    vec4 _4837 = _4836;
                    _4837.z = 1.0;
                    ivec2 _4840 = ivec2(vec2(ivec2(_3931 + uvec2(1u))));
                    ivec2 _4844 = _4840;
                    _4844.x = max(0, min(_4840.x, _248));
                    ivec2 _4848 = _4844;
                    _4848.y = max(0, min(_4840.y, _254));
                    vec2 _4854 = (vec2(_4848) + vec2(0.5)) / _264;
                    float _187[5];
                    for (int _4860 = 0; _4860 < 5; )
                    {
                        ivec2 _4867 = ivec2(floor((_4854 + _268) * _264)) + _116[_4860];
                        ivec2 _4871 = _4867;
                        _4871.x = max(0, min(_4867.x, _248));
                        ivec2 _4875 = _4871;
                        _4875.y = max(0, min(_4867.y, _254));
                        vec4 _4881 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                        float _4882 = _4881.x;
                        _187[_4860] = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_4875)), int(0u)).x * ((_4882 == 0.0) ? 1.0 : _4882);
                        _187[_4860] = pow(_187[_4860], 1.0);
                        _187[_4860] = max(_187[_4860], 6.099999882280826568603515625e-05);
                        _4860++;
                        continue;
                    }
                    vec2 _4893 = (_4854 + _311) + texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(uvec2(_4848)), int(0u)).xy;
                    float _4894 = _4893.x;
                    float _4898 = _4893.y;
                    bool _4902 = ((_4894 >= 0.0) && (_4894 <= 1.0)) && ((_4898 >= 0.0) && (_4898 <= 1.0));
                    float _139[5];
                    if (_4902)
                    {
                        for (int _4909 = 0; _4909 < 5; )
                        {
                            ivec2 _4916 = ivec2(floor(_4893 * _310)) + _116[_4909];
                            ivec2 _4922 = _4916;
                            _4922.x = max(0, min(_4916.x, (cbFSR3Upscaler.iPreviousFrameRenderSize.x - 1)));
                            ivec2 _4928 = _4922;
                            _4928.y = max(0, min(_4916.y, (cbFSR3Upscaler.iPreviousFrameRenderSize.y - 1)));
                            vec4 _4937 = texelFetch(SPIRV_Cross_Combinedr_input_exposureSPIRV_Cross_DummySampler, ivec2(uvec2(0u)), int(0u));
                            float _4938 = _4937.x;
                            _139[_4909] = (texelFetch(SPIRV_Cross_Combinedr_previous_lumaSPIRV_Cross_DummySampler, ivec2(uvec2(_4928)), int(0u)).x * cbFSR3Upscaler.fDeltaPreExposure) * ((_4938 == 0.0) ? 1.0 : _4938);
                            _139[_4909] = pow(_139[_4909], 1.0);
                            _139[_4909] = max(_139[_4909], 6.099999882280826568603515625e-05);
                            _4909++;
                            continue;
                        }
                    }
                    float _5130;
                    if (_4902)
                    {
                        float _186[5] = _187;
                        float _185[5] = _139;
                        float _4951 = _186[0];
                        float _4953 = _186[3];
                        _186[3] = max(_186[0], _186[3]);
                        _186[0] = min(_4951, _4953);
                        float _4959 = _186[1];
                        float _4961 = _186[4];
                        _186[4] = max(_186[1], _186[4]);
                        _186[1] = min(_4959, _4961);
                        float _4966 = _186[0];
                        float _4968 = _186[2];
                        _186[2] = max(_186[0], _186[2]);
                        _186[0] = min(_4966, _4968);
                        float _4973 = _186[1];
                        float _4974 = _186[3];
                        _186[3] = max(_186[1], _186[3]);
                        _186[1] = min(_4973, _4974);
                        float _4979 = _186[0];
                        float _4980 = _186[1];
                        _186[1] = max(_186[0], _186[1]);
                        _186[0] = min(_4979, _4980);
                        float _4985 = _186[2];
                        float _4986 = _186[4];
                        _186[4] = max(_186[2], _186[4]);
                        _186[2] = min(_4985, _4986);
                        float _4991 = _186[1];
                        float _4992 = _186[2];
                        _186[2] = max(_186[1], _186[2]);
                        _186[1] = min(_4991, _4992);
                        float _4997 = _186[3];
                        float _4998 = _186[4];
                        _186[4] = max(_186[3], _186[4]);
                        _186[3] = min(_4997, _4998);
                        float _5003 = _186[2];
                        float _5004 = _186[3];
                        _186[3] = max(_186[2], _186[3]);
                        _186[2] = min(_5003, _5004);
                        float _5010 = _185[0];
                        float _5012 = _185[3];
                        _185[3] = max(_185[0], _185[3]);
                        _185[0] = min(_5010, _5012);
                        float _5018 = _185[1];
                        float _5020 = _185[4];
                        _185[4] = max(_185[1], _185[4]);
                        _185[1] = min(_5018, _5020);
                        float _5025 = _185[0];
                        float _5027 = _185[2];
                        _185[2] = max(_185[0], _185[2]);
                        _185[0] = min(_5025, _5027);
                        float _5032 = _185[1];
                        float _5033 = _185[3];
                        _185[3] = max(_185[1], _185[3]);
                        _185[1] = min(_5032, _5033);
                        float _5038 = _185[0];
                        float _5039 = _185[1];
                        _185[1] = max(_185[0], _185[1]);
                        _185[0] = min(_5038, _5039);
                        float _5044 = _185[2];
                        float _5045 = _185[4];
                        _185[4] = max(_185[2], _185[4]);
                        _185[2] = min(_5044, _5045);
                        float _5050 = _185[1];
                        float _5051 = _185[2];
                        _185[2] = max(_185[1], _185[2]);
                        _185[1] = min(_5050, _5051);
                        float _5056 = _185[3];
                        float _5057 = _185[4];
                        _185[4] = max(_185[3], _185[4]);
                        _185[3] = min(_5056, _5057);
                        float _5062 = _185[2];
                        float _5063 = _185[3];
                        _185[3] = max(_185[2], _185[3]);
                        _185[2] = min(_5062, _5063);
                        float _5126;
                        if (min(_186[4], _185[4]) > 1.1754943508222875079687365372222e-38)
                        {
                            float _5080;
                            _5080 = 65503.0;
                            int _5076;
                            int _5079;
                            float _5081;
                            for (int _5075 = 0, _5078 = 0, _5082 = 0; (_5082 < 5) && (_5080 < 65504.0); _5075 = _5076, _5078 = _5079, _5080 = _5081, _5082++)
                            {
                                float _5093 = _186[_5078] - _185[_5075];
                                if (abs(_5093) > 6.099999882280826568603515625e-05)
                                {
                                    float _5104 = max(_186[_5078], _185[_5075]);
                                    float _5110 = float(int(sign(_5093))) * (1.0 - ((_5104 != 0.0) ? (min(_186[_5078], _185[_5075]) / _5104) : 0.0));
                                    int _5119 = _5078 + int(_186[_5078] < _185[_5075]);
                                    _5076 = _5075 + int(_186[_5119] >= _185[_5075]);
                                    _5079 = _5119;
                                    _5081 = (abs(_5110) < abs(_5080)) ? _5110 : _5080;
                                }
                                else
                                {
                                    _5076 = _5075;
                                    _5079 = _5078;
                                    _5081 = 65504.0;
                                }
                            }
                            _5126 = _5080;
                        }
                        else
                        {
                            _5126 = 65503.0;
                        }
                        _5130 = _5126 * float(_5126 < 65503.0);
                    }
                    else
                    {
                        _5130 = 0.0;
                    }
                    vec4 _5131 = vec4(0.0);
                    _5131.x = _5130;
                    vec4 _5137 = _5131;
                    _5137.y = (_5130 != 0.0) ? float(int(sign(_5130))) : 0.0;
                    vec4 _5138 = _5137;
                    _5138.z = 1.0;
                    _188[3] = (((_4235 + _4536) + _4837) + _5138) * 0.25;
                    imageStore(rw_spd_mip0, ivec2(uvec2(_232 + ivec2(_1478, _2705))), vec4(_188[3].xy, 0.0, 0.0));
                    if (cbSPD.mips <= 1u)
                    {
                        break;
                    }
                    for (uint _5154 = 0u; _5154 < 4u; _5154++)
                    {
                        spdIntermediateR[_217][_220] = _188[_5154].x;
                        spdIntermediateG[_217][_220] = _188[_5154].y;
                        spdIntermediateB[_217][_220] = _188[_5154].z;
                        spdIntermediateA[_217][_220] = _188[_5154].w;
                        barrier();
                        if (gl_LocalInvocationIndex < 64u)
                        {
                            uint _5173 = _225 + 1u;
                            uint _5174 = _227 + 1u;
                            _188[_5154] = (((vec4(spdIntermediateR[_225][_227], spdIntermediateG[_225][_227], spdIntermediateB[_225][_227], spdIntermediateA[_225][_227]) + vec4(spdIntermediateR[_5173][_227], spdIntermediateG[_5173][_227], spdIntermediateB[_5173][_227], spdIntermediateA[_5173][_227])) + vec4(spdIntermediateR[_225][_5174], spdIntermediateG[_225][_5174], spdIntermediateB[_225][_5174], spdIntermediateA[_225][_5174])) + vec4(spdIntermediateR[_5173][_5174], spdIntermediateG[_5173][_5174], spdIntermediateB[_5173][_5174], spdIntermediateA[_5173][_5174])) * 0.25;
                            imageStore(rw_spd_mip1, ivec2(uvec2(ivec2(_198 * uvec2(16u)) + ivec2(int(_217 + ((_5154 % 2u) * 8u)), int(_220 + ((_5154 / 2u) * 8u))))), vec4(_188[_5154].xy, 0.0, 0.0));
                        }
                        barrier();
                    }
                    if (gl_LocalInvocationIndex < 64u)
                    {
                        spdIntermediateR[_217][_220] = _188[0].x;
                        spdIntermediateG[_217][_220] = _188[0].y;
                        spdIntermediateB[_217][_220] = _188[0].z;
                        spdIntermediateA[_217][_220] = _188[0].w;
                        uint _5245 = _217 + 8u;
                        spdIntermediateR[_5245][_220] = _188[1].x;
                        spdIntermediateG[_5245][_220] = _188[1].y;
                        spdIntermediateB[_5245][_220] = _188[1].z;
                        spdIntermediateA[_5245][_220] = _188[1].w;
                        uint _5255 = _220 + 8u;
                        spdIntermediateR[_217][_5255] = _188[2].x;
                        spdIntermediateG[_217][_5255] = _188[2].y;
                        spdIntermediateB[_217][_5255] = _188[2].z;
                        spdIntermediateA[_217][_5255] = _188[2].w;
                        spdIntermediateR[_5245][_5255] = _188[3].x;
                        spdIntermediateG[_5245][_5255] = _188[3].y;
                        spdIntermediateB[_5245][_5255] = _188[3].z;
                        spdIntermediateA[_5245][_5255] = _188[3].w;
                    }
                    break;
                }
            }
            switch (0u)
            {
                default:
                {
                    if (cbSPD.mips <= 2u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 64u)
                    {
                        uint _5282 = _225 + 1u;
                        uint _5283 = _227 + 1u;
                        float _5285 = spdIntermediateR[_225][_227];
                        float _5287 = spdIntermediateG[_225][_227];
                        float _5289 = spdIntermediateB[_225][_227];
                        float _5294 = spdIntermediateR[_5282][_227];
                        float _5296 = spdIntermediateG[_5282][_227];
                        float _5298 = spdIntermediateB[_5282][_227];
                        float _5303 = spdIntermediateR[_225][_5283];
                        float _5305 = spdIntermediateG[_225][_5283];
                        float _5307 = spdIntermediateB[_225][_5283];
                        float _5312 = spdIntermediateR[_5282][_5283];
                        float _5314 = spdIntermediateG[_5282][_5283];
                        float _5316 = spdIntermediateB[_5282][_5283];
                        vec4 _5323 = (((vec4(_5285, _5287, _5289, spdIntermediateA[_225][_227]) + vec4(_5294, _5296, _5298, spdIntermediateA[_5282][_227])) + vec4(_5303, _5305, _5307, spdIntermediateA[_225][_5283])) + vec4(_5312, _5314, _5316, spdIntermediateA[_5282][_5283])) * 0.25;
                        float _5327 = _5323.x;
                        imageStore(rw_spd_mip2, ivec2(uvec2(ivec2(_198 * uvec2(8u)) + _235)), vec4(_5327, _5323.y, 0.0, 0.0));
                        uint _5333 = _225 + (_220 % 2u);
                        spdIntermediateR[_5333][_227] = _5327;
                        spdIntermediateG[_5333][_227] = _5323.y;
                        spdIntermediateB[_5333][_227] = _5323.z;
                        spdIntermediateA[_5333][_227] = _5323.w;
                    }
                    if (cbSPD.mips <= 3u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 16u)
                    {
                        uint _5346 = _217 * 4u;
                        uint _5347 = _220 * 4u;
                        uint _5348 = _5346 + 2u;
                        uint _5349 = _5346 + 1u;
                        uint _5350 = _5347 + 2u;
                        uint _5351 = _5346 + 3u;
                        float _5353 = spdIntermediateR[_5346][_5347];
                        float _5355 = spdIntermediateG[_5346][_5347];
                        float _5357 = spdIntermediateB[_5346][_5347];
                        float _5362 = spdIntermediateR[_5348][_5347];
                        float _5364 = spdIntermediateG[_5348][_5347];
                        float _5366 = spdIntermediateB[_5348][_5347];
                        float _5371 = spdIntermediateR[_5349][_5350];
                        float _5373 = spdIntermediateG[_5349][_5350];
                        float _5375 = spdIntermediateB[_5349][_5350];
                        float _5380 = spdIntermediateR[_5351][_5350];
                        float _5382 = spdIntermediateG[_5351][_5350];
                        float _5384 = spdIntermediateB[_5351][_5350];
                        vec4 _5391 = (((vec4(_5353, _5355, _5357, spdIntermediateA[_5346][_5347]) + vec4(_5362, _5364, _5366, spdIntermediateA[_5348][_5347])) + vec4(_5371, _5373, _5375, spdIntermediateA[_5349][_5350])) + vec4(_5380, _5382, _5384, spdIntermediateA[_5351][_5350])) * 0.25;
                        float _5395 = _5391.x;
                        imageStore(rw_spd_mip3, ivec2(uvec2(ivec2(_198 * uvec2(4u)) + _235)), vec4(_5395, _5391.y, 0.0, 0.0));
                        uint _5400 = _5346 + _220;
                        spdIntermediateR[_5400][_5347] = _5395;
                        spdIntermediateG[_5400][_5347] = _5391.y;
                        spdIntermediateB[_5400][_5347] = _5391.z;
                        spdIntermediateA[_5400][_5347] = _5391.w;
                    }
                    if (cbSPD.mips <= 4u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 4u)
                    {
                        uint _5413 = _217 * 8u;
                        uint _5414 = _5413 + _227;
                        uint _5415 = _220 * 8u;
                        uint _5417 = (_5413 + 4u) + _227;
                        uint _5419 = (_5413 + 1u) + _227;
                        uint _5420 = _5415 + 4u;
                        uint _5422 = (_5413 + 5u) + _227;
                        float _5424 = spdIntermediateR[_5414][_5415];
                        float _5426 = spdIntermediateG[_5414][_5415];
                        float _5428 = spdIntermediateB[_5414][_5415];
                        float _5433 = spdIntermediateR[_5417][_5415];
                        float _5435 = spdIntermediateG[_5417][_5415];
                        float _5437 = spdIntermediateB[_5417][_5415];
                        float _5442 = spdIntermediateR[_5419][_5420];
                        float _5444 = spdIntermediateG[_5419][_5420];
                        float _5446 = spdIntermediateB[_5419][_5420];
                        float _5451 = spdIntermediateR[_5422][_5420];
                        float _5453 = spdIntermediateG[_5422][_5420];
                        float _5455 = spdIntermediateB[_5422][_5420];
                        vec4 _5462 = (((vec4(_5424, _5426, _5428, spdIntermediateA[_5414][_5415]) + vec4(_5433, _5435, _5437, spdIntermediateA[_5417][_5415])) + vec4(_5442, _5444, _5446, spdIntermediateA[_5419][_5420])) + vec4(_5451, _5453, _5455, spdIntermediateA[_5422][_5420])) * 0.25;
                        float _5466 = _5462.x;
                        imageStore(rw_spd_mip4, ivec2(uvec2(ivec2(_198 * uvec2(2u)) + _235)), vec4(_5466, _5462.y, 0.0, 0.0));
                        uint _5471 = _217 + _227;
                        spdIntermediateR[_5471][0u] = _5466;
                        spdIntermediateG[_5471][0u] = _5462.y;
                        spdIntermediateB[_5471][0u] = _5462.z;
                        spdIntermediateA[_5471][0u] = _5462.w;
                    }
                    if (cbSPD.mips <= 5u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 1u)
                    {
                        imageStore(w_spd_mip5, ivec2(uvec2(ivec2(_198))), vec4(((((vec4(spdIntermediateR[0u][0u], spdIntermediateG[0u][0u], _122, _122) + vec4(spdIntermediateR[1u][0u], spdIntermediateG[1u][0u], _122, _122)) + vec4(spdIntermediateR[2u][0u], spdIntermediateG[2u][0u], _122, _122)) + vec4(spdIntermediateR[3u][0u], spdIntermediateG[3u][0u], _122, _122)) * 0.25).xy, 0.0, 0.0));
                    }
                    break;
                }
            }
            if (cbSPD.mips <= 6u)
            {
                break;
            }
            if (gl_LocalInvocationIndex == 0u)
            {
                uint _5522 = imageAtomicAdd(rw_spd_global_atomic, ivec2(uvec2(ivec2(0))), 1u);
                spdCounter = _5522;
            }
            barrier();
            if (spdCounter != (cbSPD.numWorkGroups - 1u))
            {
                break;
            }
            uint _5532;
            uint _5534;
            uint _5573;
            uint _5612;
            imageStore(rw_spd_global_atomic, ivec2(uvec2(ivec2(0))), uvec4(0u));
            switch (0u)
            {
                default:
                {
                    _5532 = _217 * 4u;
                    int _5533 = int(_5532);
                    _5534 = _220 * 4u;
                    int _5535 = int(_5534);
                    uvec2 _5537 = uvec2(ivec2(_5533, _5535));
                    _5573 = _5532 + 2u;
                    int _5574 = int(_5573);
                    uvec2 _5576 = uvec2(ivec2(_5574, _5535));
                    _5612 = _5534 + 2u;
                    int _5613 = int(_5612);
                    uvec2 _5615 = uvec2(ivec2(_5533, _5613));
                    uvec2 _5652 = uvec2(ivec2(_5574, _5613));
                    if (cbSPD.mips <= 7u)
                    {
                        break;
                    }
                    vec4 _5694 = (((((((vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5537)))).xy, 0.0, 0.0) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5537 + uvec2(0u, 1u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5537 + uvec2(1u, 0u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5537 + uvec2(1u))))).xy, 0.0, 0.0)) * 0.25) + ((((vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5576)))).xy, 0.0, 0.0) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5576 + uvec2(0u, 1u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5576 + uvec2(1u, 0u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5576 + uvec2(1u))))).xy, 0.0, 0.0)) * 0.25)) + ((((vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5615)))).xy, 0.0, 0.0) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5615 + uvec2(0u, 1u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5615 + uvec2(1u, 0u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5615 + uvec2(1u))))).xy, 0.0, 0.0)) * 0.25)) + ((((vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5652)))).xy, 0.0, 0.0) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5652 + uvec2(0u, 1u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5652 + uvec2(1u, 0u))))).xy, 0.0, 0.0)) + vec4(imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_5652 + uvec2(1u))))).xy, 0.0, 0.0)) * 0.25)) * 0.25;
                    spdIntermediateR[_217][_220] = _5694.x;
                    spdIntermediateG[_217][_220] = _5694.y;
                    spdIntermediateB[_217][_220] = _5694.z;
                    spdIntermediateA[_217][_220] = _5694.w;
                    break;
                }
            }
            switch (0u)
            {
                default:
                {
                    if (cbSPD.mips <= 8u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 64u)
                    {
                        uint _5711 = _225 + 1u;
                        uint _5712 = _227 + 1u;
                        float _5714 = spdIntermediateR[_225][_227];
                        float _5716 = spdIntermediateG[_225][_227];
                        float _5718 = spdIntermediateB[_225][_227];
                        float _5723 = spdIntermediateR[_5711][_227];
                        float _5725 = spdIntermediateG[_5711][_227];
                        float _5727 = spdIntermediateB[_5711][_227];
                        float _5732 = spdIntermediateR[_225][_5712];
                        float _5734 = spdIntermediateG[_225][_5712];
                        float _5736 = spdIntermediateB[_225][_5712];
                        float _5741 = spdIntermediateR[_5711][_5712];
                        float _5743 = spdIntermediateG[_5711][_5712];
                        float _5745 = spdIntermediateB[_5711][_5712];
                        vec4 _5752 = (((vec4(_5714, _5716, _5718, spdIntermediateA[_225][_227]) + vec4(_5723, _5725, _5727, spdIntermediateA[_5711][_227])) + vec4(_5732, _5734, _5736, spdIntermediateA[_225][_5712])) + vec4(_5741, _5743, _5745, spdIntermediateA[_5711][_5712])) * 0.25;
                        uint _5754 = _225 + (_220 % 2u);
                        spdIntermediateR[_5754][_227] = _5752.x;
                        spdIntermediateG[_5754][_227] = _5752.y;
                        spdIntermediateB[_5754][_227] = _5752.z;
                        spdIntermediateA[_5754][_227] = _5752.w;
                    }
                    if (cbSPD.mips <= 9u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 16u)
                    {
                        uint _5769 = _5532 + 1u;
                        uint _5770 = _5532 + 3u;
                        float _5772 = spdIntermediateR[_5532][_5534];
                        float _5774 = spdIntermediateG[_5532][_5534];
                        float _5776 = spdIntermediateB[_5532][_5534];
                        float _5781 = spdIntermediateR[_5573][_5534];
                        float _5783 = spdIntermediateG[_5573][_5534];
                        float _5785 = spdIntermediateB[_5573][_5534];
                        float _5790 = spdIntermediateR[_5769][_5612];
                        float _5792 = spdIntermediateG[_5769][_5612];
                        float _5794 = spdIntermediateB[_5769][_5612];
                        float _5799 = spdIntermediateR[_5770][_5612];
                        float _5801 = spdIntermediateG[_5770][_5612];
                        float _5803 = spdIntermediateB[_5770][_5612];
                        vec4 _5810 = (((vec4(_5772, _5774, _5776, spdIntermediateA[_5532][_5534]) + vec4(_5781, _5783, _5785, spdIntermediateA[_5573][_5534])) + vec4(_5790, _5792, _5794, spdIntermediateA[_5769][_5612])) + vec4(_5799, _5801, _5803, spdIntermediateA[_5770][_5612])) * 0.25;
                        uint _5811 = _5532 + _220;
                        spdIntermediateR[_5811][_5534] = _5810.x;
                        spdIntermediateG[_5811][_5534] = _5810.y;
                        spdIntermediateB[_5811][_5534] = _5810.z;
                        spdIntermediateA[_5811][_5534] = _5810.w;
                    }
                    if (cbSPD.mips <= 10u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 4u)
                    {
                        uint _5826 = _217 * 8u;
                        uint _5827 = _5826 + _227;
                        uint _5828 = _220 * 8u;
                        uint _5830 = (_5826 + 4u) + _227;
                        uint _5832 = (_5826 + 1u) + _227;
                        uint _5833 = _5828 + 4u;
                        uint _5835 = (_5826 + 5u) + _227;
                        float _5837 = spdIntermediateR[_5827][_5828];
                        float _5839 = spdIntermediateG[_5827][_5828];
                        float _5841 = spdIntermediateB[_5827][_5828];
                        float _5846 = spdIntermediateR[_5830][_5828];
                        float _5848 = spdIntermediateG[_5830][_5828];
                        float _5850 = spdIntermediateB[_5830][_5828];
                        float _5855 = spdIntermediateR[_5832][_5833];
                        float _5857 = spdIntermediateG[_5832][_5833];
                        float _5859 = spdIntermediateB[_5832][_5833];
                        float _5864 = spdIntermediateR[_5835][_5833];
                        float _5866 = spdIntermediateG[_5835][_5833];
                        float _5868 = spdIntermediateB[_5835][_5833];
                        vec4 _5875 = (((vec4(_5837, _5839, _5841, spdIntermediateA[_5827][_5828]) + vec4(_5846, _5848, _5850, spdIntermediateA[_5830][_5828])) + vec4(_5855, _5857, _5859, spdIntermediateA[_5832][_5833])) + vec4(_5864, _5866, _5868, spdIntermediateA[_5835][_5833])) * 0.25;
                        uint _5876 = _217 + _227;
                        spdIntermediateR[_5876][0u] = _5875.x;
                        spdIntermediateG[_5876][0u] = _5875.y;
                        spdIntermediateB[_5876][0u] = _5875.z;
                        spdIntermediateA[_5876][0u] = _5875.w;
                    }
                    if (cbSPD.mips <= 11u)
                    {
                        break;
                    }
                    barrier();
                    break;
                }
            }
            break;
        }
    }
}

