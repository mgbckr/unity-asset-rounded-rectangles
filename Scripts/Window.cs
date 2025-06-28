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

    private float lastDistanceScaleFactor = 1f;

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
        frame.transform.localScale += new Vector3(deltaX, deltaY, 0f);
        UpdateControls();
    }

    private void ScaleWithDistance()
    {

        // Calculate the distance from the camera
        float distance = Vector3.Distance(frame.transform.position, cameraTransform.position);

        // Calculate the scale factor based on the distance
        float scaleFactor = distance;
        float effectiveScaleFactor = scaleFactor / lastDistanceScaleFactor;
        Vector3 scaleVector = new Vector3(effectiveScaleFactor, effectiveScaleFactor, 1f);

        // Apply the scale factor to the controls
        controls_move.transform.localScale = Vector3.Scale(controls_move.transform.localScale, scaleVector);
        controls_left.transform.localScale = Vector3.Scale(controls_left.transform.localScale, scaleVector);
        controls_right.transform.localScale = Vector3.Scale(controls_right.transform.localScale, scaleVector);
        controls_close.transform.localScale = Vector3.Scale(controls_close.transform.localScale, scaleVector);

        // Update the last distance scale factor
        lastDistanceScaleFactor = scaleFactor;
    }


    private void UpdateControls()
    {

        // Scale controls with distance
        ScaleWithDistance();

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
