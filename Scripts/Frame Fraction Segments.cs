using Unity.VisualScripting;
using UnityEngine;

public class FrameFractionSegments : MonoBehaviour
{

    // left right bottom top
    public Vector4 padding = new Vector4(0f, 0f, 0f, 0f);
    // left-bottom, left-top, right-bottom, right-top
    public Vector4 cornerRadius = new Vector4(0f, 0f, 0f, 0f);
    public float borderWidth = 0.05f;

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

        FrameFractionUtils.UpdateShader(

            GetComponent<Renderer>().material,

            transform.localScale, 
            padding,
            cornerRadius,
            borderWidth,

            (int) segment,
            segmentPosition,
            startOffset,
            endOffset
        );
    }

}

