// INPUTS
// float2 uv;
// float2 center_inner;
// float2 center_outer;
// float radius_inner;
// float radius_outer;

// OUTPUTS
// float g;

float a = distance(center_inner, center_outer);
float2 D = center_inner - center_outer;

// direction from uv to center_inner
float l = length(uv - center_inner);
float2 d = (uv - center_inner) / l;

float dD = dot(d, D);

// use abs(.) to prevent NaNs 
// which are outside of our area of interest anyway
float L = - dD + sqrt(abs(dD * dD - (a * a - radius_outer * radius_outer)));

g = (l - radius_inner) / (L - radius_inner);