/**
 * Notes:
 * - This is the shader variant. There is also a version in Unity script.
 *   TODO: I am not sure which is more efficient, yet.
 * - Using `$precision` gave me warnings in the shader graph and I am getting an error 
 *   message when I am trying to use `half`. So I am using `float` now.
 * - TODO: fix edge case where edge overlaps corner when corner is max size
 */

// // INPUT
// float4 Padding;

// float4 Corner_Radius_Adjusted;
// float Border_Width;
// bool Size_Reference; // true=width, false=height

// float Segment;
// float Segment_Position;
// float Start_Offset;
// float End_Offset;
// bool Offset_Reference; // true=segment, false=circumference

// float4 Edge_Lengths;
// float4 Corner_Lengths;

// // OUTPUT
// TODO


// calculating cumulative lengths

float4 edgeCumLengths = float4(0, 0, 0, 0);
float4 cornerCumLengths = float4(0, 0, 0, 0);

float sum = 0;

// left
sum += Edge_Lengths.x;
edgeCumLengths.x = sum;

// left-bottom
sum += Corner_Lengths.x;
cornerCumLengths.x = sum;

// bottom
sum += Edge_Lengths.z;
edgeCumLengths.z = sum;

// right-bottom
sum += Corner_Lengths.z;
cornerCumLengths.z = sum;

// right
sum += Edge_Lengths.y;
edgeCumLengths.y = sum;

// right-top
sum += Corner_Lengths.w;
cornerCumLengths.w = sum;

// top
sum += Edge_Lengths.w;
edgeCumLengths.w = sum;

// left-top
sum += Corner_Lengths.y;
cornerCumLengths.y = sum;

float circumference = cornerCumLengths.y;

// calculate relative start and end positions
float startLength = 0.0;
startLength += (Segment == 0) * (0.0                   + Segment_Position * Edge_Lengths.x  );
startLength += (Segment == 1) * (edgeCumLengths.x      + Segment_Position * Corner_Lengths.x);
startLength += (Segment == 2) * (cornerCumLengths.x    + Segment_Position * Edge_Lengths.z  );
startLength += (Segment == 3) * (edgeCumLengths.z      + Segment_Position * Corner_Lengths.z);
startLength += (Segment == 4) * (cornerCumLengths.z    + Segment_Position * Edge_Lengths.y  );
startLength += (Segment == 5) * (edgeCumLengths.y      + Segment_Position * Corner_Lengths.w);
startLength += (Segment == 6) * (cornerCumLengths.w    + Segment_Position * Edge_Lengths.w  );
startLength += (Segment == 7) * (edgeCumLengths.w      + Segment_Position * Corner_Lengths.y);
startLength /= circumference;

float segmentLength = 0.0;
segmentLength += (Segment == 0) * Edge_Lengths.x;
segmentLength += (Segment == 1) * Corner_Lengths.x;
segmentLength += (Segment == 2) * Edge_Lengths.z;
segmentLength += (Segment == 3) * Corner_Lengths.z;
segmentLength += (Segment == 4) * Edge_Lengths.y;
segmentLength += (Segment == 5) * Corner_Lengths.w;
segmentLength += (Segment == 6) * Edge_Lengths.w;
segmentLength += (Segment == 7) * Corner_Lengths.y;
segmentLength /= circumference;

float relativeStart = Offset_Reference ? 
    startLength + Start_Offset * segmentLength :
    startLength + Start_Offset; 
float relativeEnd = Offset_Reference ? 
    startLength + End_Offset * segmentLength :    
    startLength + End_Offset;               
    
// wrap around relative start and end

float t = 1e-3;
float tmp;

tmp = frac(relativeStart);
float Start = (tmp < t && abs(relativeStart) > t) ? 1.0 : tmp;
tmp = frac(relativeEnd);
float End = (tmp < t && abs(relativeEnd) > t) ? 1.0 : tmp;

// calculate offsets

float position;
float pos;
float4 edgeActivation;
float4 cornerActivation;

position = Start;
pos = position * circumference;

edgeActivation = float4(
    saturate((pos                     ) / Edge_Lengths.x),  // left 
    saturate((pos - cornerCumLengths.z) / Edge_Lengths.y),  // right
    saturate((pos - cornerCumLengths.x) / Edge_Lengths.z),  // bottom
    saturate((pos - cornerCumLengths.w) / Edge_Lengths.w)); // top

cornerActivation = float4(
    saturate((pos - edgeCumLengths.x) / Corner_Lengths.x),
    saturate((pos - edgeCumLengths.w) / Corner_Lengths.y),
    saturate((pos - edgeCumLengths.z) / Corner_Lengths.z),
    saturate((pos - edgeCumLengths.y) / Corner_Lengths.w));

float4 startEdgeActivation = edgeActivation;
float4 startCornerActivation = cornerActivation;

position = End < Start ? 1 : End;
pos = position * circumference;

