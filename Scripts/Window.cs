using UnityEngine;

public class Window : MonoBehaviour
{

    private GameObject controls;
    private GameObject frame;

    // controls modes (enum): inactive, move, left, right, (close)
    // inactive: no controls are active
    // move: the window can be moved
    // left: the window can be resized from the left
    // right: the window can be resized from the right
    // close: the window can be closed
    public enum ControlsMode
    {
        Inactive,
        Move,
        Left,
        Right,
        Close
    }
    private ControlsMode controlsMode = ControlsMode.Inactive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the controls GameObject iin the children of this GameObject
        controls = transform.Find("Controls").gameObject;
        Debug.Log("Controls: " + controls);

        // Find the frame GameObject in the children of this GameObject
        frame = transform.Find("Frame").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Material material = controls.GetComponent<Renderer>().material;
        Color color = material.GetColor("_Color");

        float speed = 1f;
        float pulse = Mathf.PingPong(Time.time * speed, 1f);
        float sawtooth = (Time.time % (1 / speed)) / (1 / speed);

        // set segment position in material
        float segmentPosition = sawtooth;
        material.SetFloat("_Segment_Position", segmentPosition);
        material.SetColor("_Color", new Color(color.r, color.g, color.b, pulse));
        Debug.Log("Segment Position: " + segmentPosition);
    }
}
