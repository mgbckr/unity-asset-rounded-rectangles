using UnityEngine;

public class Window : MonoBehaviour
{

    private GameObject frame;
    private GameObject controls;
    private GameObject controls_center;
    private GameObject controls_center_move;
    private GameObject controls_center_close;
    private GameObject controls_left;
    private GameObject controls_right;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the frame GameObject in the children of this GameObject
        frame = transform.Find("Frame").gameObject;

        // Find the controls GameObject iin the children of this GameObject
        controls = transform.Find("Controls").gameObject;
        controls_center = controls.transform.Find("Center").gameObject;
        controls_center_move = controls_center.transform.Find("Move").gameObject;
        controls_center_close = controls_center.transform.Find("Close").gameObject;
        controls_left = controls.transform.Find("Left").gameObject;
        controls_right = controls.transform.Find("Right").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
