using Unity.VisualScripting;
using UnityEngine;

public class FrameFractionSegments : FrameFraction
{

    public float segment;
    public float segmentPosition;
    public float startOffset;
    public float endOffset;

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

        // ensure corner radius to be at least border width
        Vector4 adjustedCornerRadius = new Vector4(
            Mathf.Max(cornerRadius.x, borderWidth),
            Mathf.Max(cornerRadius.y, borderWidth),
            Mathf.Max(cornerRadius.z, borderWidth),
            Mathf.Max(cornerRadius.w, borderWidth)
        );

        FormatShape(transform.localScale, padding, adjustedCornerRadius, borderWidth);

        // prepare segment information
        Vector4[] segmentInfos = PrepareSegments(
            transform.localScale, padding, adjustedCornerRadius, borderWidth);
        Vector4 edgeLengths = segmentInfos[0];
        Vector4 cornerLengths = segmentInfos[1];
        Vector4 edgeCumLengths = segmentInfos[2];
        Vector4 cornerCumLengths = segmentInfos[3];
        
        float circumference = cornerCumLengths[1];

        // calculate relative start and end positions
        float startLength = 0f;
        startLength += segment == 0 ? 0f                    + segmentPosition * edgeLengths.x   : 0f;
        startLength += segment == 1 ? edgeCumLengths.x      + segmentPosition * cornerLengths.x : 0f;
        startLength += segment == 2 ? cornerCumLengths.x    + segmentPosition * edgeLengths.z   : 0f;
        startLength += segment == 3 ? edgeCumLengths.z      + segmentPosition * cornerLengths.z : 0f;
        startLength += segment == 4 ? cornerCumLengths.z    + segmentPosition * edgeLengths.y   : 0f;
        startLength += segment == 5 ? edgeCumLengths.y      + segmentPosition * cornerLengths.w : 0f;
        startLength += segment == 6 ? cornerCumLengths.w    + segmentPosition * edgeLengths.w   : 0f;
        startLength += segment == 7 ? edgeCumLengths.w      + segmentPosition * cornerLengths.y : 0f;

        float maxScale = Mathf.Max(
            transform.localScale.x,
            transform.localScale.y
        );
        FormatSegments(
            startOffset / circumference,
            endOffset / circumference,
            startLength / circumference, 
            circumference,

            padding,
            adjustedCornerRadius,
            borderWidth,

            edgeLengths,
            cornerLengths,
            edgeCumLengths,
            cornerCumLengths,
            transform.localScale
        );
    }

}

