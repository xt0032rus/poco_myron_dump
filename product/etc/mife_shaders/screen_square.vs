layout (location = 4) uniform highp float depth;
layout(location = 0) out highp vec2 in_TEXCOORD0;

void main()
{
    float x = -1.0 + float((gl_VertexID & 1) << 2);
    float y = -1.0 + float((gl_VertexID & 2) << 1);

    gl_Position = vec4(x, y, depth, 1);

    in_TEXCOORD0.x = (x+1.0)*0.5;
    in_TEXCOORD0.y = (y+1.0)*0.5;
}