// INPUTS
// float2 uv;
// float2 center;
// float radius;

// one of these:
// float2 offset = float2(0, radius + border_width.y); 
// float2 offset = float2(radius + border_width.y, 0);

// OUTPUTS
// float g;

float2 p = center + offset;

float a = length(center - p);
float2 D = center - p;

// direction from uv to center
float l = length(uv - center);
float2 d = (uv - center) / l;

float dD = dot(d, D);

float L = -(a*a) / dD;

g = (l - radius) / (L - radius);