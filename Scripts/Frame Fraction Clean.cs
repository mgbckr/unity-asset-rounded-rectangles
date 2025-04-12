using UnityEngine;

public class FrameFractionClean : MonoBehaviour
{

    // left right bottom top
    public Vector4 padding = new Vector4(0f, 0f, 0f, 0f);
    // left-bottom, left-top, right-bottom, right-top
    public Vector4 cornerRadius = new Vector4(0f, 0f, 0f, 0f);
    public float borderWidth = 0.05f;
    public float relativeOffset = 0f;
    public float relativeStart = 0f;
    public float relativeEnd = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update() {
        // animate offset
        float speed = 1f;
        float pulse = Mathf.PingPong(Time.time * speed, 1f);
        float sawtooth = (Time.time % (1/speed)) / (1/speed);
        // relativeOffset = sawtooth;

        Debug.Log($"Update");
        Layout(
            transform.localScale, 
            padding, 
            cornerRadius, 
            borderWidth,
            relativeStart, 
            relativeEnd, 
            relativeOffset
        );
    }

    // Update is called once per frame
    void Layout(
        Vector2 scale,
        Vector4 worldPadding,
        Vector4 worldCornerRadius,
        float   worldBorderWidth,

        float relativeStart, 
        float relativeEnd, 
        float relativeOffset
    )
    {        

        var shaderShape = DeriveShaderShapeParameters(
            scale, 
            worldPadding, 
            worldCornerRadius, 
            worldBorderWidth);

        // prepare segment information
        // which does not change if the shape is static
        var segmentInfos = DeriveStaticSegmentProperties(
            shaderShape.padding, 
            shaderShape.cornerRadius, 
            shaderShape.borderWidth);
        
        var segmentParameters = DeriveSegmentParameters(
            relativeStart,
            relativeEnd,
            relativeOffset,

            shaderShape.padding,
            shaderShape.cornerRadius,
            shaderShape.borderWidth,

            segmentInfos.edgeLengths,
            segmentInfos.cornerLengths,
            segmentInfos.edgeCumLengths,
            segmentInfos.cornerCumLengths
        );

        // set shader properties

        // get the material from the renderer
        Material material = GetComponent<Renderer>().material;

        // Set the basic properties of the material
        material.SetVector("_Tiling", shaderShape.tiling);
        material.SetVector("_Padding_left_right_bottom_top", shaderShape.padding);
        material.SetVector("_Corner_Radius_lb_lt_rb_rt", shaderShape.cornerRadius);
        material.SetFloat("_Border_Width", shaderShape.borderWidth);


        // // first frame
        material.SetVector("_First_Start_Offset_left_right_bottom_top", segmentParameters.firstStartOffset);
        material.SetVector("_First_Start_Fraction_lb_lt_rb_rt", segmentParameters.firstStartFraction);

        material.SetVector("_First_End_Offset_left_right_bottom_top", segmentParameters.firstEndOffset);
        material.SetVector("_First_End_Fraction_lb_lt_rb_rt", segmentParameters.firstEndFraction);

        // // second frame
        material.SetVector("_Second_Start_Offset_left_right_bottom_top", segmentParameters.secondStartOffset);
        material.SetVector("_Second_Start_Fraction_lb_lt_rb_rt", segmentParameters.secondStartFraction);

        material.SetVector("_Second_End_Offset_left_right_bottom_top", segmentParameters.secondEndOffset);
        material.SetVector("_Second_End_Fraction_lb_lt_rb_rt", segmentParameters.secondEndFraction);

        // // circles
        material.SetVector("_Center_1", segmentParameters.startCoordinates);
        material.SetVector("_Center_2", segmentParameters.endCoordinates);
        
    }

    protected (
            Vector2 tiling, 
            Vector4 padding, 
            Vector4 cornerRadius, 
            float borderWidth)
        DeriveShaderShapeParameters(
            Vector2 scale,
            Vector4 worldPadding,
            Vector4 worldCornerRadius,
            float worldBorderWidth)
    {

        // calculate scaling parameters
        float maxScale = Mathf.Max(scale.x, scale.y);
        Vector2 shaderScale = new Vector2(scale.x / maxScale, scale.y / maxScale);

        // derive padding for accounting for scale
        Vector4 scalePadding = new Vector4(
            0f,
            1 - shaderScale.x,
            0f,
            1 - shaderScale.y
        );

        // finalize shape parameters in shader coordinates
        Vector2 tiling = shaderScale;
        Vector4 shaderPadding = scalePadding + worldPadding / maxScale;
        Vector4 shaderCornerRadius = worldCornerRadius / maxScale;
        float shaderBorderWidth = worldBorderWidth / maxScale;

        // ensure corner radius to be at least border width
        shaderCornerRadius = new Vector4(
            Mathf.Max(shaderCornerRadius.x, shaderBorderWidth),
            Mathf.Max(shaderCornerRadius.y, shaderBorderWidth),
            Mathf.Max(shaderCornerRadius.z, shaderBorderWidth),
            Mathf.Max(shaderCornerRadius.w, shaderBorderWidth)
        );

        return (tiling, shaderPadding, shaderCornerRadius, shaderBorderWidth);
    }

