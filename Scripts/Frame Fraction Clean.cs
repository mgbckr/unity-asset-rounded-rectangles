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

        // circumference
        float circumference = cornerCumLengths.y;

        // frames
        var firstFrame = CalculateSegmentOffsets(
            relativeStart,
            relativeEnd < relativeStart ? 1 : relativeEnd,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        var secondFrame = CalculateSegmentOffsets(
            0f,
            relativeEnd < relativeStart ? relativeEnd : 0f,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        // basic coordinates

        Vector4 outerCoordinates = 
            new Vector4(0f, 1f, 0f, 1f)
            + Multiply4(new Vector4(1f, -1f, 1f, -1f), padding);

        // corner coordinates

        Vector4 cornerCenterX = new Vector4(
                outerCoordinates.x + cornerRadius.x, 
                outerCoordinates.x + cornerRadius.y, 
                outerCoordinates.y - cornerRadius.z, 
                outerCoordinates.y - cornerRadius.w);

        Vector4 cornerCenterY = new Vector4(
                outerCoordinates.z + cornerRadius.x,
                outerCoordinates.w - cornerRadius.y,
                outerCoordinates.z + cornerRadius.z,
                outerCoordinates.w - cornerRadius.w);

        // select the frame containing the end of the frame
        var endFrame = relativeEnd < relativeStart ? secondFrame : firstFrame;

        Vector2 startCoordinates = GapCoordinates(
            outerCoordinates,
            cornerCenterX,
            cornerCenterY,
            firstFrame.edgeStartOffset,
            firstFrame.cornerStartOffset,
            cornerRadius,
            borderWidth,
            circumference,
            edgeCumLengths,
            cornerCumLengths,
            relativeStart,
            true
        );

        Vector2 endCoordinates = GapCoordinates(
            outerCoordinates,
            cornerCenterX,
            cornerCenterY,
            endFrame.edgeEndOffset,
            endFrame.cornerEndOffset,
            cornerRadius,
            borderWidth,
            circumference,
            edgeCumLengths,
            cornerCumLengths,
            relativeEnd,
            false
        );

        return (
            firstFrame.edgeStartOffset,
            firstFrame.cornerStartOffset,
            firstFrame.edgeEndOffset,
            firstFrame.cornerEndOffset,
            secondFrame.edgeStartOffset,
            secondFrame.cornerStartOffset,
            secondFrame.edgeEndOffset,
            secondFrame.cornerEndOffset,
            startCoordinates,
            endCoordinates
        );
    }

    protected
    (
        Vector4 edgeStartOffset,
        Vector4 cornerStartOffset,
        Vector4 edgeEndOffset,
        Vector4 cornerEndOffset) 
    CalculateSegmentOffsets(
        float relativeStart,
        float relativeEnd,
        Vector4 edgeLengths,
        Vector4 cornerLengths,
        Vector4 edgeCumLengths,
        Vector4 cornerCumLengths)
    {

        var startActivation = CalculateSegmentActivation(
            relativeStart,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        var endActivation = CalculateSegmentActivation(
            relativeEnd,
            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths
        );

        // scale
        Vector4 edgeStartOffset = Multiply4(
            startActivation.edgeActivation, 
            edgeLengths);
        Vector4 cornerStartOffset = startActivation.cornerActivation * 0.25f;

        Vector4 edgeEndOffset = Multiply4(
            Vector4.one - endActivation.edgeActivation, 
            edgeLengths);
        Vector4 cornerEndOffset = endActivation.cornerActivation * 0.25f;

        return (
            edgeStartOffset,
            cornerStartOffset,
            edgeEndOffset,
            cornerEndOffset);
    }


    protected (
        Vector4 edgeActivation,
        Vector4 cornerActivation
    ) 
    CalculateSegmentActivation(
        float position,
        Vector4 edgeLengths,
        Vector4 cornerLengths,
        Vector4 edgeCumLengths,
        Vector4 cornerCumLengths)
    {

        // circumference
        float circumference =   cornerCumLengths.y;

        // sort values in clockwise order for better readability
        // we could get rid of this for reducing code length
        edgeLengths =       LeftRightBottomTopToClockwise(edgeLengths);
        cornerLengths =     LbLtRbRtToClockwise(cornerLengths);
        edgeCumLengths =    LeftRightBottomTopToClockwise(edgeCumLengths);
        cornerCumLengths =  LbLtRbRtToClockwise(cornerCumLengths);

        // get circumference
        Debug.Log($"position: {position}");
        Debug.Log($"circumference: {circumference}");
        float pos =             position * circumference;
        Debug.Log($"pos: {pos}");

        Vector4 edgeActivation = new Vector4(
            SegmentActivation(pos, edgeLengths.x, 0f),
            SegmentActivation(pos, edgeLengths.y, cornerCumLengths.x),
            SegmentActivation(pos, edgeLengths.z, cornerCumLengths.y),
            SegmentActivation(pos, edgeLengths.w, cornerCumLengths.z)
        );
        Vector4 cornerActivation = new Vector4(
            SegmentActivation(pos, cornerLengths.x, edgeCumLengths.x),
            SegmentActivation(pos, cornerLengths.y, edgeCumLengths.y),
            SegmentActivation(pos, cornerLengths.z, edgeCumLengths.z),
            SegmentActivation(pos, cornerLengths.w, edgeCumLengths.w)
        );

        // sort values back to original order
        edgeActivation =    ClockwiseToLeftRightBottomTop(edgeActivation);
        cornerActivation =  ClockwiseToLbLtRbRt(cornerActivation);

        Debug.Log($"edgeActivation: {edgeActivation}");
        Debug.Log($"cornerActivation: {cornerActivation}");

        // return
        return (
            edgeActivation,
            cornerActivation
        );
    }

    protected Vector2 GapCoordinates(
        Vector4 outerCoordinates,
        Vector4 cornerCenterX,
        Vector4 cornerCenterY,
        Vector4 edgeOffsets,
        Vector4 cornerOffsets,
        Vector4 cornerRadius,
        float borderWidth,
        float circumference,
        Vector4 edgeCumLengths,
        Vector4 cornerCumLengths,
        float relativePosition,
        bool start = true
    )
    {
        Vector4 edgeCoordinatesX;
        Vector4 edgeCoordinatesY;
        if (start)
        {
            edgeCoordinatesX = new Vector4(
                outerCoordinates.x + borderWidth / 2f,
                outerCoordinates.y - borderWidth / 2f,
                outerCoordinates.x + cornerRadius.x,
                outerCoordinates.y - cornerRadius.w);
                
            edgeCoordinatesY = new Vector4(
                outerCoordinates.w - cornerRadius.y,
                outerCoordinates.z + cornerRadius.z,
                outerCoordinates.z + borderWidth / 2f,
                outerCoordinates.w - borderWidth / 2f);
        }
        else 
        {
            edgeCoordinatesX = new Vector4(
                outerCoordinates.x + borderWidth / 2f,
                outerCoordinates.y - borderWidth / 2f,
                outerCoordinates.y - cornerRadius.z,
                outerCoordinates.x + cornerRadius.y);
                
            edgeCoordinatesY = new Vector4(
                outerCoordinates.z + cornerRadius.x,
                outerCoordinates.w - cornerRadius.w,
                outerCoordinates.z + borderWidth / 2f,
                outerCoordinates.w - borderWidth / 2f);
        }

        // add edge offsets to the start coordinates
        edgeCoordinatesX = Add4(
            edgeCoordinatesX, 
            new Vector4(
                0f,
                0f,
                (start ? 1f : -1f) * edgeOffsets.z,
                (start ? -1f : 1f) * edgeOffsets.w));

        edgeCoordinatesY = Add4(
            edgeCoordinatesY,
            new Vector4(
                (start ? -1f : 1f) * edgeOffsets.x,
                (start ? 1f : -1f) * edgeOffsets.y,
                0f,
                0f));

        // start edge offsets
        Vector2 leftCoordinates =   new Vector2(edgeCoordinatesX.x, edgeCoordinatesY.x);
        Vector2 rightCoordinates =  new Vector2(edgeCoordinatesX.y, edgeCoordinatesY.y);
        Vector2 bottomCoordinates = new Vector2(edgeCoordinatesX.z, edgeCoordinatesY.z);
        Vector2 topCoordinates =    new Vector2(edgeCoordinatesX.w, edgeCoordinatesY.w);

        // convert relative corner offset into degrees
        Vector4 startAngle = new Vector4(
              0f + cornerOffsets.x * 360f,
            270f + cornerOffsets.y * 360f,
             90f + cornerOffsets.z * 360f,
            180f + cornerOffsets.w * 360f);

        // corner offset coordinates
        Vector2 leftBottomCoordinates = RingCoordinates(
            new Vector2(cornerCenterX.x, cornerCenterY.x),
            cornerRadius.x - borderWidth / 2f, 
            startAngle.x);
        Vector2 leftTopCoordinates = RingCoordinates(
            new Vector2(cornerCenterX.y, cornerCenterY.y),
            cornerRadius.y - borderWidth / 2f,
            startAngle.y);
        Vector2 rightBottomCoordinates = RingCoordinates(
            new Vector2(cornerCenterX.z, cornerCenterY.z),
            cornerRadius.z - borderWidth / 2f,
            startAngle.z);
        Vector2 rightTopCoordinates = RingCoordinates(
            new Vector2(cornerCenterX.w, cornerCenterY.w),
            cornerRadius.w - borderWidth / 2f,
            startAngle.w);

        float pos = relativePosition * circumference;
        Vector2 coordinates = Vector2.zero;
        coordinates += (pos >= 0f                 & pos < edgeCumLengths.x)    ? leftCoordinates :           Vector2.zero;
        coordinates += (pos >= edgeCumLengths.x   & pos < cornerCumLengths.x)  ? leftBottomCoordinates :     Vector2.zero;
        coordinates += (pos >= cornerCumLengths.x & pos < edgeCumLengths.z)    ? bottomCoordinates :         Vector2.zero;
        coordinates += (pos >= edgeCumLengths.z   & pos < cornerCumLengths.z)  ? rightBottomCoordinates :    Vector2.zero;
        coordinates += (pos >= cornerCumLengths.z & pos < edgeCumLengths.y)    ? rightCoordinates :          Vector2.zero;
        coordinates += (pos >= edgeCumLengths.y   & pos < cornerCumLengths.w)  ? rightTopCoordinates :       Vector2.zero;
        coordinates += (pos >= cornerCumLengths.w & pos < edgeCumLengths.w)    ? topCoordinates :            Vector2.zero;
        coordinates += (pos >= edgeCumLengths.w   & pos <= cornerCumLengths.y) ? leftTopCoordinates :        Vector2.zero;

        return coordinates;

    }

    Vector2 RingCoordinates(
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

    Vector4 Add4(
        Vector4 a, 
        Vector4 b)
    {
        return new Vector4(
            a.x + b.x,
            a.y + b.y,
            a.z + b.z,
            a.w + b.w
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
        float segmentLength,
        float segmentStart
    )
    {   
        return Mathf.Clamp01(
            (position - segmentStart) 
            / segmentLength);
    }

    float RampUp(float value)
    {
        if (value > 1f | value < 0f) {
            value = ((value % 1f) + 1f) % 1f;
            value = IsClose(value, 0f) ? 1f : value;
        }
        return Mathf.Clamp01(value);
    }

    bool IsClose(float a, float b, float tolerance = 0.0001f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }

}

