using UnityEngine;

public class FrameFraction : MonoBehaviour
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
        FrameFractionUtils.UpdateShader(
            GetComponent<Renderer>().material,

            transform.localScale, 
            padding, 
            cornerRadius, 
            borderWidth,
            relativeStart, 
            relativeEnd, 
            relativeOffset
        );
    }

}