    protected (
            Vector4 edgeLengths, 
            Vector4 cornerLengths, 
            Vector4 edgeCumLengths, 
            Vector4 cornerCumLengths) 
        DeriveStaticSegmentProperties(
            Vector4 shaderPadding, 
            Vector4 shaderCornerRadius,
            float shaderBorderWidth)
    {

        // left, right, bottom, top edge
        var edgeLengths = 
            new Vector4(shaderPadding.z, shaderPadding.z, shaderPadding.x, shaderPadding.x)
            + new Vector4(shaderPadding.w, shaderPadding.w, shaderPadding.y, shaderPadding.y)
            + new Vector4(shaderCornerRadius.x, shaderCornerRadius.z, shaderCornerRadius.x, shaderCornerRadius.y)
            + new Vector4(shaderCornerRadius.y, shaderCornerRadius.w, shaderCornerRadius.z, shaderCornerRadius.w);
        edgeLengths = Vector4.one - edgeLengths;

        // left-bottom, left-top, right-bottom, right-top corner (quarter circumference)
        // For the radii we subtract half the border width.
        // Formula: 2 * pi * r / 4
        float half = shaderBorderWidth / 2f;
        Vector4 halfBorder = new Vector4(half, half, half, half);
        var cornerLengths = (shaderCornerRadius - halfBorder) * Mathf.PI / 2f;

        // calculate cumulative lengths
        var edgeCumLengths = Vector4.zero;
        var cornerCumLengths = Vector4.zero;

        Vector4 edgeLengthsClockwise = LeftRightBottomTopToClockwise(edgeLengths);
        Vector4 cornerLengthsClockwise = LbLtRbRtToClockwise(cornerLengths);
        
        float sum = 0;

        edgeCumLengths.x    = sum += edgeLengthsClockwise.x;
        cornerCumLengths.x  = sum += cornerLengthsClockwise.x;

        edgeCumLengths.y    = sum += edgeLengthsClockwise.y;
        cornerCumLengths.y  = sum += cornerLengthsClockwise.y;

        edgeCumLengths.z    = sum += edgeLengthsClockwise.z;
        cornerCumLengths.z  = sum += cornerLengthsClockwise.z;

        edgeCumLengths.w    = sum += edgeLengthsClockwise.w;
        cornerCumLengths.w  = sum += cornerLengthsClockwise.w;

        edgeCumLengths = ClockwiseToLeftRightBottomTop(edgeCumLengths);
        cornerCumLengths = ClockwiseToLbLtRbRt(cornerCumLengths);

        return (
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );
    }

    protected (
        Vector4 firstStartOffset,
        Vector4 firstStartFraction,
        Vector4 firstEndOffset,
        Vector4 firstEndFraction,
        Vector4 secondStartOffset,
        Vector4 secondStartFraction,
        Vector4 secondEndOffset,
        Vector4 secondEndFraction,
        Vector2 startCoordinates,
        Vector2 endCoordinates
    ) 
    DeriveSegmentParameters(
        float relativeStart,
        float relativeEnd,
        float relativeOffset,

        Vector4 padding,
        Vector4 cornerRadius,
        float borderWidth,

        Vector4 edgeLengths,
        Vector4 cornerLengths,
        Vector4 edgeCumLengths,
        Vector4 cornerCumLengths
    )
    {

        // Normalize the relative start and end values
        if (IsClose(relativeEnd - relativeStart, 1f))
        {
            // TODO: Do we still need this with current RampUp implementation?
            relativeStart = 0f;
            relativeEnd = 1f;
        }
        else 
        {
            relativeStart = RampUp(relativeStart + relativeOffset);
            relativeEnd = RampUp(relativeEnd + relativeOffset);
        }

        // frames
        float circumference = cornerCumLengths.y;
        Vector4[] firstFrame = CalculateSegmentOffsets(
            relativeStart,
            relativeEnd < relativeStart ? 1 : relativeEnd,
            circumference,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        Vector4[] secondFrame = CalculateSegmentOffsets(
            0f,
            relativeEnd < relativeStart ? relativeEnd : 0f,
            circumference,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        Vector4 outerCoordinates = 
            new Vector4(0f, 1f, 0f, 1f)
            + Multiply4(new Vector4(1f, -1f, 1f, -1f), padding);
        Vector4 innerPadding = AddScalar4(padding, borderWidth / 2f);
        Vector4 innerCoordinates = 
            new Vector4(0f, 1f, 0f, 1f) 
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


        return (
            firstFrame[0],
            firstFrame[1],
            firstFrame[2],
            firstFrame[3],
            secondFrame[0],
            secondFrame[1],
            secondFrame[2],
            secondFrame[3],
            startCoordinates,
            endCoordinates
        );

    }


    Vector4[] CalculateSegmentOffsets(
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
            Debug.Log($"value: {value}");
            value = ((value % 1f) + 1f) % 1f;
            Debug.Log($"value % 1: {value}");
            value = IsClose(value, 0f) ? 1f : value;
            Debug.Log($"value == 0: {value}");
        }
        return Mathf.Clamp01(value);
    }

    bool IsClose(float a, float b, float tolerance = 0.0001f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }
}

