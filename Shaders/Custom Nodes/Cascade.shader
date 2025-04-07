// Resort values
Edge_Lengths = Edge_Lengths.xzyw;
Corner_Lengths = Corner_Lengths.xzwy;

// Init output
Edge_Mask = float4(0.0, 0.0, 0.0, 0.0);
Edge_Values = float4(0.0, 0.0, 0.0, 0.0);
Corner_Mask = float4(0.0, 0.0, 0.0, 0.0);
Corner_Values = float4(0.0, 0.0, 0.0, 0.0);
Inverse_Edge_Values = float4(0.0, 0.0, 0.0, 0.0);


// Interleave Edge and Corner, storing cumulative sum
float s0 = 0.0;
float s1 = s0 + Edge_Lengths.x;
float s2 = s1 + Corner_Lengths.x;
float s3 = s2 + Edge_Lengths.y;
float s4 = s3 + Corner_Lengths.y;
float s5 = s4 + Edge_Lengths.z;
float s6 = s5 + Corner_Lengths.z;
float s7 = s6 + Edge_Lengths.w;
float s8 = s7 + Corner_Lengths.w;

// Prepare
float Total = s8;
float Pos = Activation * Total;

// Edge 0
Edge_Mask.x = Pos >= 0 && Pos < s1;
Edge_Values.x = saturate((Pos - s0) / Edge_Lengths.x);

// Corner 0
Corner_Mask.x = Pos >= s1 && Pos < s2;
Corner_Values.x = saturate((Pos - s1) / Corner_Lengths.x);

// Edge 1
Edge_Mask.y = Pos >= s2 && Pos < s3;
Edge_Values.y = saturate((Pos - s2) / Edge_Lengths.y);

// Corner 1
Corner_Mask.y  = Pos >= s3 && Pos < s4;
Corner_Values.y = saturate((Pos - s3) / Corner_Lengths.y);

// Edge 2
Edge_Mask.z = Pos >= s4 && Pos < s5;
Edge_Values.z = saturate((Pos - s4) / Edge_Lengths.z);

// Corner 2
Corner_Mask.z = Pos >= s5 && Pos < s6;
Corner_Values.z = saturate((Pos - s5) / Corner_Lengths.z);

// Edge 3
Edge_Mask.w = Pos >= s6 && Pos < s7;
Edge_Values.w = saturate((Pos - s6) / Edge_Lengths.w);

// Corner 3
Corner_Mask.w = Pos >= s7 && Pos <= s8;
Corner_Values.w = saturate((Pos - s7) / Corner_Lengths.w);


// Scale values
Edge_Values *= Edge_Lengths;
Corner_Values *= 0.25;
Inverse_Edge_Values = Edge_Lengths - Edge_Values;

// Sort values
Edge_Mask = Edge_Mask.xzyw;
Corner_Mask = Corner_Mask.xwyz;
Edge_Values = Edge_Values.ywzx;
Corner_Values = Corner_Values.xwyz;
Inverse_Edge_Values = Inverse_Edge_Values.wyxz;