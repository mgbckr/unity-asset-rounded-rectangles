using UnityEngine;

public class Window : MonoBehaviour
{

    private Transform cameraTransform;

    private bool followCamera = false;

    private GameObject frame;
    private GameObject controls;
    private GameObject controls_move;
    private GameObject controls_left;
    private GameObject controls_right;
    private GameObject controls_close;

    void Start()
    {

        // Find the camera transform in the scene
        cameraTransform = Camera.main != null ? Camera.main.transform : null;
        if (cameraTransform == null)
        {
            Debug.LogError("No camera found in the scene. Please ensure there is a Camera with the MainCamera tag.");
        }

        // Find the frame GameObject in the children of this GameObject
        frame = transform.Find("Frame").gameObject;

        // Find the controls GameObject iin the children of this GameObject
        controls = transform.Find("Controls").gameObject;
        controls_move = controls.transform.Find("Move").gameObject;
        controls_left = controls.transform.Find("Resize Left").gameObject;
        controls_right = controls.transform.Find("Resize Right").gameObject;
        controls_close = controls.transform.Find("Close").gameObject;
    }

    void Update()
    {
        // Update the controls position based on the frame size
        UpdateControls();

        // Check if the lookAtTarget is assigned
        if (cameraTransform != null)
        {
            // track target
            if (followCamera)
            {
                // Make the frame look at the lookAtTarget
                transform.LookAt(cameraTransform);
                // For some reason quads are facing opposite of the blue arrow, so we have to rotate it
                transform.Rotate(0, 180f, 0);
            }

            // resize controls
            float distance = Vector3.Distance(transform.position, cameraTransform.position);
        }
    }

    public void FollowCamera(bool track)
    {
        followCamera = track;
    }



    public void ChangeScale(float deltaX, float deltaY)
    {
        frame.transform.localScale = new Vector3(
            transform.localScale.x + deltaX,
            transform.localScale.y + deltaY,
            transform.localScale.z);
    }

    private void UpdateControls()
    {
        controls.transform.localPosition = new Vector3(
            0,
            -frame.transform.localScale.y / 2,
            0
        );

        controls_move.transform.localPosition = new Vector3(
            0,
            -controls_move.transform.localScale.y / 2,
            0
        );

        controls_close.transform.localPosition = new Vector3(
            -(controls_move.transform.localScale.x / 2 + controls_close.transform.localScale.x / 2 * 1.1f),
            -controls_close.transform.localScale.y / 2,
            0
        );

        controls_left.transform.localPosition = new Vector3(
            - (
                frame.transform.localScale.x / 2
                - controls_left.transform.localScale.x / 2
                + controls_left.transform.localScale.y / 2),
            0,
            0
        );
        
        controls_right.transform.localPosition = new Vector3(
            + (
                frame.transform.localScale.x / 2
                - controls_right.transform.localScale.x / 2
                + controls_right.transform.localScale.y / 2),
            0,
            0
        );
    }

}
