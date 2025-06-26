using UnityEngine;

public class Window : MonoBehaviour
{

    private Transform cameraTransform;

    private bool lookAtCamera = false;

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
        // Check if the lookAtTarget is assigned
        if (cameraTransform != null)
        {
            // track target
            if (lookAtCamera)
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

    public void SetLookAtCamera(bool track)
    {
        lookAtCamera = track;
    }

    public bool GetLookAtCamera()
    {
        return lookAtCamera;
    }

    public void ChangeScale(float deltaScale)
    {

        transform.localScale += new Vector3(deltaScale, deltaScale, 0);

        // // Change the scale of the frame
        // Vector3 newScale = frame.transform.localScale + new Vector3(scaleDifference, scaleDifference, 0);
        // // Ensure the scale does not go below a certain threshold
        // newScale.x = Mathf.Max(newScale.x, 0.1f);
        // newScale.y = Mathf.Max(newScale.y, 0.1f);
        // frame.transform.localScale = newScale;

        // Update the controls' positions based on the new scale
        // TODO
        // if (controls != null)
        // {
        //     controls.transform.localPosition = new Vector3(newScale.x / 2 + 0.1f, newScale.y / 2 + 0.1f, 0);
        //     controls_move.transform.localPosition = new Vector3(0, newScale.y / 2 + 0.1f, 0);
        //     controls_left.transform.localPosition = new Vector3(-newScale.x / 2 - 0.1f, 0, 0);
        //     controls_right.transform.localPosition = new Vector3(newScale.x / 2 + 0.1f, 0, 0);
        //     controls_close.transform.localPosition = new Vector3(0, -newScale.y / 2 - 0.1f, 0);
        // }

    }

}