edgeActivation = float4(
    saturate((pos                     ) / Edge_Lengths.x),  // left 
    saturate((pos - cornerCumLengths.z) / Edge_Lengths.y),  // right
    saturate((pos - cornerCumLengths.x) / Edge_Lengths.z),  // bottom
    saturate((pos - cornerCumLengths.w) / Edge_Lengths.w)); // top

cornerActivation = float4(
    saturate((pos - edgeCumLengths.x) / Corner_Lengths.x),
    saturate((pos - edgeCumLengths.w) / Corner_Lengths.y),
    saturate((pos - edgeCumLengths.z) / Corner_Lengths.z),
    saturate((pos - edgeCumLengths.y) / Corner_Lengths.w));

float4 endEdgeActivation = 1.0 - edgeActivation;
float4 endCornerActivation = cornerActivation;

position = End < Start ? End : 0.0;
pos = position * circumference;

edgeActivation = float4(
    saturate((pos                     ) / Edge_Lengths.x),  // left 
    saturate((pos - cornerCumLengths.z) / Edge_Lengths.y),  // right
    saturate((pos - cornerCumLengths.x) / Edge_Lengths.z),  // bottom
    saturate((pos - cornerCumLengths.w) / Edge_Lengths.w)); // top

cornerActivation = float4(
    saturate((pos - edgeCumLengths.x) / Corner_Lengths.x), // TODO: Here is an issue!
    saturate((pos - edgeCumLengths.w) / Corner_Lengths.y),
    saturate((pos - edgeCumLengths.z) / Corner_Lengths.z),
    saturate((pos - edgeCumLengths.y) / Corner_Lengths.w));

float4 tailEdgeActivation = 1.0 - edgeActivation;
float4 tailCornerActivation = cornerActivation;
// float4 tailCornerActivation = float4(0, cornerActivation.y, cornerActivation.z, cornerActivation.w);

Debug = float(Edge_Lengths.x < 0);

// calculate offsets

First_Edge_Start_Offset =   startEdgeActivation * Edge_Lengths;
First_Edge_End_Offset =     endEdgeActivation * Edge_Lengths;

First_Corner_Start_Offset = startCornerActivation * 0.25;
First_Corner_End_Offset =   endCornerActivation * 0.25;

Second_Edge_End_Offset =   tailEdgeActivation * Edge_Lengths;
Second_Corner_End_Offset = tailCornerActivation * 0.25;


///////
// calculate circle coordinates
///////

float4 outerCoordinates = 
    float4(0.0, 1.0, 0.0, 1.0) 
    + float4(1.0, -1.0, 1.0, -1.0) * Padding;

float4 cornerCenterX = outerCoordinates.xxyy + float4(1.0, 1.0, -1.0, -1.0) * Corner_Radius_Adjusted;
float4 cornerCenterY = outerCoordinates.zwzw + float4(1.0, -1.0, 1.0, -1.0) * Corner_Radius_Adjusted;

float4 halfRadius = Corner_Radius_Adjusted - 0.5 * Border_Width;

float4 edgeCoordinatesX;
float4 edgeCoordinatesY;
float2 leftCoordinates;
float2 rightCoordinates;
float2 bottomCoordinates;
float2 topCoordinates;
float2 leftBottomCoordinates;
float2 leftTopCoordinates;
float2 rightBottomCoordinates;
float2 rightTopCoordinates;
float radians;
float2 coordinates;
float4 cornerOffset;
float4 edgeOffset;

// start circle

edgeOffset = First_Edge_Start_Offset;
cornerOffset = First_Corner_Start_Offset;

// add edge offsets to the start coordinates
edgeCoordinatesX = outerCoordinates.xyxy + float4(
      Border_Width / 2.0,
    - Border_Width / 2.0,
      Corner_Radius_Adjusted.x       + edgeOffset.z,
    - Corner_Radius_Adjusted.w       - edgeOffset.w
);
edgeCoordinatesY = outerCoordinates.wzzw + float4(
    - Corner_Radius_Adjusted.y      - edgeOffset.x,
      Corner_Radius_Adjusted.z      + edgeOffset.y,
      Border_Width / 2.0,
    - Border_Width / 2.0
);

leftCoordinates =   float2(edgeCoordinatesX.x, edgeCoordinatesY.x);
rightCoordinates =  float2(edgeCoordinatesX.y, edgeCoordinatesY.y);
bottomCoordinates = float2(edgeCoordinatesX.z, edgeCoordinatesY.z);
topCoordinates =    float2(edgeCoordinatesX.w, edgeCoordinatesY.w);

radians = (0.0 + cornerOffset.x * 360.0 + 180.0) * (3.14159265 / 180.0);
leftBottomCoordinates = float2(
    cornerCenterX.x + halfRadius.x * cos(radians),
    cornerCenterY.x + halfRadius.x * sin(radians)
);

radians = (270.0 + cornerOffset.y * 360.0 + 180.0) * (3.14159265 / 180.0);
leftTopCoordinates = float2(
    cornerCenterX.y + halfRadius.y * cos(radians),
    cornerCenterY.y + halfRadius.y * sin(radians)
);

