in highp vec3 in_POSITION0;
out highp vec2 vs_TEXCOORD0;
vec2 u_xlat0;
void main()
{
    gl_Position.xy = in_POSITION0.xy;
    gl_Position.zw = vec2(0.0, 1.0);
    u_xlat0.xy = in_POSITION0.xy + vec2(1.0, 1.0);
    vs_TEXCOORD0.xy = u_xlat0.xy * vec2(0.5, 0.5);
    return;
}