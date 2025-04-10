using UnityEngine;

public class FrameFraction : MonoBehaviour
{

    // left right bottom top
    public Vector4 padding;
    // left-bottom, left-top, right-bottom, right-top
    public Vector4 cornerRadius;
    public float borderWidth;
    public float relativeOffset;
    public float relativeStart;
    public float relativeEnd;

    // left, right, bottom, top
    Vector4 edgeLengths;
    // left-bottom, left-top, right-bottom, right-top
    Vector4 cornerLengths;
    // left, right, bottom, top
    Vector4 edgeCumLengths;
    // left-bottom, left-top, right-bottom, right-top
    Vector4 cornerCumLengths;
    float circumference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {        
        // animate offset
        float speed = 1f;
        float pulse = Mathf.PingPong(Time.time * speed, 1f);
        float sawtooth = (Time.time % (1/speed)) / (1/speed);
        relativeOffset = sawtooth;

        FormatShape(transform.localScale, padding, cornerRadius, borderWidth);

        // prepare segment information
        Vector4[] segmentInfos = PrepareSegments(
            transform.localScale, padding, cornerRadius, borderWidth);
        edgeLengths = segmentInfos[0];
        cornerLengths = segmentInfos[1];
        edgeCumLengths = segmentInfos[2];
        cornerCumLengths = segmentInfos[3];
        
        circumference = cornerCumLengths[1];

        FormatSegments(
            relativeStart, 
            relativeEnd, 
            relativeOffset,
            circumference,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths,
            transform.localScale
        );
        
    }

    protected void FormatShape(
        Vector2 scale, 
        Vector4 padding, 
        Vector4 cornerRadius, 
        float borderWidth)
    {

        // Get scale
        float maxScale = Mathf.Max(scale.x, scale.y);
        Vector2 relativeScale = new Vector2(scale.x / maxScale, scale.y / maxScale);
        Vector4 scalePadding = new Vector4(
            0f,
            1 - relativeScale.x,
            0f,
            1 - relativeScale.y
        );

        // Get the material from the renderer
        Material material = GetComponent<Renderer>().material;

        // Set the basic properties of the material
        material.SetVector("_Tiling", relativeScale);
        material.SetVector("_Padding_left_right_bottom_top", scalePadding + padding / maxScale);
        material.SetVector("_Corner_Radius_lb_lt_rb_rt", cornerRadius / maxScale);
        material.SetFloat("_Border_Width", borderWidth / maxScale);

    }

    protected Vector4[] PrepareSegments(
        Vector2 scale,
        Vector4 padding, 
        Vector4 cornerRadius, 
        float borderWidth)
    {

        // left, right, bottom, top edge
        edgeLengths = 
            new Vector4(padding.z, padding.z, padding.x, padding.x)
            + new Vector4(padding.w, padding.w, padding.y, padding.y)
            + new Vector4(cornerRadius.x, cornerRadius.z, cornerRadius.x, cornerRadius.y)
            + new Vector4(cornerRadius.y, cornerRadius.w, cornerRadius.z, cornerRadius.w);
        edgeLengths = new Vector4(scale.y, scale.y, scale.x, scale.x) - edgeLengths;

        // left-bottom, left-top, right-bottom, right-top corner (quarter circumference)
        // For the radii we subtract half the border width.
        // Formula: 2 * pi * r / 4
        float half = borderWidth / 2f;
        Vector4 halfBorder = new Vector4(half, half, half, half);
        cornerLengths = (cornerRadius - halfBorder) * Mathf.PI / 2f;

        // calculate cumulative lengths
        edgeCumLengths = Vector4.zero;
        cornerCumLengths = Vector4.zero;

        Vector4 edgeLengthsClockwise = LeftRightBottomTopToClockwise(edgeLengths);
        Vector4 cornerLengthsClockwise = LbLtRbRtToClockwise(cornerLengths);
        
        // Debug.Log($"edgeLengths:            {edgeLengths}");
        // Debug.Log($"edgeLengthsClockwise:   {edgeLengthsClockwise}");
        // Debug.Log($"cornerLengths:          {cornerLengths}");
        // Debug.Log($"cornerLengthsClockwise: {cornerLengthsClockwise}");

        float sum = 0;

        edgeCumLengths.x    = sum += edgeLengthsClockwise.x;
        // Debug.Log($"sum '+ {edgeLengthsClockwise.x} = {sum}");
        cornerCumLengths.x  = sum += cornerLengthsClockwise.x;
        // Debug.Log($"sum '+ {cornerLengthsClockwise.x} = {sum}");

        edgeCumLengths.y    = sum += edgeLengthsClockwise.y;
        // Debug.Log($"sum '+ {edgeLengthsClockwise.y} = {sum}");
        cornerCumLengths.y  = sum += cornerLengthsClockwise.y;
        // Debug.Log($"sum '+ {cornerLengthsClockwise.y} = {sum}");        

        edgeCumLengths.z    = sum += edgeLengthsClockwise.z;
        // Debug.Log($"sum '+ {edgeLengthsClockwise.z} = {sum}");
        cornerCumLengths.z  = sum += cornerLengthsClockwise.z;
        // Debug.Log($"sum '+ {cornerLengthsClockwise.z} = {sum}");

        edgeCumLengths.w    = sum += edgeLengthsClockwise.w;
        // Debug.Log($"sum '+ {edgeLengthsClockwise.w} = {sum}");
        cornerCumLengths.w  = sum += cornerLengthsClockwise.w;
        // Debug.Log($"sum '+ {cornerLengthsClockwise.w} = {sum}");

        // Debug.Log($"edgeCumLengthsClockwise:    {edgeCumLengths}");
        // Debug.Log($"cornerCumLengthsClockwise:  {cornerCumLengths}");

        edgeCumLengths = ClockwiseToLeftRightBottomTop(edgeCumLengths);
        cornerCumLengths = ClockwiseToLbLtRbRt(cornerCumLengths);

        // Debug.Log($"edgeCumLengths:             {edgeCumLengths}");
        // Debug.Log($"cornerCumLengths:           {cornerCumLengths}");

        return new Vector4[] {
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        };
    }

