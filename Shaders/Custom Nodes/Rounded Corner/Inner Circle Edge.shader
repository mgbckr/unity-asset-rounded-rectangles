// INPUTS
// float2 uv;
// float2 center_inner;
// float radius_inner;

// one of these:
// float2(0, radius_inner + border_width.y)
// float2(radius_inner + border_width.y, 0)
// float2 offset; 

// OUTPUTS
// float g;

float2 p = center_inner + offset;

float a = length(center_inner - p);
float2 D = center_inner - p;

// direction from uv to center_inner
float l = length(uv - center_inner);
float2 d = (uv - center_inner) / l;

float dD = dot(d, D);

float L = -(a*a) / dD;

g = (l - radius_inner) / (L - radius_inner);