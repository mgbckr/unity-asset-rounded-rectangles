// INPUT
// float2 UV;
// float2 Size;
// float4 Radius;

// OUTPUT
// float Out_uv;   // debugging
// float Out_d;    // debugging
// float Out;

// radii can be maximally half the width and half the height
Radius = max(
    min(
        min(
            abs(Size.x / 2),
            abs(Size.y / 2)
        ), 
        abs(Radius)
    ), 
    1e-5);

// set radius per quadrant / corner
float r = UV.x < 0.5 
    ? (UV.y < 0.5 ? Radius.x : Radius.y) 
    : (UV.y < 0.5 ? Radius.z : Radius.w);
float diameter = r * 2;

// set UV based on size and diameters
float2 uv = abs(UV * 2 - 1) - Size + diameter;
Out_uv = uv;

// signed distance field to rounded rectangle
float d = length(max(0, uv)) / diameter;
d = min(1, 1 - (1 - d) * r);
Out_d = d;

// for anti-aliasing
float fwd = max(fwidth(d), 1e-5);
Out_fwd = fwd;

// final rectangle
Out = saturate((1 - d) / fwd);