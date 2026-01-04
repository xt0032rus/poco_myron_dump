#extension GL_OES_shader_image_atomic : require
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
    float onlycopy;
} cbFSR3Upscaler;

layout(binding = 1, std140) uniform type_cbSceneInfo
{
    mat4 transform;
} cbSceneInfo;

layout(binding = 0, rgba8) uniform writeonly highp image2D rw_internal_upscaled_color;
layout(binding = 4, r32ui) uniform highp uimage2D rw_game_motion_vector_field_x;
uniform highp sampler2D SPIRV_Cross_Combinedr_input_depthSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler;
uniform highp sampler2D SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler;

ivec2 _211;

void main()
{
    if (cbFSR3Upscaler.onlycopy > 0.100000001490116119384765625)
    {
        ivec2 _914 = ivec2(floor((vec2(ivec3(gl_GlobalInvocationID).xy) + vec2(0.5)) * cbFSR3Upscaler.fDownscaleFactor));
        ivec2 _922 = _914;
        _922.x = max(1, min(_914.x, (cbFSR3Upscaler.iRenderSize.x - 2)));
        ivec2 _928 = _922;
        _928.y = max(1, min(_914.y, (cbFSR3Upscaler.iRenderSize.y - 2)));
        imageStore(rw_internal_upscaled_color, ivec2(uvec2(ivec3(gl_GlobalInvocationID).xy)), vec4(texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(uvec2(_928)), int(0u)).xyz, 1.0));
    }
    else
    {
        uvec2 _237;
        vec2 _222 = vec2(ivec3(gl_GlobalInvocationID).xy) + vec2(0.5);
        vec2 _225 = vec2(cbFSR3Upscaler.iUpscaleSize);
        vec2 _226 = _222 / _225;
        vec2 _229 = vec2(cbFSR3Upscaler.iRenderSize);
        uvec2 _231 = uvec2(_226 * _229);
        vec4 _233 = texelFetch(SPIRV_Cross_Combinedr_input_depthSPIRV_Cross_DummySampler, ivec2(_231), int(0u));
        float _234 = _233.x;
        vec2 _276;
        switch (0u)
        {
            default:
            {
                _237 = uvec2(cbFSR3Upscaler.iRenderSize);
                if (all(lessThan(_231, _237)))
                {
                    vec4 _243 = texelFetch(SPIRV_Cross_Combinedr_dilated_motion_vectorsSPIRV_Cross_DummySampler, ivec2(_231), int(0u));
                    vec3 _274;
                    if (_243.z < 0.001000000047497451305389404296875)
                    {
                        vec2 _257 = (vec2(_231) + vec2(0.5)) / _229;
                        vec4 _265 = cbSceneInfo.transform * vec4((_257 * vec2(2.0)) - vec2(1.0), _234, 1.0);
                        vec2 _272 = (((_265.xy / vec2(_265.w)) + vec2(1.0)) * vec2(0.5)) - _257;
                        _274 = vec3(_272.x, _272.y, _243.z);
                    }
                    else
                    {
                        vec2 _253 = ((_243.xy - vec2(0.4999924004077911376953125)) * vec2(4.008016109466552734375)).xy * vec2(-0.5);
                        _274 = vec3(_253.x, _253.y, _243.z);
                    }
                    _276 = _274.xy;
                    break;
                }
                _276 = vec2(0.0);
                break;
            }
        }
        float _278 = _234 * (-999.0);
        vec2 _292 = _276 * 0.5;
        ivec2 _296 = ivec2(floor(((vec2(ivec2(_231)) * (vec2(1.0) / _229)) + _292) * _229));
        if (all(lessThan(_296, cbFSR3Upscaler.iRenderSize)) && all(greaterThan(_296, ivec2(0))))
        {
            vec2 _305 = (_292 + vec2(1.0)) * vec2(0.5);
            uint _322 = imageAtomicMax(rw_game_motion_vector_field_x, ivec2(uvec2(uint(_296.x), uint(_296.y))), ((((2147483648u | ((max(1u, uint(((_278 + 1000.0) / (_278 + 2000.0)) * 2046.0)) & 1023u) << 21u)) | 0u) & 4292870144u) | (uint(_305.x * 2047.0) << 10u)) | uint(_305.y * 1023.0));
        }
        vec2 _324 = _226 + (_276 * vec2(0.5));
        float _325 = _324.x;
        float _329 = _324.y;
        bool _333 = ((_325 >= 0.0) && (_325 <= 1.0)) && ((_329 >= 0.0) && (_329 <= 1.0));
        vec3 _419;
        if (_333 && (!((_333 == false) || (0.0 == cbFSR3Upscaler.fFrameIndex))))
        {
            vec2 _344 = (_324 * _225) - vec2(0.5);
            vec2 _346 = _344 - floor(_344);
            vec2 _352 = _344;
            _352.x = max(0.0, min(float(cbFSR3Upscaler.iUpscaleSize.x), _344.x));
            vec2 _358 = _352;
            _358.y = max(0.0, min(float(cbFSR3Upscaler.iUpscaleSize.y), _344.y));
            ivec2 _360 = ivec2(floor(_358));
            ivec2 _365 = _360;
            _365.x = max(1, min(_360.x, (cbFSR3Upscaler.iUpscaleSize.x - 2)));
            ivec2 _370 = _365;
            _370.y = max(1, min(_360.y, (cbFSR3Upscaler.iUpscaleSize.y - 2)));
            vec4 _385 = texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370)), int(0u));
            vec4 _388 = texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(1, 0))), int(0u));
            vec4 _394 = texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(0, 1))), int(0u));
            vec4 _397 = texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(1))), int(0u));
            mediump float _18 = _346.x;
            mediump float _21 = (-1.0) - _18;
            mediump float _33 = _21 * _21;
            mediump float _35 = (0.39990234375 * _33) - 1.0;
            mediump float _37 = (0.25 * _33) - 1.0;
            mediump float _42 = (((1.5625 * _35) * _35) - 0.5625) * (_37 * _37);
            mediump float _22 = -_18;
            mediump float _43 = _22 * _22;
            mediump float _45 = (0.39990234375 * _43) - 1.0;
            mediump float _47 = (0.25 * _43) - 1.0;
            mediump float _52 = (((1.5625 * _45) * _45) - 0.5625) * (_47 * _47);
            mediump float _23 = 1.0 - _18;
            mediump float _53 = _23 * _23;
            mediump float _55 = (0.39990234375 * _53) - 1.0;
            mediump float _57 = (0.25 * _53) - 1.0;
            mediump float _62 = (((1.5625 * _55) * _55) - 0.5625) * (_57 * _57);
            mediump vec4 _31 = vec4((_42 + _52) + _62);
            mediump float _19 = _346.y;
            mediump float _75 = (-1.0) - _19;
            mediump float _87 = _75 * _75;
            mediump float _89 = (0.39990234375 * _87) - 1.0;
            mediump float _91 = (0.25 * _87) - 1.0;
            mediump float _96 = (((1.5625 * _89) * _89) - 0.5625) * (_91 * _91);
            mediump float _76 = -_19;
            mediump float _97 = _76 * _76;
            mediump float _99 = (0.39990234375 * _97) - 1.0;
            mediump float _101 = (0.25 * _97) - 1.0;
            mediump float _106 = (((1.5625 * _99) * _99) - 0.5625) * (_101 * _101);
            mediump float _77 = 1.0 - _19;
            mediump float _107 = _77 * _77;
            mediump float _109 = (0.39990234375 * _107) - 1.0;
            mediump float _111 = (0.25 * _107) - 1.0;
            mediump float _116 = (((1.5625 * _109) * _109) - 0.5625) * (_111 * _111);
            vec3 _403 = (clamp((((((((texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(-1))), int(0u)) * _42) + (texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(0, -1))), int(0u)) * _52)) + (texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(1, -1))), int(0u)) * _62)) / _31) * _96) + (((((texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(-1, 0))), int(0u)) * _42) + (_385 * _52)) + (_388 * _62)) / _31) * _106)) + (((((texelFetch(SPIRV_Cross_Combinedr_internal_upscaled_colorSPIRV_Cross_DummySampler, ivec2(uvec2(_370 + ivec2(-1, 1))), int(0u)) * _42) + (_394 * _52)) + (_397 * _62)) / _31) * _116)) / vec4((_96 + _106) + _116), min(min(min(_385, _388), _394), _397), max(max(max(_385, _388), _394), _397)).xyz * cbFSR3Upscaler.fDeltaPreExposure) * 1.0;
            float _404 = _403.x;
            float _407 = 0.5 * _403.y;
            float _409 = _403.z;
            float _410 = 0.25 * _409;
            _419 = vec3(((0.25 * _404) + _407) + _410, (0.5 * _404) - (0.5 * _409), (((-0.25) * _404) + _407) - _410);
        }
        else
        {
            _419 = vec3(0.0);
        }
        vec2 _422 = _222 * cbFSR3Upscaler.fDownscaleFactor;
        ivec2 _424 = ivec2(floor(_422));
        ivec2 _430 = _424;
        _430.x = max(1, min(_424.x, (cbFSR3Upscaler.iRenderSize.x - 2)));
        ivec2 _436 = _430;
        _436.y = max(1, min(_424.y, (cbFSR3Upscaler.iRenderSize.y - 2)));
        vec2 _440 = vec2(_436) + cbFSR3Upscaler.fJitter;
        vec2 _441 = _440 - _422;
        bool _444 = _440.x > _422.x;
        ivec2 _446 = _211;
        _446.x = _444 ? (-2) : (-1);
        bool _449 = _440.y > _422.y;
        ivec2 _451 = _446;
        _451.y = _449 ? (-2) : (-1);
        vec2 _452 = vec2(_451);
        int _453 = _444 ? 3 : 0;
        int _454 = _449 ? 3 : 0;
        ivec2 _455 = ivec2(_453, _454);
        ivec2 _456 = _436 + _451;
        uvec2 _458 = uvec2(_456 + _455);
        vec3 _462 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_458), int(0u)).xyz * 1.0;
        float _463 = _462.x;
        float _466 = 0.5 * _462.y;
        float _468 = _462.z;
        float _469 = 0.25 * _468;
        vec3 _477 = vec3(((0.25 * _463) + _466) + _469, (0.5 * _463) - (0.5 * _468), (((-0.25) * _463) + _466) - _469);
        int _478 = _444 ? 2 : 1;
        ivec2 _479 = ivec2(_478, _454);
        uvec2 _481 = uvec2(_456 + _479);
        vec3 _484 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_481), int(0u)).xyz * 1.0;
        float _485 = _484.x;
        float _488 = 0.5 * _484.y;
        float _490 = _484.z;
        float _491 = 0.25 * _490;
        vec3 _499 = vec3(((0.25 * _485) + _488) + _491, (0.5 * _485) - (0.5 * _490), (((-0.25) * _485) + _488) - _491);
        int _500 = _444 ? 1 : 2;
        ivec2 _501 = ivec2(_500, _454);
        uvec2 _503 = uvec2(_456 + _501);
        vec3 _506 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_503), int(0u)).xyz * 1.0;
        float _507 = _506.x;
        float _510 = 0.5 * _506.y;
        float _512 = _506.z;
        float _513 = 0.25 * _512;
        vec3 _521 = vec3(((0.25 * _507) + _510) + _513, (0.5 * _507) - (0.5 * _512), (((-0.25) * _507) + _510) - _513);
        int _522 = _449 ? 2 : 1;
        ivec2 _523 = ivec2(_453, _522);
        uvec2 _525 = uvec2(_456 + _523);
        vec3 _528 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_525), int(0u)).xyz * 1.0;
        float _529 = _528.x;
        float _532 = 0.5 * _528.y;
        float _534 = _528.z;
        float _535 = 0.25 * _534;
        vec3 _543 = vec3(((0.25 * _529) + _532) + _535, (0.5 * _529) - (0.5 * _534), (((-0.25) * _529) + _532) - _535);
        ivec2 _544 = ivec2(_478, _522);
        uvec2 _546 = uvec2(_456 + _544);
        vec3 _549 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_546), int(0u)).xyz * 1.0;
        float _550 = _549.x;
        float _553 = 0.5 * _549.y;
        float _555 = _549.z;
        float _556 = 0.25 * _555;
        vec3 _564 = vec3(((0.25 * _550) + _553) + _556, (0.5 * _550) - (0.5 * _555), (((-0.25) * _550) + _553) - _556);
        ivec2 _565 = ivec2(_500, _522);
        uvec2 _567 = uvec2(_456 + _565);
        vec3 _570 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_567), int(0u)).xyz * 1.0;
        float _571 = _570.x;
        float _574 = 0.5 * _570.y;
        float _576 = _570.z;
        float _577 = 0.25 * _576;
        vec3 _585 = vec3(((0.25 * _571) + _574) + _577, (0.5 * _571) - (0.5 * _576), (((-0.25) * _571) + _574) - _577);
        int _586 = _449 ? 1 : 2;
        ivec2 _587 = ivec2(_453, _586);
        uvec2 _589 = uvec2(_456 + _587);
        vec3 _592 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_589), int(0u)).xyz * 1.0;
        float _593 = _592.x;
        float _596 = 0.5 * _592.y;
        float _598 = _592.z;
        float _599 = 0.25 * _598;
        vec3 _607 = vec3(((0.25 * _593) + _596) + _599, (0.5 * _593) - (0.5 * _598), (((-0.25) * _593) + _596) - _599);
        ivec2 _608 = ivec2(_478, _586);
        uvec2 _610 = uvec2(_456 + _608);
        vec3 _613 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_610), int(0u)).xyz * 1.0;
        float _614 = _613.x;
        float _617 = 0.5 * _613.y;
        float _619 = _613.z;
        float _620 = 0.25 * _619;
        vec3 _628 = vec3(((0.25 * _614) + _617) + _620, (0.5 * _614) - (0.5 * _619), (((-0.25) * _614) + _617) - _620);
        ivec2 _629 = ivec2(_500, _586);
        uvec2 _631 = uvec2(_456 + _629);
        vec3 _634 = texelFetch(SPIRV_Cross_Combinedr_input_color_jitteredSPIRV_Cross_DummySampler, ivec2(_631), int(0u)).xyz * 1.0;
        float _635 = _634.x;
        float _638 = 0.5 * _634.y;
        float _640 = _634.z;
        float _641 = 0.25 * _640;
        vec3 _649 = vec3(((0.25 * _635) + _638) + _641, (0.5 * _635) - (0.5 * _640), (((-0.25) * _635) + _638) - _641);
        vec2 _652 = _441 + (_452 + vec2(_455));
        float _657 = min(dot(_652, _652), 4.0);
        float _659 = (0.4000000059604644775390625 * _657) - 1.0;
        float _661 = (0.25 * _657) - 1.0;
        float _667 = float(all(lessThan(_458, _237))) * ((((1.5625 * _659) * _659) - 0.5625) * (_661 * _661));
        vec2 _671 = _441 + (_452 + vec2(_479));
        float _676 = min(dot(_671, _671), 4.0);
        float _678 = (0.4000000059604644775390625 * _676) - 1.0;
        float _680 = (0.25 * _676) - 1.0;
        float _686 = float(all(lessThan(_481, _237))) * ((((1.5625 * _678) * _678) - 0.5625) * (_680 * _680));
        vec2 _694 = _441 + (_452 + vec2(_501));
        float _699 = min(dot(_694, _694), 4.0);
        float _701 = (0.4000000059604644775390625 * _699) - 1.0;
        float _703 = (0.25 * _699) - 1.0;
        float _709 = float(all(lessThan(_503, _237))) * ((((1.5625 * _701) * _701) - 0.5625) * (_703 * _703));
        vec2 _717 = _441 + (_452 + vec2(_523));
        float _722 = min(dot(_717, _717), 4.0);
        float _724 = (0.4000000059604644775390625 * _722) - 1.0;
        float _726 = (0.25 * _722) - 1.0;
        float _732 = float(all(lessThan(_525, _237))) * ((((1.5625 * _724) * _724) - 0.5625) * (_726 * _726));
        vec2 _740 = _441 + (_452 + vec2(_544));
        float _745 = min(dot(_740, _740), 4.0);
        float _747 = (0.4000000059604644775390625 * _745) - 1.0;
        float _749 = (0.25 * _745) - 1.0;
        float _755 = float(all(lessThan(_546, _237))) * ((((1.5625 * _747) * _747) - 0.5625) * (_749 * _749));
        vec2 _763 = _441 + (_452 + vec2(_565));
        float _768 = min(dot(_763, _763), 4.0);
        float _770 = (0.4000000059604644775390625 * _768) - 1.0;
        float _772 = (0.25 * _768) - 1.0;
        float _778 = float(all(lessThan(_567, _237))) * ((((1.5625 * _770) * _770) - 0.5625) * (_772 * _772));
        vec2 _786 = _441 + (_452 + vec2(_587));
        float _791 = min(dot(_786, _786), 4.0);
        float _793 = (0.4000000059604644775390625 * _791) - 1.0;
        float _795 = (0.25 * _791) - 1.0;
        float _801 = float(all(lessThan(_589, _237))) * ((((1.5625 * _793) * _793) - 0.5625) * (_795 * _795));
        vec2 _809 = _441 + (_452 + vec2(_608));
        float _814 = min(dot(_809, _809), 4.0);
        float _816 = (0.4000000059604644775390625 * _814) - 1.0;
        float _818 = (0.25 * _814) - 1.0;
        float _824 = float(all(lessThan(_610, _237))) * ((((1.5625 * _816) * _816) - 0.5625) * (_818 * _818));
        vec2 _832 = _441 + (_452 + vec2(_629));
        float _837 = min(dot(_832, _832), 4.0);
        float _839 = (0.4000000059604644775390625 * _837) - 1.0;
        float _841 = (0.25 * _837) - 1.0;
        float _847 = float(all(lessThan(_631, _237))) * ((((1.5625 * _839) * _839) - 0.5625) * (_841 * _841));
        vec3 _849 = ((((((((_477 * _667) + (_499 * _686)) + (_521 * _709)) + (_543 * _732)) + (_564 * _755)) + (_585 * _778)) + (_607 * _801)) + (_628 * _824)) + (_649 * _847);
        float _850 = (((((((_667 + _686) + _709) + _732) + _755) + _778) + _801) + _824) + _847;
        vec3 _851 = min(min(min(min(min(min(min(min(_477, _499), _521), _543), _564), _585), _607), _628), _649);
        vec3 _852 = max(max(max(max(max(max(max(max(_477, _499), _521), _543), _564), _585), _607), _628), _649);
        float _855 = _850 * float(_850 > 6.099999882280826568603515625e-05);
        vec3 _862;
        if (_855 > 6.099999882280826568603515625e-05)
        {
            _862 = clamp(_849 / vec3(_855), _851, _852);
        }
        else
        {
            _862 = _849;
        }
        vec3 _888;
        if (any(greaterThan(_851, _419)) || any(greaterThan(_419, _852)))
        {
            vec3 _870 = clamp(_419, _851, _852);
            vec3 _872 = abs(_419 - _870);
            float _873 = _872.x;
            float _874 = _872.y;
            float _876 = _872.z;
            vec3 _887;
            if (any(greaterThan(vec3((_873 + _874) - _876, _873 + _876, (_873 - _874) - _876), vec3(0.00999999977648258209228515625))))
            {
                _887 = mix(_870, _419, vec3(0.00999999977648258209228515625));
            }
            else
            {
                _887 = _419;
            }
            _888 = _887;
        }
        else
        {
            _888 = _419;
        }
        vec3 _891 = mix(_888, _862, vec3(_333 ? 0.0999999940395355224609375 : 1.0));
        float _892 = _891.x;
        float _893 = _891.y;
        float _895 = _891.z;
        imageStore(rw_internal_upscaled_color, ivec2(uvec2(ivec3(gl_GlobalInvocationID).xy)), vec4(max(vec3((_892 + _893) - _895, _892 + _895, (_892 - _893) - _895), vec3(0.0)), 1.0));
    }
}

