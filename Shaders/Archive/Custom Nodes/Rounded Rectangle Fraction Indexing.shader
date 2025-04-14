// // Input
// // left, right, bottom, top
// Padding = float4(0.0, 0.0, 0.0, 0.0);
// // Vector4: left-bottom, left-top, right-bottom, right-top
// Corner_Radius = float4(0.0, 0.0, 0.0, 0.0);
// // flat: border width
// Border_Width = 0.0;
// // segment index
// Segment_Index = 0;
// // origin relative to segment
// Relative_Origin = 0.0;
// // absolute start offset
// Absolute_Start_Offset = 0.0;
// // absolute end offset
// Absolute_End_Offset = 0.0;
// // width and height of the rectangle
// Absolute_Scale = float2(0.0, 0.0);

// // Output
// // origin, start, end
// Origin_Out = 0.0;
// Start_Out = 0.0;
// End_Out = 0.0;

// calculate edge lengths
// left, right, bottom, top edge
float4 Edge_Lengths = 
    Padding.zzxx + Padding.wwyy
    + Corner_Radius.xzxy + Corner_Radius.ywzw;
Edge_Lengths = Absolute_Scale.yyxx - Edge_Lengths;

// Calculate corner lengths:
// For the radii we subtract half the border width.
// Formula: 2 * pi * r / 4
// left-bottom, left-top, right-bottom, right-top corner (quarter circumference)
float4 Corner_Lengths = 
    (Corner_Radius - Border_Width * 0.5) * 3.14159265359 * 0.5;

float Circumference = 
    Edge_Lengths.x + Corner_Lengths.x
    + Edge_Lengths.y + Corner_Lengths.y
    + Edge_Lengths.z + Corner_Lengths.z
    + Edge_Lengths.w + Corner_Lengths.w;

float Start_Length = 
    Edge_Lengths.x * (Segment_Index > 0)
    + Corner_Lengths.x * (Segment_Index > 1)
    + Edge_Lengths.z * (Segment_Index > 2)
    + Corner_Lengths.z * (Segment_Index > 3)
    + Edge_Lengths.y * (Segment_Index > 4)
    + Corner_Lengths.w * (Segment_Index > 5)
    + Edge_Lengths.w * (Segment_Index > 6)
    + Corner_Lengths.y * (Segment_Index > 7);

float Segment_Length =
    Edge_Lengths.x * (Segment_Index == 0)
    + Corner_Lengths.x * (Segment_Index == 1)
    + Edge_Lengths.z * (Segment_Index == 2)
    + Corner_Lengths.z * (Segment_Index == 3)
    + Edge_Lengths.y * (Segment_Index == 4)
    + Corner_Lengths.w * (Segment_Index == 5)
    + Edge_Lengths.w * (Segment_Index == 6)
    + Corner_Lengths.y * (Segment_Index == 7);


float OriginLength = Start_Length + Relative_Origin * Segment_Length;

Origin_Out = (OriginLength + Absolute_Start_Offset) / Circumference;
Start_Out = 0.0;
End_Out = (Absolute_End_Offset - Absolute_Start_Offset) / Circumference;

