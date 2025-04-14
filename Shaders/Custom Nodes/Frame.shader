// order counter clockwise for convenience
Padding = Padding.xzyw;
Corner_Radius = Corner_Radius.xzwy;
Start_Offset = Start_Offset.xzyw;
End_Offset = End_Offset.xzyw;


float4 Outer_Padding = Padding;
float4 Inner_Padding = 1 - Border_Width - Outer_Padding;

float4 Start_Padding = Outer_Padding.wxyz + Corner_Radius.wxyz;
float4 End_Padding =   Outer_Padding.yzwx + Corner_Radius.xyzw;

Left = float4(
    Outer_Padding.x, Inner_Padding.x, 
    End_Padding.x + End_Offset.x, Start_Padding.x + Start_Offset.x);

Bottom = float4(
    Start_Padding.y + Start_Offset.y, End_Padding.y + End_Offset.y,
    Outer_Padding.y, Inner_Padding.y);

Right = float4(
    Inner_Padding.z, Outer_Padding.z,
    Start_Padding.z + Start_Offset.z, End_Padding.z + End_Offset.z);

Top = float4(
    End_Padding.w + End_Offset.w, Start_Padding.w + Start_Offset.w,
    Inner_Padding.w, Outer_Padding.w);

Center_Left_Bottom =    float2(    Start_Padding.y,       End_Padding.x);
Center_Right_Bottom =   float2(1 -   End_Padding.y,     Start_Padding.z);
Center_Right_Top =      float2(1 - Start_Padding.w, 1 -   End_Padding.z);
Center_Left_Top =       float2(      End_Padding.w, 1 - Start_Padding.x);

float4 Inner_Corner_Radius = Corner_Radius - Border_Width;
Radius_Left_Bottom =    float2(Inner_Corner_Radius.x, Corner_Radius.x);
Radius_Right_Bottom =   float2(Inner_Corner_Radius.y, Corner_Radius.y);
Radius_Right_Top =      float2(Inner_Corner_Radius.z, Corner_Radius.z);
Radius_Left_Top =       float2(Inner_Corner_Radius.w, Corner_Radius.w);