radians = (90.0 + cornerOffset.z * 360.0 + 180.0) * (3.14159265 / 180.0);
rightBottomCoordinates = float2(
    cornerCenterX.z + halfRadius.z * cos(radians),
    cornerCenterY.z + halfRadius.z * sin(radians)
);

radians = (180.0 + cornerOffset.w * 360.0 + 180.0) * (3.14159265 / 180.0);
rightTopCoordinates = float2(
    cornerCenterX.w + halfRadius.w * cos(radians),
    cornerCenterY.w + halfRadius.w * sin(radians)
);

pos = Start * circumference;
coordinates = float2(0.0, 0.0);
coordinates += (pos >= 0.0                && pos < edgeCumLengths.x)    * leftCoordinates       ;
coordinates += (pos >= edgeCumLengths.x   && pos < cornerCumLengths.x)  * leftBottomCoordinates ;
coordinates += (pos >= cornerCumLengths.x && pos < edgeCumLengths.z)    * bottomCoordinates     ;
coordinates += (pos >= edgeCumLengths.z   && pos < cornerCumLengths.z)  * rightBottomCoordinates;
coordinates += (pos >= cornerCumLengths.z && pos < edgeCumLengths.y)    * rightCoordinates      ;
coordinates += (pos >= edgeCumLengths.y   && pos < cornerCumLengths.w)  * rightTopCoordinates   ;
coordinates += (pos >= cornerCumLengths.w && pos < edgeCumLengths.w)    * topCoordinates        ;
coordinates += (pos >= edgeCumLengths.w   && pos <= cornerCumLengths.y) * leftTopCoordinates    ;

Start_Coordinates = coordinates;


// end circle

edgeOffset = End < Start ? Second_Edge_End_Offset : First_Edge_End_Offset;
cornerOffset = End < Start ? Second_Corner_End_Offset : First_Corner_End_Offset;

// add edge offsets to the coordinates
edgeCoordinatesX = outerCoordinates.xyyx + float4(
    + Border_Width / 2.0,
    - Border_Width / 2.0,
    - Corner_Radius_Adjusted.z       - edgeOffset.z,
    + Corner_Radius_Adjusted.y       + edgeOffset.w
);
edgeCoordinatesY = outerCoordinates.zwzw + float4(
    + Corner_Radius_Adjusted.x      + edgeOffset.x,
    - Corner_Radius_Adjusted.w      - edgeOffset.y,
    + Border_Width / 2.0,
    - Border_Width / 2.0
);

leftCoordinates =   float2(edgeCoordinatesX.x, edgeCoordinatesY.x);
rightCoordinates =  float2(edgeCoordinatesX.y, edgeCoordinatesY.y);
bottomCoordinates = float2(edgeCoordinatesX.z, edgeCoordinatesY.z);
topCoordinates =    float2(edgeCoordinatesX.w, edgeCoordinatesY.w);

radians = (0.0 + cornerOffset.x * 360.0 + 180.0) * (3.14159265 / 180.0);
leftBottomCoordinates = float2(
    cornerCenterX.x + halfRadius.x * cos(radians),
    cornerCenterY.x + halfRadius.x * sin(radians)
);

radians = (270.0 + cornerOffset.y * 360.0 + 180.0) * (3.14159265 / 180.0);
leftTopCoordinates = float2(
    cornerCenterX.y + halfRadius.y * cos(radians),
    cornerCenterY.y + halfRadius.y * sin(radians)
);

radians = (90.0 + cornerOffset.z * 360.0 + 180.0) * (3.14159265 / 180.0);
rightBottomCoordinates = float2(
    cornerCenterX.z + halfRadius.z * cos(radians),
    cornerCenterY.z + halfRadius.z * sin(radians)
);

radians = (180.0 + cornerOffset.w * 360.0 + 180.0) * (3.14159265 / 180.0);
rightTopCoordinates = float2(
    cornerCenterX.w + halfRadius.w * cos(radians),
    cornerCenterY.w + halfRadius.w * sin(radians)
);

pos = End * circumference;
coordinates = float2(0.0, 0.0);
coordinates += (pos >= 0.0                && pos < edgeCumLengths.x)    * leftCoordinates       ;
coordinates += (pos >= edgeCumLengths.x   && pos < cornerCumLengths.x)  * leftBottomCoordinates ;
coordinates += (pos >= cornerCumLengths.x && pos < edgeCumLengths.z)    * bottomCoordinates     ;
coordinates += (pos >= edgeCumLengths.z   && pos < cornerCumLengths.z)  * rightBottomCoordinates;
coordinates += (pos >= cornerCumLengths.z && pos < edgeCumLengths.y)    * rightCoordinates      ;
coordinates += (pos >= edgeCumLengths.y   && pos < cornerCumLengths.w)  * rightTopCoordinates   ;
coordinates += (pos >= cornerCumLengths.w && pos < edgeCumLengths.w)    * topCoordinates        ;
coordinates += (pos >= edgeCumLengths.w   && pos <= cornerCumLengths.y) * leftTopCoordinates    ;

End_Coordinates = coordinates;
