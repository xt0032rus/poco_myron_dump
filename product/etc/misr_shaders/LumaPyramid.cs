#extension GL_OES_shader_image_atomic : require
layout(local_size_x = 256, local_size_y = 1, local_size_z = 1) in;

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

layout(binding = 2, r32f) uniform writeonly highp image2D rw_farthest_depth_mip1;
layout(binding = 1, rgba8) uniform readonly highp image2D r_frame_info;
layout(binding = 1, rgba8) uniform writeonly highp image2D w_frame_info;
layout(binding = 7, rgba8) uniform readonly highp image2D r_spd_mip5;
layout(binding = 7, rgba8) uniform writeonly highp image2D w_spd_mip5;
layout(binding = 3, r32ui) uniform highp uimage2D rw_test_global_atomic;
layout(binding = 0, r32ui) uniform highp uimage2D rw_spd_global_atomic;
uniform highp sampler2D SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler;

shared uint spdCounter;
shared float spdIntermediateR[16][16];
shared float spdIntermediateG[16][16];
shared float spdIntermediateB[16][16];
shared float spdIntermediateA[16][16];

float _103;

void main()
{
    uvec2 _115 = gl_WorkGroupID.xy + cbSPD.workGroupOffset;
    switch (0u)
    {
        default:
        {
            uint _142;
            int _143;
            uint _144;
            int _145;
            ivec2 _146;
            ivec2 _152;
            uint _259;
            uint _118 = gl_LocalInvocationIndex % 64u;
            uint _134 = ((_118 & 1u) | (((_118 >> 2u) & 7u) & 4294967294u)) + (8u * ((gl_LocalInvocationIndex >> 6u) % 2u));
            uint _137 = ((((_118 >> 1u) & 3u) & 3u) | (((_118 >> 3u) & 7u) & 4294967292u)) + (8u * (gl_LocalInvocationIndex >> 7u));
            switch (0u)
            {
                default:
                {
                    ivec2 _141 = ivec2(_115 * uvec2(64u));
                    _142 = _134 * 2u;
                    _143 = int(_142);
                    _144 = _137 * 2u;
                    _145 = int(_144);
                    _146 = ivec2(_143, _145);
                    ivec2 _149 = ivec2(_115 * uvec2(32u));
                    int _150 = int(_134);
                    int _151 = int(_137);
                    _152 = ivec2(_150, _151);
                    ivec2 _153 = _149 + _152;
                    uvec2 _154 = uvec2(_141 + _146);
                    ivec2 _160 = ivec2(vec2(ivec2(_154)));
                    int _165 = cbFSR3Upscaler.iRenderSize.x - 1;
                    ivec2 _168 = _160;
                    _168.x = max(0, min(_160.x, _165));
                    int _171 = cbFSR3Upscaler.iRenderSize.y - 1;
                    ivec2 _174 = _168;
                    _174.y = max(0, min(_160.y, _171));
                    uvec2 _175 = uvec2(_174);
                    vec4 _177 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_175), int(0u));
                    float _178 = _177.x;
                    vec4 _184 = vec4(0.0);
                    _184.x = max(6.099999882280826568603515625e-05, log(_178));
                    vec4 _185 = _184;
                    _185.y = _178;
                    vec4 _186 = _185;
                    _186.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_175), int(0u)).x;
                    ivec2 _189 = ivec2(vec2(ivec2(_154 + uvec2(0u, 1u))));
                    ivec2 _193 = _189;
                    _193.x = max(0, min(_189.x, _165));
                    ivec2 _197 = _193;
                    _197.y = max(0, min(_189.y, _171));
                    uvec2 _198 = uvec2(_197);
                    vec4 _199 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_198), int(0u));
                    float _200 = _199.x;
                    vec4 _205 = vec4(0.0);
                    _205.x = max(6.099999882280826568603515625e-05, log(_200));
                    vec4 _206 = _205;
                    _206.y = _200;
                    vec4 _207 = _206;
                    _207.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_198), int(0u)).x;
                    ivec2 _210 = ivec2(vec2(ivec2(_154 + uvec2(1u, 0u))));
                    ivec2 _214 = _210;
                    _214.x = max(0, min(_210.x, _165));
                    ivec2 _218 = _214;
                    _218.y = max(0, min(_210.y, _171));
                    uvec2 _219 = uvec2(_218);
                    vec4 _220 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_219), int(0u));
                    float _221 = _220.x;
                    vec4 _226 = vec4(0.0);
                    _226.x = max(6.099999882280826568603515625e-05, log(_221));
                    vec4 _227 = _226;
                    _227.y = _221;
                    vec4 _228 = _227;
                    _228.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_219), int(0u)).x;
                    ivec2 _231 = ivec2(vec2(ivec2(_154 + uvec2(1u))));
                    ivec2 _235 = _231;
                    _235.x = max(0, min(_231.x, _165));
                    ivec2 _239 = _235;
                    _239.y = max(0, min(_231.y, _171));
                    uvec2 _240 = uvec2(_239);
                    vec4 _241 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_240), int(0u));
                    float _242 = _241.x;
                    vec4 _247 = vec4(0.0);
                    _247.x = max(6.099999882280826568603515625e-05, log(_242));
                    vec4 _248 = _247;
                    _248.y = _242;
                    vec4 _249 = _248;
                    _249.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_240), int(0u)).x;
                    vec4 _105[4];
                    _105[0] = (((_186 + _207) + _228) + _249) * 0.25;
                    imageStore(rw_farthest_depth_mip1, ivec2(uvec2(_153)), vec4(_105[0].z));
                    _259 = cbSPD.mips - 1u;
                    bool _260 = 0u == _259;
                    if (_260)
                    {
                        if (all(equal(_153, ivec2(0))))
                        {
                            uvec2 _267 = uvec2(ivec2(0));
                            vec4 _269 = imageLoad(r_frame_info, ivec2(_267));
                            float _271 = _269.y;
                            float _285;
                            if (_271 < 10000.0)
                            {
                                _285 = max(0.0, _271 + ((_105[0].x - _271) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _285 = _105[0].x;
                            }
                            vec4 _291 = _269;
                            _291.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_285) * 8.0));
                            vec4 _292 = _291;
                            _292.y = _285;
                            vec4 _293 = _292;
                            _293.z = _105[0].y;
                            imageStore(w_frame_info, ivec2(_267), _293);
                        }
                    }
                    int _296 = int(_142 + 32u);
                    int _300 = int(_134 + 16u);
                    ivec2 _302 = _149 + ivec2(_300, _151);
                    uvec2 _303 = uvec2(_141 + ivec2(_296, _145));
                    ivec2 _309 = ivec2(vec2(ivec2(_303)));
                    ivec2 _313 = _309;
                    _313.x = max(0, min(_309.x, _165));
                    ivec2 _317 = _313;
                    _317.y = max(0, min(_309.y, _171));
                    uvec2 _318 = uvec2(_317);
                    vec4 _319 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_318), int(0u));
                    float _320 = _319.x;
                    vec4 _325 = vec4(0.0);
                    _325.x = max(6.099999882280826568603515625e-05, log(_320));
                    vec4 _326 = _325;
                    _326.y = _320;
                    vec4 _327 = _326;
                    _327.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_318), int(0u)).x;
                    ivec2 _330 = ivec2(vec2(ivec2(_303 + uvec2(0u, 1u))));
                    ivec2 _334 = _330;
                    _334.x = max(0, min(_330.x, _165));
                    ivec2 _338 = _334;
                    _338.y = max(0, min(_330.y, _171));
                    uvec2 _339 = uvec2(_338);
                    vec4 _340 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_339), int(0u));
                    float _341 = _340.x;
                    vec4 _346 = vec4(0.0);
                    _346.x = max(6.099999882280826568603515625e-05, log(_341));
                    vec4 _347 = _346;
                    _347.y = _341;
                    vec4 _348 = _347;
                    _348.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_339), int(0u)).x;
                    ivec2 _351 = ivec2(vec2(ivec2(_303 + uvec2(1u, 0u))));
                    ivec2 _355 = _351;
                    _355.x = max(0, min(_351.x, _165));
                    ivec2 _359 = _355;
                    _359.y = max(0, min(_351.y, _171));
                    uvec2 _360 = uvec2(_359);
                    vec4 _361 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_360), int(0u));
                    float _362 = _361.x;
                    vec4 _367 = vec4(0.0);
                    _367.x = max(6.099999882280826568603515625e-05, log(_362));
                    vec4 _368 = _367;
                    _368.y = _362;
                    vec4 _369 = _368;
                    _369.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_360), int(0u)).x;
                    ivec2 _372 = ivec2(vec2(ivec2(_303 + uvec2(1u))));
                    ivec2 _376 = _372;
                    _376.x = max(0, min(_372.x, _165));
                    ivec2 _380 = _376;
                    _380.y = max(0, min(_372.y, _171));
                    uvec2 _381 = uvec2(_380);
                    vec4 _382 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_381), int(0u));
                    float _383 = _382.x;
                    vec4 _388 = vec4(0.0);
                    _388.x = max(6.099999882280826568603515625e-05, log(_383));
                    vec4 _389 = _388;
                    _389.y = _383;
                    vec4 _390 = _389;
                    _390.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_381), int(0u)).x;
                    _105[1] = (((_327 + _348) + _369) + _390) * 0.25;
                    imageStore(rw_farthest_depth_mip1, ivec2(uvec2(_302)), vec4(_105[1].z));
                    if (_260)
                    {
                        if (all(equal(_302, ivec2(0))))
                        {
                            uvec2 _406 = uvec2(ivec2(0));
                            vec4 _408 = imageLoad(r_frame_info, ivec2(_406));
                            float _410 = _408.y;
                            float _424;
                            if (_410 < 10000.0)
                            {
                                _424 = max(0.0, _410 + ((_105[1].x - _410) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _424 = _105[1].x;
                            }
                            vec4 _430 = _408;
                            _430.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_424) * 8.0));
                            vec4 _431 = _430;
                            _431.y = _424;
                            vec4 _432 = _431;
                            _432.z = _105[1].y;
                            imageStore(w_frame_info, ivec2(_406), _432);
                        }
                    }
                    int _435 = int(_144 + 32u);
                    int _439 = int(_137 + 16u);
                    ivec2 _441 = _149 + ivec2(_150, _439);
                    uvec2 _442 = uvec2(_141 + ivec2(_143, _435));
                    ivec2 _448 = ivec2(vec2(ivec2(_442)));
                    ivec2 _452 = _448;
                    _452.x = max(0, min(_448.x, _165));
                    ivec2 _456 = _452;
                    _456.y = max(0, min(_448.y, _171));
                    uvec2 _457 = uvec2(_456);
                    vec4 _458 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_457), int(0u));
                    float _459 = _458.x;
                    vec4 _464 = vec4(0.0);
                    _464.x = max(6.099999882280826568603515625e-05, log(_459));
                    vec4 _465 = _464;
                    _465.y = _459;
                    vec4 _466 = _465;
                    _466.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_457), int(0u)).x;
                    ivec2 _469 = ivec2(vec2(ivec2(_442 + uvec2(0u, 1u))));
                    ivec2 _473 = _469;
                    _473.x = max(0, min(_469.x, _165));
                    ivec2 _477 = _473;
                    _477.y = max(0, min(_469.y, _171));
                    uvec2 _478 = uvec2(_477);
                    vec4 _479 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_478), int(0u));
                    float _480 = _479.x;
                    vec4 _485 = vec4(0.0);
                    _485.x = max(6.099999882280826568603515625e-05, log(_480));
                    vec4 _486 = _485;
                    _486.y = _480;
                    vec4 _487 = _486;
                    _487.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_478), int(0u)).x;
                    ivec2 _490 = ivec2(vec2(ivec2(_442 + uvec2(1u, 0u))));
                    ivec2 _494 = _490;
                    _494.x = max(0, min(_490.x, _165));
                    ivec2 _498 = _494;
                    _498.y = max(0, min(_490.y, _171));
                    uvec2 _499 = uvec2(_498);
                    vec4 _500 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_499), int(0u));
                    float _501 = _500.x;
                    vec4 _506 = vec4(0.0);
                    _506.x = max(6.099999882280826568603515625e-05, log(_501));
                    vec4 _507 = _506;
                    _507.y = _501;
                    vec4 _508 = _507;
                    _508.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_499), int(0u)).x;
                    ivec2 _511 = ivec2(vec2(ivec2(_442 + uvec2(1u))));
                    ivec2 _515 = _511;
                    _515.x = max(0, min(_511.x, _165));
                    ivec2 _519 = _515;
                    _519.y = max(0, min(_511.y, _171));
                    uvec2 _520 = uvec2(_519);
                    vec4 _521 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_520), int(0u));
                    float _522 = _521.x;
                    vec4 _527 = vec4(0.0);
                    _527.x = max(6.099999882280826568603515625e-05, log(_522));
                    vec4 _528 = _527;
                    _528.y = _522;
                    vec4 _529 = _528;
                    _529.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_520), int(0u)).x;
                    _105[2] = (((_466 + _487) + _508) + _529) * 0.25;
                    imageStore(rw_farthest_depth_mip1, ivec2(uvec2(_441)), vec4(_105[2].z));
                    if (_260)
                    {
                        if (all(equal(_441, ivec2(0))))
                        {
                            uvec2 _545 = uvec2(ivec2(0));
                            vec4 _547 = imageLoad(r_frame_info, ivec2(_545));
                            float _549 = _547.y;
                            float _563;
                            if (_549 < 10000.0)
                            {
                                _563 = max(0.0, _549 + ((_105[2].x - _549) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _563 = _105[2].x;
                            }
                            vec4 _569 = _547;
                            _569.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_563) * 8.0));
                            vec4 _570 = _569;
                            _570.y = _563;
                            vec4 _571 = _570;
                            _571.z = _105[2].y;
                            imageStore(w_frame_info, ivec2(_545), _571);
                        }
                    }
                    ivec2 _576 = _149 + ivec2(_300, _439);
                    uvec2 _577 = uvec2(_141 + ivec2(_296, _435));
                    ivec2 _583 = ivec2(vec2(ivec2(_577)));
                    ivec2 _587 = _583;
                    _587.x = max(0, min(_583.x, _165));
                    ivec2 _591 = _587;
                    _591.y = max(0, min(_583.y, _171));
                    uvec2 _592 = uvec2(_591);
                    vec4 _593 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_592), int(0u));
                    float _594 = _593.x;
                    vec4 _599 = vec4(0.0);
                    _599.x = max(6.099999882280826568603515625e-05, log(_594));
                    vec4 _600 = _599;
                    _600.y = _594;
                    vec4 _601 = _600;
                    _601.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_592), int(0u)).x;
                    ivec2 _604 = ivec2(vec2(ivec2(_577 + uvec2(0u, 1u))));
                    ivec2 _608 = _604;
                    _608.x = max(0, min(_604.x, _165));
                    ivec2 _612 = _608;
                    _612.y = max(0, min(_604.y, _171));
                    uvec2 _613 = uvec2(_612);
                    vec4 _614 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_613), int(0u));
                    float _615 = _614.x;
                    vec4 _620 = vec4(0.0);
                    _620.x = max(6.099999882280826568603515625e-05, log(_615));
                    vec4 _621 = _620;
                    _621.y = _615;
                    vec4 _622 = _621;
                    _622.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_613), int(0u)).x;
                    ivec2 _625 = ivec2(vec2(ivec2(_577 + uvec2(1u, 0u))));
                    ivec2 _629 = _625;
                    _629.x = max(0, min(_625.x, _165));
                    ivec2 _633 = _629;
                    _633.y = max(0, min(_625.y, _171));
                    uvec2 _634 = uvec2(_633);
                    vec4 _635 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_634), int(0u));
                    float _636 = _635.x;
                    vec4 _641 = vec4(0.0);
                    _641.x = max(6.099999882280826568603515625e-05, log(_636));
                    vec4 _642 = _641;
                    _642.y = _636;
                    vec4 _643 = _642;
                    _643.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_634), int(0u)).x;
                    ivec2 _646 = ivec2(vec2(ivec2(_577 + uvec2(1u))));
                    ivec2 _650 = _646;
                    _650.x = max(0, min(_646.x, _165));
                    ivec2 _654 = _650;
                    _654.y = max(0, min(_646.y, _171));
                    uvec2 _655 = uvec2(_654);
                    vec4 _656 = texelFetch(SPIRV_Cross_Combinedr_current_lumaSPIRV_Cross_DummySampler, ivec2(_655), int(0u));
                    float _657 = _656.x;
                    vec4 _662 = vec4(0.0);
                    _662.x = max(6.099999882280826568603515625e-05, log(_657));
                    vec4 _663 = _662;
                    _663.y = _657;
                    vec4 _664 = _663;
                    _664.z = texelFetch(SPIRV_Cross_Combinedr_farthest_depthSPIRV_Cross_DummySampler, ivec2(_655), int(0u)).x;
                    _105[3] = (((_601 + _622) + _643) + _664) * 0.25;
                    imageStore(rw_farthest_depth_mip1, ivec2(uvec2(_576)), vec4(_105[3].z));
                    if (_260)
                    {
                        if (all(equal(_576, ivec2(0))))
                        {
                            uvec2 _680 = uvec2(ivec2(0));
                            vec4 _682 = imageLoad(r_frame_info, ivec2(_680));
                            float _684 = _682.y;
                            float _698;
                            if (_684 < 10000.0)
                            {
                                _698 = max(0.0, _684 + ((_105[3].x - _684) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _698 = _105[3].x;
                            }
                            vec4 _704 = _682;
                            _704.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_698) * 8.0));
                            vec4 _705 = _704;
                            _705.y = _698;
                            vec4 _706 = _705;
                            _706.z = _105[3].y;
                            imageStore(w_frame_info, ivec2(_680), _706);
                        }
                    }
                    if (cbSPD.mips <= 1u)
                    {
                        break;
                    }
                    for (uint _712 = 0u; _712 < 4u; _712++)
                    {
                        spdIntermediateR[_134][_137] = _105[_712].x;
                        spdIntermediateG[_134][_137] = _105[_712].y;
                        spdIntermediateB[_134][_137] = _105[_712].z;
                        spdIntermediateA[_134][_137] = _105[_712].w;
                        barrier();
                        if (gl_LocalInvocationIndex < 64u)
                        {
                            uint _731 = _142 + 1u;
                            uint _732 = _144 + 1u;
                            _105[_712] = (((vec4(spdIntermediateR[_142][_144], spdIntermediateG[_142][_144], spdIntermediateB[_142][_144], spdIntermediateA[_142][_144]) + vec4(spdIntermediateR[_731][_144], spdIntermediateG[_731][_144], spdIntermediateB[_731][_144], spdIntermediateA[_731][_144])) + vec4(spdIntermediateR[_142][_732], spdIntermediateG[_142][_732], spdIntermediateB[_142][_732], spdIntermediateA[_142][_732])) + vec4(spdIntermediateR[_731][_732], spdIntermediateG[_731][_732], spdIntermediateB[_731][_732], spdIntermediateA[_731][_732])) * 0.25;
                            if (1u == _259)
                            {
                                if (all(equal((ivec2(_115 * uvec2(16u)) + ivec2(int(_134 + ((_712 % 2u) * 8u)), int(_137 + ((_712 / 2u) * 8u)))), ivec2(0))))
                                {
                                    uvec2 _793 = uvec2(ivec2(0));
                                    vec4 _795 = imageLoad(r_frame_info, ivec2(_793));
                                    float _797 = _795.y;
                                    float _811;
                                    if (_797 < 10000.0)
                                    {
                                        _811 = max(0.0, _797 + ((_105[_712].x - _797) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                    }
                                    else
                                    {
                                        _811 = _105[_712].x;
                                    }
                                    vec4 _817 = _795;
                                    _817.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_811) * 8.0));
                                    vec4 _818 = _817;
                                    _818.y = _811;
                                    vec4 _819 = _818;
                                    _819.z = _105[_712].y;
                                    imageStore(w_frame_info, ivec2(_793), _819);
                                }
                            }
                        }
                        barrier();
                    }
                    if (gl_LocalInvocationIndex < 64u)
                    {
                        spdIntermediateR[_134][_137] = _105[0].x;
                        spdIntermediateG[_134][_137] = _105[0].y;
                        spdIntermediateB[_134][_137] = _105[0].z;
                        spdIntermediateA[_134][_137] = _105[0].w;
                        uint _833 = _134 + 8u;
                        spdIntermediateR[_833][_137] = _105[1].x;
                        spdIntermediateG[_833][_137] = _105[1].y;
                        spdIntermediateB[_833][_137] = _105[1].z;
                        spdIntermediateA[_833][_137] = _105[1].w;
                        uint _843 = _137 + 8u;
                        spdIntermediateR[_134][_843] = _105[2].x;
                        spdIntermediateG[_134][_843] = _105[2].y;
                        spdIntermediateB[_134][_843] = _105[2].z;
                        spdIntermediateA[_134][_843] = _105[2].w;
                        spdIntermediateR[_833][_843] = _105[3].x;
                        spdIntermediateG[_833][_843] = _105[3].y;
                        spdIntermediateB[_833][_843] = _105[3].z;
                        spdIntermediateA[_833][_843] = _105[3].w;
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
                        uint _870 = _142 + 1u;
                        uint _871 = _144 + 1u;
                        float _873 = spdIntermediateR[_142][_144];
                        float _875 = spdIntermediateG[_142][_144];
                        float _877 = spdIntermediateB[_142][_144];
                        float _882 = spdIntermediateR[_870][_144];
                        float _884 = spdIntermediateG[_870][_144];
                        float _886 = spdIntermediateB[_870][_144];
                        float _891 = spdIntermediateR[_142][_871];
                        float _893 = spdIntermediateG[_142][_871];
                        float _895 = spdIntermediateB[_142][_871];
                        float _900 = spdIntermediateR[_870][_871];
                        float _902 = spdIntermediateG[_870][_871];
                        float _904 = spdIntermediateB[_870][_871];
                        vec4 _911 = (((vec4(_873, _875, _877, spdIntermediateA[_142][_144]) + vec4(_882, _884, _886, spdIntermediateA[_870][_144])) + vec4(_891, _893, _895, spdIntermediateA[_142][_871])) + vec4(_900, _902, _904, spdIntermediateA[_870][_871])) * 0.25;
                        if (2u == _259)
                        {
                            if (all(equal((ivec2(_115 * uvec2(8u)) + _152), ivec2(0))))
                            {
                                uvec2 _922 = uvec2(ivec2(0));
                                vec4 _924 = imageLoad(r_frame_info, ivec2(_922));
                                float _926 = _924.y;
                                float _927 = _911.x;
                                float _940;
                                if (_926 < 10000.0)
                                {
                                    _940 = max(0.0, _926 + ((_927 - _926) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _940 = _927;
                                }
                                vec4 _946 = _924;
                                _946.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_940) * 8.0));
                                vec4 _947 = _946;
                                _947.y = _940;
                                vec4 _948 = _947;
                                _948.z = _911.y;
                                imageStore(w_frame_info, ivec2(_922), _948);
                            }
                        }
                        uint _951 = _142 + (_137 % 2u);
                        spdIntermediateR[_951][_144] = _911.x;
                        spdIntermediateG[_951][_144] = _911.y;
                        spdIntermediateB[_951][_144] = _911.z;
                        spdIntermediateA[_951][_144] = _911.w;
                    }
                    if (cbSPD.mips <= 3u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 16u)
                    {
                        uint _966 = _134 * 4u;
                        uint _967 = _137 * 4u;
                        uint _968 = _966 + 2u;
                        uint _969 = _966 + 1u;
                        uint _970 = _967 + 2u;
                        uint _971 = _966 + 3u;
                        float _973 = spdIntermediateR[_966][_967];
                        float _975 = spdIntermediateG[_966][_967];
                        float _977 = spdIntermediateB[_966][_967];
                        float _982 = spdIntermediateR[_968][_967];
                        float _984 = spdIntermediateG[_968][_967];
                        float _986 = spdIntermediateB[_968][_967];
                        float _991 = spdIntermediateR[_969][_970];
                        float _993 = spdIntermediateG[_969][_970];
                        float _995 = spdIntermediateB[_969][_970];
                        float _1000 = spdIntermediateR[_971][_970];
                        float _1002 = spdIntermediateG[_971][_970];
                        float _1004 = spdIntermediateB[_971][_970];
                        vec4 _1011 = (((vec4(_973, _975, _977, spdIntermediateA[_966][_967]) + vec4(_982, _984, _986, spdIntermediateA[_968][_967])) + vec4(_991, _993, _995, spdIntermediateA[_969][_970])) + vec4(_1000, _1002, _1004, spdIntermediateA[_971][_970])) * 0.25;
                        if (3u == _259)
                        {
                            if (all(equal((ivec2(_115 * uvec2(4u)) + _152), ivec2(0))))
                            {
                                uvec2 _1022 = uvec2(ivec2(0));
                                vec4 _1024 = imageLoad(r_frame_info, ivec2(_1022));
                                float _1026 = _1024.y;
                                float _1027 = _1011.x;
                                float _1040;
                                if (_1026 < 10000.0)
                                {
                                    _1040 = max(0.0, _1026 + ((_1027 - _1026) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _1040 = _1027;
                                }
                                vec4 _1046 = _1024;
                                _1046.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1040) * 8.0));
                                vec4 _1047 = _1046;
                                _1047.y = _1040;
                                vec4 _1048 = _1047;
                                _1048.z = _1011.y;
                                imageStore(w_frame_info, ivec2(_1022), _1048);
                            }
                        }
                        uint _1050 = _966 + _137;
                        spdIntermediateR[_1050][_967] = _1011.x;
                        spdIntermediateG[_1050][_967] = _1011.y;
                        spdIntermediateB[_1050][_967] = _1011.z;
                        spdIntermediateA[_1050][_967] = _1011.w;
                    }
                    if (cbSPD.mips <= 4u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 4u)
                    {
                        uint _1065 = _134 * 8u;
                        uint _1066 = _1065 + _144;
                        uint _1067 = _137 * 8u;
                        uint _1069 = (_1065 + 4u) + _144;
                        uint _1071 = (_1065 + 1u) + _144;
                        uint _1072 = _1067 + 4u;
                        uint _1074 = (_1065 + 5u) + _144;
                        float _1076 = spdIntermediateR[_1066][_1067];
                        float _1078 = spdIntermediateG[_1066][_1067];
                        float _1080 = spdIntermediateB[_1066][_1067];
                        float _1085 = spdIntermediateR[_1069][_1067];
                        float _1087 = spdIntermediateG[_1069][_1067];
                        float _1089 = spdIntermediateB[_1069][_1067];
                        float _1094 = spdIntermediateR[_1071][_1072];
                        float _1096 = spdIntermediateG[_1071][_1072];
                        float _1098 = spdIntermediateB[_1071][_1072];
                        float _1103 = spdIntermediateR[_1074][_1072];
                        float _1105 = spdIntermediateG[_1074][_1072];
                        float _1107 = spdIntermediateB[_1074][_1072];
                        vec4 _1114 = (((vec4(_1076, _1078, _1080, spdIntermediateA[_1066][_1067]) + vec4(_1085, _1087, _1089, spdIntermediateA[_1069][_1067])) + vec4(_1094, _1096, _1098, spdIntermediateA[_1071][_1072])) + vec4(_1103, _1105, _1107, spdIntermediateA[_1074][_1072])) * 0.25;
                        if (4u == _259)
                        {
                            if (all(equal((ivec2(_115 * uvec2(2u)) + _152), ivec2(0))))
                            {
                                uvec2 _1125 = uvec2(ivec2(0));
                                vec4 _1127 = imageLoad(r_frame_info, ivec2(_1125));
                                float _1129 = _1127.y;
                                float _1130 = _1114.x;
                                float _1143;
                                if (_1129 < 10000.0)
                                {
                                    _1143 = max(0.0, _1129 + ((_1130 - _1129) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _1143 = _1130;
                                }
                                vec4 _1149 = _1127;
                                _1149.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1143) * 8.0));
                                vec4 _1150 = _1149;
                                _1150.y = _1143;
                                vec4 _1151 = _1150;
                                _1151.z = _1114.y;
                                imageStore(w_frame_info, ivec2(_1125), _1151);
                            }
                        }
                        uint _1153 = _134 + _144;
                        spdIntermediateR[_1153][0u] = _1114.x;
                        spdIntermediateG[_1153][0u] = _1114.y;
                        spdIntermediateB[_1153][0u] = _1114.z;
                        spdIntermediateA[_1153][0u] = _1114.w;
                    }
                    if (cbSPD.mips <= 5u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 1u)
                    {
                        vec4 _1191 = (((vec4(spdIntermediateR[0u][0u], spdIntermediateG[0u][0u], _103, _103) + vec4(spdIntermediateR[1u][0u], spdIntermediateG[1u][0u], _103, _103)) + vec4(spdIntermediateR[2u][0u], spdIntermediateG[2u][0u], _103, _103)) + vec4(spdIntermediateR[3u][0u], spdIntermediateG[3u][0u], _103, _103)) * 0.25;
                        ivec2 _1192 = ivec2(_115);
                        float _1193 = _1191.x;
                        imageStore(w_spd_mip5, ivec2(uvec2(_1192)), vec4(_1193, _1191.y, 0.0, 0.0));
                        if (5u == _259)
                        {
                            if (all(equal(_1192, ivec2(0))))
                            {
                                uvec2 _1205 = uvec2(ivec2(0));
                                vec4 _1207 = imageLoad(r_frame_info, ivec2(_1205));
                                float _1208 = _1207.y;
                                float _1221;
                                if (_1208 < 10000.0)
                                {
                                    _1221 = max(0.0, _1208 + ((_1193 - _1208) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _1221 = _1193;
                                }
                                vec4 _1227 = _1207;
                                _1227.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1221) * 8.0));
                                vec4 _1228 = _1227;
                                _1228.y = _1221;
                                vec4 _1229 = _1228;
                                _1229.z = _1191.y;
                                imageStore(w_frame_info, ivec2(_1205), _1229);
                            }
                        }
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
                uvec2 _1237 = uvec2(ivec2(0));
                uint _1239 = imageAtomicAdd(rw_test_global_atomic, ivec2(_1237), 1u);
                uint _1241 = imageAtomicAdd(rw_spd_global_atomic, ivec2(_1237), 1u);
                spdCounter = _1241;
            }
            barrier();
            if (spdCounter != (cbSPD.numWorkGroups - 1u))
            {
                break;
            }
            uint _1252;
            uint _1254;
            uint _1327;
            uint _1330;
            uint _1402;
            uint _1405;
            uvec2 _1247 = uvec2(ivec2(0));
            imageStore(rw_spd_global_atomic, ivec2(_1247), uvec4(0u));
            imageStore(rw_test_global_atomic, ivec2(_1247), uvec4(0u));
            switch (0u)
            {
                default:
                {
                    _1252 = _134 * 4u;
                    int _1253 = int(_1252);
                    _1254 = _137 * 4u;
                    int _1255 = int(_1254);
                    uvec2 _1257 = uvec2(ivec2(_1253, _1255));
                    vec4 _1264 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1257))));
                    vec4 _1271 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1257 + uvec2(0u, 1u)))));
                    vec4 _1278 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1257 + uvec2(1u, 0u)))));
                    vec4 _1285 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1257 + uvec2(1u)))));
                    vec4 _1292 = (((vec4(_1264.xy, 0.0, 0.0) + vec4(_1271.xy, 0.0, 0.0)) + vec4(_1278.xy, 0.0, 0.0)) + vec4(_1285.xy, 0.0, 0.0)) * 0.25;
                    bool _1293 = 6u == _259;
                    if (_1293)
                    {
                        if (all(equal(_146, ivec2(0))))
                        {
                            vec4 _1301 = imageLoad(r_frame_info, ivec2(_1247));
                            float _1303 = _1301.y;
                            float _1304 = _1292.x;
                            float _1317;
                            if (_1303 < 10000.0)
                            {
                                _1317 = max(0.0, _1303 + ((_1304 - _1303) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _1317 = _1304;
                            }
                            vec4 _1323 = _1301;
                            _1323.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1317) * 8.0));
                            vec4 _1324 = _1323;
                            _1324.y = _1317;
                            vec4 _1325 = _1324;
                            _1325.z = _1292.y;
                            imageStore(w_frame_info, ivec2(_1247), _1325);
                        }
                    }
                    _1327 = _1252 + 2u;
                    int _1328 = int(_1327);
                    _1330 = _142 + 1u;
                    int _1331 = int(_1330);
                    uvec2 _1333 = uvec2(ivec2(_1328, _1255));
                    vec4 _1340 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1333))));
                    vec4 _1347 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1333 + uvec2(0u, 1u)))));
                    vec4 _1354 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1333 + uvec2(1u, 0u)))));
                    vec4 _1361 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1333 + uvec2(1u)))));
                    vec4 _1368 = (((vec4(_1340.xy, 0.0, 0.0) + vec4(_1347.xy, 0.0, 0.0)) + vec4(_1354.xy, 0.0, 0.0)) + vec4(_1361.xy, 0.0, 0.0)) * 0.25;
                    if (_1293)
                    {
                        if (all(equal(ivec2(_1331, _145), ivec2(0))))
                        {
                            vec4 _1376 = imageLoad(r_frame_info, ivec2(_1247));
                            float _1378 = _1376.y;
                            float _1379 = _1368.x;
                            float _1392;
                            if (_1378 < 10000.0)
                            {
                                _1392 = max(0.0, _1378 + ((_1379 - _1378) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _1392 = _1379;
                            }
                            vec4 _1398 = _1376;
                            _1398.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1392) * 8.0));
                            vec4 _1399 = _1398;
                            _1399.y = _1392;
                            vec4 _1400 = _1399;
                            _1400.z = _1368.y;
                            imageStore(w_frame_info, ivec2(_1247), _1400);
                        }
                    }
                    _1402 = _1254 + 2u;
                    int _1403 = int(_1402);
                    _1405 = _144 + 1u;
                    int _1406 = int(_1405);
                    uvec2 _1408 = uvec2(ivec2(_1253, _1403));
                    vec4 _1415 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1408))));
                    vec4 _1422 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1408 + uvec2(0u, 1u)))));
                    vec4 _1429 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1408 + uvec2(1u, 0u)))));
                    vec4 _1436 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1408 + uvec2(1u)))));
                    vec4 _1443 = (((vec4(_1415.xy, 0.0, 0.0) + vec4(_1422.xy, 0.0, 0.0)) + vec4(_1429.xy, 0.0, 0.0)) + vec4(_1436.xy, 0.0, 0.0)) * 0.25;
                    if (_1293)
                    {
                        if (all(equal(ivec2(_143, _1406), ivec2(0))))
                        {
                            vec4 _1451 = imageLoad(r_frame_info, ivec2(_1247));
                            float _1453 = _1451.y;
                            float _1454 = _1443.x;
                            float _1467;
                            if (_1453 < 10000.0)
                            {
                                _1467 = max(0.0, _1453 + ((_1454 - _1453) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _1467 = _1454;
                            }
                            vec4 _1473 = _1451;
                            _1473.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1467) * 8.0));
                            vec4 _1474 = _1473;
                            _1474.y = _1467;
                            vec4 _1475 = _1474;
                            _1475.z = _1443.y;
                            imageStore(w_frame_info, ivec2(_1247), _1475);
                        }
                    }
                    uvec2 _1479 = uvec2(ivec2(_1328, _1403));
                    vec4 _1486 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1479))));
                    vec4 _1493 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1479 + uvec2(0u, 1u)))));
                    vec4 _1500 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1479 + uvec2(1u, 0u)))));
                    vec4 _1507 = imageLoad(r_spd_mip5, ivec2(uvec2(ivec2(_1479 + uvec2(1u)))));
                    vec4 _1514 = (((vec4(_1486.xy, 0.0, 0.0) + vec4(_1493.xy, 0.0, 0.0)) + vec4(_1500.xy, 0.0, 0.0)) + vec4(_1507.xy, 0.0, 0.0)) * 0.25;
                    if (_1293)
                    {
                        if (all(equal(ivec2(_1331, _1406), ivec2(0))))
                        {
                            vec4 _1522 = imageLoad(r_frame_info, ivec2(_1247));
                            float _1524 = _1522.y;
                            float _1525 = _1514.x;
                            float _1538;
                            if (_1524 < 10000.0)
                            {
                                _1538 = max(0.0, _1524 + ((_1525 - _1524) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _1538 = _1525;
                            }
                            vec4 _1544 = _1522;
                            _1544.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1538) * 8.0));
                            vec4 _1545 = _1544;
                            _1545.y = _1538;
                            vec4 _1546 = _1545;
                            _1546.z = _1514.y;
                            imageStore(w_frame_info, ivec2(_1247), _1546);
                        }
                    }
                    if (cbSPD.mips <= 7u)
                    {
                        break;
                    }
                    vec4 _1554 = (((_1292 + _1368) + _1443) + _1514) * 0.25;
                    if (7u == _259)
                    {
                        if (all(equal(_152, ivec2(0))))
                        {
                            vec4 _1563 = imageLoad(r_frame_info, ivec2(_1247));
                            float _1565 = _1563.y;
                            float _1566 = _1554.x;
                            float _1579;
                            if (_1565 < 10000.0)
                            {
                                _1579 = max(0.0, _1565 + ((_1566 - _1565) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                            }
                            else
                            {
                                _1579 = _1566;
                            }
                            vec4 _1585 = _1563;
                            _1585.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1579) * 8.0));
                            vec4 _1586 = _1585;
                            _1586.y = _1579;
                            vec4 _1587 = _1586;
                            _1587.z = _1554.y;
                            imageStore(w_frame_info, ivec2(_1247), _1587);
                        }
                    }
                    spdIntermediateR[_134][_137] = _1554.x;
                    spdIntermediateG[_134][_137] = _1554.y;
                    spdIntermediateB[_134][_137] = _1554.z;
                    spdIntermediateA[_134][_137] = _1554.w;
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
                        float _1606 = spdIntermediateR[_142][_144];
                        float _1608 = spdIntermediateG[_142][_144];
                        float _1610 = spdIntermediateB[_142][_144];
                        float _1615 = spdIntermediateR[_1330][_144];
                        float _1617 = spdIntermediateG[_1330][_144];
                        float _1619 = spdIntermediateB[_1330][_144];
                        float _1624 = spdIntermediateR[_142][_1405];
                        float _1626 = spdIntermediateG[_142][_1405];
                        float _1628 = spdIntermediateB[_142][_1405];
                        float _1633 = spdIntermediateR[_1330][_1405];
                        float _1635 = spdIntermediateG[_1330][_1405];
                        float _1637 = spdIntermediateB[_1330][_1405];
                        vec4 _1644 = (((vec4(_1606, _1608, _1610, spdIntermediateA[_142][_144]) + vec4(_1615, _1617, _1619, spdIntermediateA[_1330][_144])) + vec4(_1624, _1626, _1628, spdIntermediateA[_142][_1405])) + vec4(_1633, _1635, _1637, spdIntermediateA[_1330][_1405])) * 0.25;
                        if (8u == _259)
                        {
                            if (all(equal((ivec2(uvec2(0u) * uvec2(8u)) + _152), ivec2(0))))
                            {
                                vec4 _1656 = imageLoad(r_frame_info, ivec2(_1247));
                                float _1658 = _1656.y;
                                float _1659 = _1644.x;
                                float _1672;
                                if (_1658 < 10000.0)
                                {
                                    _1672 = max(0.0, _1658 + ((_1659 - _1658) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _1672 = _1659;
                                }
                                vec4 _1678 = _1656;
                                _1678.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1672) * 8.0));
                                vec4 _1679 = _1678;
                                _1679.y = _1672;
                                vec4 _1680 = _1679;
                                _1680.z = _1644.y;
                                imageStore(w_frame_info, ivec2(_1247), _1680);
                            }
                        }
                        uint _1683 = _142 + (_137 % 2u);
                        spdIntermediateR[_1683][_144] = _1644.x;
                        spdIntermediateG[_1683][_144] = _1644.y;
                        spdIntermediateB[_1683][_144] = _1644.z;
                        spdIntermediateA[_1683][_144] = _1644.w;
                    }
                    if (cbSPD.mips <= 9u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 16u)
                    {
                        uint _1698 = _1252 + 1u;
                        uint _1699 = _1252 + 3u;
                        float _1701 = spdIntermediateR[_1252][_1254];
                        float _1703 = spdIntermediateG[_1252][_1254];
                        float _1705 = spdIntermediateB[_1252][_1254];
                        float _1710 = spdIntermediateR[_1327][_1254];
                        float _1712 = spdIntermediateG[_1327][_1254];
                        float _1714 = spdIntermediateB[_1327][_1254];
                        float _1719 = spdIntermediateR[_1698][_1402];
                        float _1721 = spdIntermediateG[_1698][_1402];
                        float _1723 = spdIntermediateB[_1698][_1402];
                        float _1728 = spdIntermediateR[_1699][_1402];
                        float _1730 = spdIntermediateG[_1699][_1402];
                        float _1732 = spdIntermediateB[_1699][_1402];
                        vec4 _1739 = (((vec4(_1701, _1703, _1705, spdIntermediateA[_1252][_1254]) + vec4(_1710, _1712, _1714, spdIntermediateA[_1327][_1254])) + vec4(_1719, _1721, _1723, spdIntermediateA[_1698][_1402])) + vec4(_1728, _1730, _1732, spdIntermediateA[_1699][_1402])) * 0.25;
                        if (9u == _259)
                        {
                            if (all(equal((ivec2(uvec2(0u) * uvec2(4u)) + _152), ivec2(0))))
                            {
                                vec4 _1751 = imageLoad(r_frame_info, ivec2(_1247));
                                float _1753 = _1751.y;
                                float _1754 = _1739.x;
                                float _1767;
                                if (_1753 < 10000.0)
                                {
                                    _1767 = max(0.0, _1753 + ((_1754 - _1753) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _1767 = _1754;
                                }
                                vec4 _1773 = _1751;
                                _1773.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1767) * 8.0));
                                vec4 _1774 = _1773;
                                _1774.y = _1767;
                                vec4 _1775 = _1774;
                                _1775.z = _1739.y;
                                imageStore(w_frame_info, ivec2(_1247), _1775);
                            }
                        }
                        uint _1777 = _1252 + _137;
                        spdIntermediateR[_1777][_1254] = _1739.x;
                        spdIntermediateG[_1777][_1254] = _1739.y;
                        spdIntermediateB[_1777][_1254] = _1739.z;
                        spdIntermediateA[_1777][_1254] = _1739.w;
                    }
                    if (cbSPD.mips <= 10u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 4u)
                    {
                        uint _1792 = _134 * 8u;
                        uint _1793 = _1792 + _144;
                        uint _1794 = _137 * 8u;
                        uint _1796 = (_1792 + 4u) + _144;
                        uint _1798 = (_1792 + 1u) + _144;
                        uint _1799 = _1794 + 4u;
                        uint _1801 = (_1792 + 5u) + _144;
                        float _1803 = spdIntermediateR[_1793][_1794];
                        float _1805 = spdIntermediateG[_1793][_1794];
                        float _1807 = spdIntermediateB[_1793][_1794];
                        float _1812 = spdIntermediateR[_1796][_1794];
                        float _1814 = spdIntermediateG[_1796][_1794];
                        float _1816 = spdIntermediateB[_1796][_1794];
                        float _1821 = spdIntermediateR[_1798][_1799];
                        float _1823 = spdIntermediateG[_1798][_1799];
                        float _1825 = spdIntermediateB[_1798][_1799];
                        float _1830 = spdIntermediateR[_1801][_1799];
                        float _1832 = spdIntermediateG[_1801][_1799];
                        float _1834 = spdIntermediateB[_1801][_1799];
                        vec4 _1841 = (((vec4(_1803, _1805, _1807, spdIntermediateA[_1793][_1794]) + vec4(_1812, _1814, _1816, spdIntermediateA[_1796][_1794])) + vec4(_1821, _1823, _1825, spdIntermediateA[_1798][_1799])) + vec4(_1830, _1832, _1834, spdIntermediateA[_1801][_1799])) * 0.25;
                        if (10u == _259)
                        {
                            if (all(equal((ivec2(uvec2(0u) * uvec2(2u)) + _152), ivec2(0))))
                            {
                                vec4 _1853 = imageLoad(r_frame_info, ivec2(_1247));
                                float _1855 = _1853.y;
                                float _1856 = _1841.x;
                                float _1869;
                                if (_1855 < 10000.0)
                                {
                                    _1869 = max(0.0, _1855 + ((_1856 - _1855) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _1869 = _1856;
                                }
                                vec4 _1875 = _1853;
                                _1875.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1869) * 8.0));
                                vec4 _1876 = _1875;
                                _1876.y = _1869;
                                vec4 _1877 = _1876;
                                _1877.z = _1841.y;
                                imageStore(w_frame_info, ivec2(_1247), _1877);
                            }
                        }
                        uint _1879 = _134 + _144;
                        spdIntermediateR[_1879][0u] = _1841.x;
                        spdIntermediateG[_1879][0u] = _1841.y;
                        spdIntermediateB[_1879][0u] = _1841.z;
                        spdIntermediateA[_1879][0u] = _1841.w;
                    }
                    if (cbSPD.mips <= 11u)
                    {
                        break;
                    }
                    barrier();
                    if (gl_LocalInvocationIndex < 1u)
                    {
                        vec4 _1917 = (((vec4(spdIntermediateR[0u][0u], spdIntermediateG[0u][0u], _103, _103) + vec4(spdIntermediateR[1u][0u], spdIntermediateG[1u][0u], _103, _103)) + vec4(spdIntermediateR[2u][0u], spdIntermediateG[2u][0u], _103, _103)) + vec4(spdIntermediateR[3u][0u], spdIntermediateG[3u][0u], _103, _103)) * 0.25;
                        if (11u == _259)
                        {
                            if (all(equal(ivec2(uvec2(0u)), ivec2(0))))
                            {
                                vec4 _1927 = imageLoad(r_frame_info, ivec2(_1247));
                                float _1929 = _1927.y;
                                float _1930 = _1917.x;
                                float _1943;
                                if (_1929 < 10000.0)
                                {
                                    _1943 = max(0.0, _1929 + ((_1930 - _1929) * (1.0 - exp(-cbFSR3Upscaler.fDeltaTime))));
                                }
                                else
                                {
                                    _1943 = _1930;
                                }
                                vec4 _1949 = _1927;
                                _1949.x = 0.833333313465118408203125 / pow(2.0, log2(exp(_1943) * 8.0));
                                vec4 _1950 = _1949;
                                _1950.y = _1943;
                                vec4 _1951 = _1950;
                                _1951.z = _1917.y;
                                imageStore(w_frame_info, ivec2(_1247), _1951);
                            }
                        }
                    }
                    break;
                }
            }
            break;
        }
    }
}

