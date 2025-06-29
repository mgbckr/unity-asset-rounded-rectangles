// // INPUT
// float4 Padding;
// float4 Corner_Radius;
// float Border_Width;

// // OUTPUT
// float4 Corner_Radius_Adjusted;
// float4 Edge_Lengths;
// float4 Corner_Lengths;

// ensure corner radius to be at least border width
Corner_Radius_Adjusted = max(Corner_Radius, Border_Width);

Edge_Lengths =
    Padding.zzxx
    + Padding.wwyy
    + Corner_Radius_Adjusted.xzxy
    + Corner_Radius_Adjusted.ywzw;

Edge_Lengths = 1 - Edge_Lengths;
// ensure that edge lengths are never negative
// TODO: should this be caught differently somehow?
Edge_Lengths = max(Edge_Lengths, 0.0);

Corner_Lengths = Corner_Radius_Adjusted * 3.14159265359 / 2;
