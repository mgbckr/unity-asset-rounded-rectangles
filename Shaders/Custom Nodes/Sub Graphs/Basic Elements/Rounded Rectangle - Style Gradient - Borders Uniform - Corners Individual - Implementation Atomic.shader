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

    // border width can not be larger than the minimum border radius
    Border_Width = min(
        Border_Width,
        min(
            min(Radius.x, Radius.y),
            min(Radius.z, Radius.w)));

    // set radius per quadrant / corner
    float r = UV.x < 0.5 
        ? (UV.y < 0.5 ? Radius.x : Radius.y) 
        : (UV.y < 0.5 ? Radius.z : Radius.w);
    float diameter = r * 2;

    // set UV based on size and diameters
    float2 uv = abs(UV * 2 - 1) - Size + diameter;

    // signed distance field to rounded rectangle
    float d = length(max(0, uv)) / diameter;

    // used for smoothing
    // TODO: inefficient to add this?
    float fwd = 1 / max(fwidth(d), 1e-5);

    // border width or smoothing factors
    float b = min(r / Border_Width, fwd);

    // final rectangle
    Out = saturate((1 - d) * b);