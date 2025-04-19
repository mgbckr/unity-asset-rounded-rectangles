
    // Compute normalized coordinate space.
    float w = min(iResolution.x, iResolution.y) / 2.0;
    float2 p = (fragCoord * 2.0 - iResolution) / w;
    float2 m = (iMouse * 2.0 - iResolution) / w;
    // Use the absolute value of m as the half-size parameter.
    float2 xy = abs(m);
    float r = 0.7; // Outer corner radius

    // --- Inline "sd_rounded_rectangle" (outer shape) ---
    // For sd_rounded_rectangle, we first need to compute the "rounded" offset.
    // In ShaderToy the quadrant is used to pick a corner radius,
    // here we simply use r (clamped by the half-size).
    float s = r;
    s = min(s, min(xy.x, xy.y));



    // Inline "sd_rectangle": compute the signed distance for a rectangle with half sizes.
    float2 rectSize = xy - s;
    float2 d = abs(p) - max(rectSize, float2(0.0, 0.0));
    float outerRect = length(max(d, float2(0.0, 0.0)));
    float innerRect = min(max(d.x, d.y), 0.0);
    float d2 = outerRect + innerRect - s; // This is the SDF for the outer rounded rectangle.

    // --- Inline "sd_border" ---
    // Define the border widths [top, right, bottom, left].
    float4 border = float4(0.1, 0.2, 0.3, 0.4);
    float2 dp = float2(border.y - border.w, border.x - border.z) / 2.0;
    float2 inner_p = p + dp;
    float2 dxy = float2(border.y + border.w, border.x + border.z) / 2.0;
    float2 inner_xy = xy - dxy;

    Out = inner_xy;

    float d_final;
    // If the outer SDF is positive or the inner half-size is invalid, use the outer SDF.
    if (d2 > 0.0 || inner_xy.x < 0.0 || inner_xy.y < 0.0)
    {
        d_final = d2;
    }
    else
    {
        // Compute inner corner radii (for each corner: top-left, top-right, bottom-left, bottom-right)
        float r0 = max(r - max(border.w, border.x), 0.0);
        float r1 = max(r - max(border.x, border.y), 0.0);
        float r2 = max(r - max(border.z, border.w), 0.0);
        float r3 = max(r - max(border.y, border.z), 0.0);

        // --- Inline "sd_rounded_rectangle" for inner shape ---
        // Choose the corner radius based on the quadrant of inner_p.
        int indexInner = (inner_p.x > 0.0 ? 1 : 0) + (inner_p.y < 0.0 ? 2 : 0);
        float s_inner = (indexInner == 0 ? r0 : (indexInner == 1 ? r1 : (indexInner == 2 ? r2 : r3)));
        s_inner = min(s_inner, min(inner_xy.x, inner_xy.y));
        float2 rectSizeInner = inner_xy - s_inner;
        float2 d_inner = abs(inner_p) - max(rectSizeInner, float2(0.0, 0.0));
        float outerRectInner = length(max(d_inner, float2(0.0, 0.0)));
        float innerRectInner = min(max(d_inner.x, d_inner.y), 0.0);
        float d1 = outerRectInner + innerRectInner - s_inner;
        
        // Use the inner SDF to adjust the final border SDF.
        d_final = (d1 < 0.0 ? -d1 : max(-d1, d2));
    }

    Out = d_final;
    
    // // --- Visualization ---
    // // Convert the signed distance to a color.
    // float3 col = float3(1.0, 1.0, 1.0) - sign(d_final) * float3(0.1, 0.4, 0.7);
    // col *= 1.0 - exp(-2.0 * abs(d_final));
    // col *= (0.8 + 0.2 * cos(120.0 * d_final));
    // col = lerp(col, float3(1.0, 1.0, 1.0), 1.0 - smoothstep(0.0, 0.01, abs(d_final)));

    // //Out = float4(col, 1.0);
	// Out = abs(d_final);