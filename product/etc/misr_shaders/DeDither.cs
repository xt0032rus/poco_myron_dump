layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(binding = 0, std140) uniform type_UniformBufferConstants_View
{
    vec4 View_ViewRectMin;
    vec4 View_ViewSizeAndInvSize;
    uint View_FullDeDither;
} UniformBufferConstants_View;

layout(binding = 2, rgba8) uniform writeonly highp image2D BlendSceneColor;
uniform highp sampler2D SPIRV_Cross_CombinedSceneColorSPIRV_Cross_DummySampler;

void main()
{
    ivec2 _63 = ivec2(vec2(gl_GlobalInvocationID.xy) + UniformBufferConstants_View.View_ViewRectMin.xy);
    if ((uint(UniformBufferConstants_View.View_ViewSizeAndInvSize.x) > gl_GlobalInvocationID.x) && (uint(UniformBufferConstants_View.View_ViewSizeAndInvSize.y) > gl_GlobalInvocationID.y))
    {
        vec4 _73 = texelFetch(SPIRV_Cross_CombinedSceneColorSPIRV_Cross_DummySampler, ivec2(uvec2(_63)), int(0u));
        vec2 _85 = UniformBufferConstants_View.View_ViewRectMin.xy + UniformBufferConstants_View.View_ViewSizeAndInvSize.xy;
        vec4 _120;
        if (UniformBufferConstants_View.View_FullDeDither != 0u)
        {
            vec4 _100 = (texelFetch(SPIRV_Cross_CombinedSceneColorSPIRV_Cross_DummySampler, ivec2(uvec2(max(vec2(_63 + ivec2(0, -1)), UniformBufferConstants_View.View_ViewRectMin.xy))), int(0u)) + texelFetch(SPIRV_Cross_CombinedSceneColorSPIRV_Cross_DummySampler, ivec2(uvec2(min(vec2(_63 + ivec2(0, 1)), _85))), int(0u))) * vec4(0.5);
            vec4 _102 = (texelFetch(SPIRV_Cross_CombinedSceneColorSPIRV_Cross_DummySampler, ivec2(uvec2(max(vec2(_63 + ivec2(-1, 0)), UniformBufferConstants_View.View_ViewRectMin.xy))), int(0u)) + texelFetch(SPIRV_Cross_CombinedSceneColorSPIRV_Cross_DummySampler, ivec2(uvec2(min(vec2(_63 + ivec2(1, 0)), _85))), int(0u))) * vec4(0.5);
            vec4 _119;
            if (all(lessThan(abs(_100 - _102), vec4(0.100000001490116119384765625))))
            {
                vec4 _110 = (_100 + _102) * vec4(0.5);
                vec4 _118;
                if (any(greaterThan(abs(_73 - _110), vec4(0.20000000298023223876953125))))
                {
                    _118 = mix(_73, _110, vec4(0.75));
                }
                else
                {
                    _118 = _73;
                }
                _119 = _118;
            }
            else
            {
                _119 = _73;
            }
            _120 = _119;
        }
        else
        {
            _120 = _73;
        }
        imageStore(BlendSceneColor, ivec2(gl_GlobalInvocationID.xy), _120);
    }
}