    protected void FormatSegments(
        float relativeStart, 
        float relativeEnd, 
        float relativeOffset, 
        float circumference, 
        Vector4 edgeLengths, 
        Vector4 cornerLengths,
        Vector4 edgeCumLengths,
        Vector4 cornerCumLengths, 
        Vector2 scale
    )
    {

        Debug.Log($"Start");
        Debug.Log($"relativeStart: {relativeStart}");
        Debug.Log($"relativeEnd: {relativeEnd}");

        // Normalize the relative start and end values
        if (IsClose(relativeEnd - relativeStart, 1f))
        {
            relativeStart = 0f;
            relativeEnd = 1f;
        }
        else 
        {
            relativeStart = RampUp(relativeStart + relativeOffset);
            relativeEnd = RampUp(relativeEnd + relativeOffset);
        }

        Debug.Log($"relativeStart: {relativeStart}");
        Debug.Log($"relativeEnd: {relativeEnd}");

        float maxScale = Mathf.Max(scale.x, scale.y);

        // Get the material from the renderer
        Material material = GetComponent<Renderer>().material;

        // frames
        
        Vector4[] firstFrame = Frame(
            relativeStart,
            relativeEnd < relativeStart ? 1 : relativeEnd,
            circumference,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        Vector4[] secondFrame = Frame(
            0f,
            relativeEnd < relativeStart ? relativeEnd : 0f,
            circumference,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        Vector4 outerCoordinates = 
            new Vector4(0f, 1f * scale.x, 0f, 1f * scale.y) 
            + Multiply4(new Vector4(1f, -1f, 1f, -1f), padding);
        Vector4 innerPadding = AddScalar4(padding, borderWidth / 2f);
        Vector4 innerCoordinates = 
            new Vector4(0f, scale.x, 0f, scale.y) 
            + Multiply4(new Vector4(1f, -1f, 1f, -1f), innerPadding);

        // start coordinates for edges

        Vector4 startEdgeCoordinatesX = new Vector4(
            innerCoordinates.x,
            innerCoordinates.y,
            outerCoordinates.x + cornerRadius.x,
            outerCoordinates.y - cornerRadius.w);
            
        Vector4 startEdgeCoordinatesY = new Vector4(
            outerCoordinates.w - cornerRadius.y,
            outerCoordinates.z + cornerRadius.z,
            innerCoordinates.z,
            innerCoordinates.w);

        Vector4 startOffsetX = new Vector4(  0f,                0f,                 firstFrame[0].z, - firstFrame[0].w);
        Vector4 startOffsetY = new Vector4(- firstFrame[0].x,   firstFrame[0].y,    0f,                0f);

        Vector4 cornerCoordinatesX = new Vector4(
                outerCoordinates.x + cornerRadius.x, 
                outerCoordinates.x + cornerRadius.y, 
                outerCoordinates.y - cornerRadius.z, 
                outerCoordinates.y - cornerRadius.w);
        Vector4 cornerCoordinatesY = new Vector4(
                outerCoordinates.z + cornerRadius.x,
                outerCoordinates.w - cornerRadius.y,
                outerCoordinates.z + cornerRadius.z,
                outerCoordinates.w - cornerRadius.w);
        Vector4 innerBorderRadius = new Vector4(
                cornerRadius.x - borderWidth / 2f,
                cornerRadius.y - borderWidth / 2f,
                cornerRadius.z - borderWidth / 2f,
                cornerRadius.w - borderWidth / 2f);
        Vector4 startAngle = new Vector4(
              0f + firstFrame[1].x * 360f,
            270f + firstFrame[1].y * 360f,
             90f + firstFrame[1].z * 360f,
            180f + firstFrame[1].w * 360f);

        Vector2 startLeftCoordinates = new Vector2(
            startEdgeCoordinatesX.x + startOffsetX.x, 
            startEdgeCoordinatesY.x + startOffsetY.x);
        Vector2 startRightCoordinates = new Vector2(
            startEdgeCoordinatesX.y + startOffsetX.y, 
            startEdgeCoordinatesY.y + startOffsetY.y);
        Vector2 startBottomCoordinates = new Vector2(
            startEdgeCoordinatesX.z + startOffsetX.z, 
            startEdgeCoordinatesY.z + startOffsetY.z);
        Vector2 startTopCoordinates = new Vector2(
            startEdgeCoordinatesX.w + startOffsetX.w, 
            startEdgeCoordinatesY.w + startOffsetY.w);

        Vector2 startLeftBottomCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.x, cornerCoordinatesY.x),
            innerBorderRadius.x,
            startAngle.x);
        Vector2 startLeftTopCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.y, cornerCoordinatesY.y),
            innerBorderRadius.y,
            startAngle.y);
        Vector2 startRightBottomCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.z, cornerCoordinatesY.z),
            innerBorderRadius.z,
            startAngle.z);
        Vector2 startRightTopCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.w, cornerCoordinatesY.w),
            innerBorderRadius.w,
            startAngle.w);

        float pos = relativeStart * circumference;
        Vector2 startCoordinates = Vector2.zero;
        startCoordinates += (pos >= 0f                 & pos < edgeCumLengths.x) ? startLeftCoordinates : Vector2.zero;
        startCoordinates += (pos >= edgeCumLengths.x & pos < cornerCumLengths.x) ? startLeftBottomCoordinates : Vector2.zero;
        startCoordinates += (pos >= cornerCumLengths.x & pos < edgeCumLengths.z) ? startBottomCoordinates : Vector2.zero;
        startCoordinates += (pos >= edgeCumLengths.z & pos < cornerCumLengths.z) ? startRightBottomCoordinates : Vector2.zero;
        startCoordinates += (pos >= cornerCumLengths.z & pos < edgeCumLengths.y) ? startRightCoordinates : Vector2.zero;
        startCoordinates += (pos >= edgeCumLengths.y & pos < cornerCumLengths.w) ? startRightTopCoordinates : Vector2.zero;
        startCoordinates += (pos >= cornerCumLengths.w & pos < edgeCumLengths.w) ? startTopCoordinates : Vector2.zero;
        startCoordinates += (pos >= edgeCumLengths.w & pos <= cornerCumLengths.y) ? startLeftTopCoordinates : Vector2.zero;

        Vector4[] endFrame = relativeEnd < relativeStart ? secondFrame : firstFrame;

        Vector4 endOffsetX = new Vector4(0f,              0f,            - endFrame[2].z, endFrame[2].w);
        Vector4 endOffsetY = new Vector4(endFrame[2].x, - endFrame[2].y, 0f,                0f);

        Vector2 endLeftCoordinates = new Vector2(
            startEdgeCoordinatesX.x + endOffsetX.x, 
            startEdgeCoordinatesY.x - edgeLengths.x + endOffsetY.x);
        Vector2 endRightCoordinates = new Vector2(
            startEdgeCoordinatesX.y + endOffsetX.y, 
            startEdgeCoordinatesY.y + edgeLengths.y + endOffsetY.y);
        Vector2 endBottomCoordinates = new Vector2(
            startEdgeCoordinatesX.z + edgeLengths.z + endOffsetX.z, 
            startEdgeCoordinatesY.z + endOffsetY.z);
        Vector2 endTopCoordinates = new Vector2(
            startEdgeCoordinatesX.w - edgeLengths.w + endOffsetX.w, 
            startEdgeCoordinatesY.w + endOffsetY.w);

        Vector4 endAngle = new Vector4(
              0f + endFrame[3].x * 360f,
            270f + endFrame[3].y * 360f,
             90f + endFrame[3].z * 360f,
            180f + endFrame[3].w * 360f);

        Vector2 endLeftBottomCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.x, cornerCoordinatesY.x),
            innerBorderRadius.x,
            endAngle.x);
        Vector2 endLeftTopCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.y, cornerCoordinatesY.y),
            innerBorderRadius.y,
            endAngle.y);
        Vector2 endRightBottomCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.z, cornerCoordinatesY.z),
            innerBorderRadius.z,
            endAngle.z);
        Vector2 endRightTopCoordinates = ringCoordinates(
            new Vector2(cornerCoordinatesX.w, cornerCoordinatesY.w),
            innerBorderRadius.w,
            endAngle.w);

        pos = relativeEnd * circumference;
        Vector2 endCoordinates = Vector2.zero;
        endCoordinates += (pos >= 0f                 & pos < edgeCumLengths.x) ? endLeftCoordinates           : Vector2.zero;
        endCoordinates += (pos >= edgeCumLengths.x & pos < cornerCumLengths.x) ? endLeftBottomCoordinates     : Vector2.zero;
        endCoordinates += (pos >= cornerCumLengths.x & pos < edgeCumLengths.z) ? endBottomCoordinates         : Vector2.zero;
        endCoordinates += (pos >= edgeCumLengths.z & pos < cornerCumLengths.z) ? endRightBottomCoordinates    : Vector2.zero;
        endCoordinates += (pos >= cornerCumLengths.z & pos < edgeCumLengths.y) ? endRightCoordinates          : Vector2.zero;
        endCoordinates += (pos >= edgeCumLengths.y & pos < cornerCumLengths.w) ? endRightTopCoordinates       : Vector2.zero;
        endCoordinates += (pos >= cornerCumLengths.w & pos < edgeCumLengths.w) ? endTopCoordinates            : Vector2.zero;
        endCoordinates += (pos >= edgeCumLengths.w & pos <= cornerCumLengths.y) ? endLeftTopCoordinates        : Vector2.zero;

        material.SetVector("_First_Start_Offset_left_right_bottom_top", firstFrame[0] / maxScale);
        material.SetVector("_First_Start_Fraction_lb_lt_rb_rt", firstFrame[1]);
        // Debug.Log($"firstFrame[0]: {firstFrame[0]}");
        // Debug.Log($"firstFrame[1]: {firstFrame[1]}");

        material.SetVector("_First_End_Offset_left_right_bottom_top", firstFrame[2] / maxScale);
        material.SetVector("_First_End_Fraction_lb_lt_rb_rt", firstFrame[3]);
        // Debug.Log($"firstFrame[2]: {firstFrame[2]}");
        // Debug.Log($"firstFrame[3]: {firstFrame[3]}");

        // second frame



        material.SetVector("_Second_Start_Offset_left_right_bottom_top", secondFrame[0] / maxScale);
        material.SetVector("_Second_Start_Fraction_lb_lt_rb_rt", secondFrame[1]);

        material.SetVector("_Second_End_Offset_left_right_bottom_top", secondFrame[2] / maxScale);
        material.SetVector("_Second_End_Fraction_lb_lt_rb_rt", secondFrame[3]);

        // circles
        material.SetVector("_Center_1", startCoordinates / maxScale);
        material.SetVector("_Center_2", endCoordinates / maxScale);


    }

    Vector2 ringCoordinates(
        Vector2 center,
        float radius,
        float degrees)
    {
        float radians = (degrees + 180f) * Mathf.Deg2Rad;
        float x = center.x + radius * Mathf.Cos(radians);
        float y = center.y + radius * Mathf.Sin(radians);
        return new Vector2(x, y);
    }

    Vector4 Multiply4(
        Vector4 a, 
        Vector4 b)
    {
        return new Vector4(
            a.x * b.x,
            a.y * b.y,
            a.z * b.z,
            a.w * b.w
        );
    }

    Vector4 AddScalar4(
        Vector4 a, 
        float scalar)
    {
        return new Vector4(
            a.x + scalar,
            a.y + scalar,
            a.z + scalar,
            a.w + scalar
        );
    }

    Vector4 LeftRightBottomTopToClockwise(Vector4 edges)
    {
        return new Vector4(edges.x, edges.z, edges.y, edges.w);
    }

    Vector4 LbLtRbRtToClockwise(Vector4 corners)
    {
        return new Vector4(corners.x, corners.z, corners.w, corners.y);
    }

    Vector4 ClockwiseToLeftRightBottomTop(Vector4 edges)
    {
        return new Vector4(edges.x, edges.z, edges.y, edges.w);
    }

    Vector4 ClockwiseToLbLtRbRt(Vector4 corners)
    {
        return new Vector4(corners.x, corners.w, corners.y, corners.z);
    }

    float SegmentActivation(
        float position, 
        float circumference, 
        float segmentLength,
        float segmentStart
    )
    {   
        return Mathf.Clamp01(
            (position * circumference - segmentStart) 
            / segmentLength);
    }

    float RampUp(float value)
    {
        if (value > 1f | value < 0f) {
            value = value % 1f;
            value = IsClose(value, 0f) ? 1f : value;
        }
        return Mathf.Clamp01(value);
    }


    Vector4[] Frame(
        float relativeStart, 
        float relativeEnd, 
        float circumference, 
        Vector4 edgeLengths,
        Vector4 cornerLengths, 
        Vector4 edgeCumLengths,
        Vector4 cornerCumLengths)
    {

        // start
        edgeLengths = LeftRightBottomTopToClockwise(edgeLengths);
        cornerLengths = LbLtRbRtToClockwise(cornerLengths);
        edgeCumLengths = LeftRightBottomTopToClockwise(edgeCumLengths);
        cornerCumLengths = LbLtRbRtToClockwise(cornerCumLengths);

        Vector4 edgeStartActivation = new Vector4(
            SegmentActivation(relativeStart, circumference, edgeLengths.x, 0f),
            SegmentActivation(relativeStart, circumference, edgeLengths.y, cornerCumLengths.x),
            SegmentActivation(relativeStart, circumference, edgeLengths.z, cornerCumLengths.y),
            SegmentActivation(relativeStart, circumference, edgeLengths.w, cornerCumLengths.z)
        );
        Vector4 cornerStartActivation = new Vector4(
            SegmentActivation(relativeStart, circumference, cornerLengths.x, edgeCumLengths.x),
            SegmentActivation(relativeStart, circumference, cornerLengths.y, edgeCumLengths.y),
            SegmentActivation(relativeStart, circumference, cornerLengths.z, edgeCumLengths.z),
            SegmentActivation(relativeStart, circumference, cornerLengths.w, edgeCumLengths.w)
        );

        // end activation
        Vector4 edgeEndActivation = new Vector4(
            SegmentActivation(relativeEnd, circumference, edgeLengths.x, 0),
            SegmentActivation(relativeEnd, circumference, edgeLengths.y, cornerCumLengths.x),
            SegmentActivation(relativeEnd, circumference, edgeLengths.z, cornerCumLengths.y),
            SegmentActivation(relativeEnd, circumference, edgeLengths.w, cornerCumLengths.z)
        );
        Vector4 cornerEndActivation = new Vector4(
            SegmentActivation(relativeEnd, circumference, cornerLengths.x, edgeCumLengths.x),
            SegmentActivation(relativeEnd, circumference, cornerLengths.y, edgeCumLengths.y),
            SegmentActivation(relativeEnd, circumference, cornerLengths.z, edgeCumLengths.z),
            SegmentActivation(relativeEnd, circumference, cornerLengths.w, edgeCumLengths.w)
        );

        // Debug.Log($"Activations:");
        // Debug.Log($"edgeStartActivation:    {edgeStartActivation}");
        // Debug.Log($"cornerStartActivation:  {cornerStartActivation}");

        // scale
        Vector4 edgeStartOffset = Multiply4(edgeStartActivation, edgeLengths);
        Vector4 cornerStartOffset = cornerStartActivation * 0.25f;

        Vector4 edgeEndOffset = Multiply4(Vector4.one - edgeEndActivation, edgeLengths);
        Vector4 cornerEndOffset = cornerEndActivation * 0.25f;

        return new Vector4[] {
            ClockwiseToLeftRightBottomTop(edgeStartOffset),
            ClockwiseToLbLtRbRt(cornerStartOffset),
            ClockwiseToLeftRightBottomTop(edgeEndOffset),
            ClockwiseToLbLtRbRt(cornerEndOffset)
        };
    }
    bool IsClose(float a, float b, float tolerance = 0.0001f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }
}

