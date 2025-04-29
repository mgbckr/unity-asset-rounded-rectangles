using UnityEngine;

public class DualFrameWithRoundGap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // animate shader property "Segment_Position"
        float time = Time.time;
        float speed = 3f;
        float segmentPosition = Mathf.PingPong(time * speed, 1.0f);
        GetComponent<Renderer>().material.SetFloat("_Segment_Position", segmentPosition);
        
    }
}
