layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(binding = 0, std140) uniform type_Globals
{
    uvec4 MinBounds;
    uvec4 MaxBounds;
} _Globals;

layout(binding = 6, r32ui) uniform writeonly highp uimage2D rw_game_motion_vector_field_x;

void main()
{
    uvec2 _30 = gl_GlobalInvocationID.xy + _Globals.MinBounds.xy;
    if ((_30.x < _Globals.MaxBounds.x) && (_30.y < _Globals.MaxBounds.y))
    {
        imageStore(rw_game_motion_vector_field_x, ivec2(_30.xy), uvec4(0u));
    }
}

