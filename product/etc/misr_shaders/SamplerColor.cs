layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(binding = 0, std140) uniform type_cbBufferInfo
{
    ivec2 iRenderSize;
    ivec2 iUpscaleSize;
    ivec2 iBackBufferSize;
} cbBufferInfo;

layout(binding = 4, rgba8) uniform writeonly highp image2D BlendSceneColor;
layout(binding = 5, rgba8) uniform writeonly highp image2D BlendSceneColorWithUI;
uniform highp sampler2D SPIRV_Cross_CombinedSceneColors_LinearClamp;
uniform highp sampler2D SPIRV_Cross_CombinedUIColors_LinearClamp;

void main()
{
    uint _43 = uint(cbBufferInfo.iBackBufferSize.x);
    uint _46 = uint(cbBufferInfo.iBackBufferSize.y);
    if ((_43 > gl_GlobalInvocationID.x) && (_46 > gl_GlobalInvocationID.y))
    {
        vec2 _59 = vec2(gl_GlobalInvocationID.xy) / vec2(float(_43), float(_46));
        vec4 _63 = textureLod(SPIRV_Cross_CombinedSceneColors_LinearClamp, _59, 0.0);
        vec4 _66 = textureLod(SPIRV_Cross_CombinedUIColors_LinearClamp, _59, 0.0);
        imageStore(BlendSceneColor, ivec2(gl_GlobalInvocationID.xy), _63);
        imageStore(BlendSceneColorWithUI, ivec2(gl_GlobalInvocationID.xy), vec4((_63.xyz * (1.0 - _66.w)) + _66.xyz, 1.0));
    }
}

