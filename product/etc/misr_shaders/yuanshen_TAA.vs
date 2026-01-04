uniform 	vec4 _CameraDepthTexture_TexelSize;
in highp vec4 in_POSITION0;
out highp vec2 vs_TEXCOORD0;
out highp vec2 vs_TEXCOORD1;
out highp vec4 vs_TEXCOORD2;
out highp vec4 vs_TEXCOORD3;
vec4 u_xlat0;
void main()
{
    gl_Position.xy = in_POSITION0.xy;
    gl_Position.zw = vec2(0.0, 1.0);
    vs_TEXCOORD0.xy = in_POSITION0.xy * vec2(0.5, 0.5) + vec2(0.5, 0.5);
    vs_TEXCOORD1.xy = _CameraDepthTexture_TexelSize.xy * vec2(2.0, 2.0);
    u_xlat0 = in_POSITION0.xyxy + vec4(1.0, 1.0, 1.0, 1.0);
    u_xlat0 = u_xlat0 * vec4(0.5, 0.5, 0.5, 0.5);
    vs_TEXCOORD2 = _CameraDepthTexture_TexelSize.xyxy * vec4(-2.0, -2.0, 2.0, -2.0) + u_xlat0.zwzw;
    vs_TEXCOORD3 = _CameraDepthTexture_TexelSize.xyxy * vec4(-2.0, 2.0, 2.0, 2.0) + u_xlat0;
    return;
}