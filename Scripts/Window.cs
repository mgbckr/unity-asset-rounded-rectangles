using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Networking; // Required for UnityWebRequest and DownloadHandlerTexture


public class Window : MonoBehaviour
{

    public float minimumWidth = 0.25f;
    public float minScaleFactor = 1.0f;

    public Material frameMaterialContent;
    public Material frameMaterialError;
    public Material frameMaterialLoading;

    private Transform cameraTransform;

    private bool followCamera = false;

    private GameObject frame;
    private GameObject controls;
    private GameObject controls_move;
    private GameObject controls_left;
    private GameObject controls_right;
    private GameObject controls_close;

    private Renderer frameRenderer;

    private float lastDistanceScaleFactor = 1f;

    public enum Control
    {
        Move,
        ResizeLeft,
        ResizeRight,
        Close
    }

    public enum WindowState
    {
        Loading,
        Error,
        Content
    }

    void Start()
    {
    }

    void Awake()
    {
        // Doing this here in Awake rather than Start because Awake is called
        // when a Prefab is instantiated. Start is not.

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

        // Get the Renderer component of the frame
        frameRenderer = frame.GetComponent<Renderer>();

        // Copy content material
        frameMaterialContent = new Material(frameMaterialContent);

        // Set window state to loading
        SetWindowState(WindowState.Loading);
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
        }
    }

    public void FollowCamera(bool track)
    {
        followCamera = track;
    }

    public void ChangeScale(float delta)
    {
        Vector2 currentScale = new Vector2(
            frame.transform.localScale.x,
            frame.transform.localScale.y
        );
        currentScale /= currentScale.MaxComponent();

        float deltaX = delta * currentScale.x;
        float deltaY = delta * currentScale.y;
        ChangeScale(deltaX, deltaY);        
    }

    public void ChangeScale(float deltaX, float deltaY)
    {

        Vector3 currentScale = frame.transform.localScale;
        frame.transform.localScale += new Vector3(deltaX, deltaY, 0f);
        if (frame.transform.localScale.x < minimumWidth)
        {
            frame.transform.localScale = new Vector3(minimumWidth, currentScale.y, currentScale.z);
        }
        if (frame.transform.localScale.y < 0.1f)
        {
            frame.transform.localScale = new Vector3(currentScale.x, 0.1f, currentScale.z);
        }
        UpdateControls();
    }


    private void UpdateControls()
    {
        ScaleControlsWithDistance();
        RepositionControls();
    }

    
    private void RepositionControls()
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


    private void ScaleControlsWithDistance()
    {

        // Calculate the distance from the camera
        float distance = Vector3.Distance(frame.transform.position, cameraTransform.position);

        // Calculate the scale factor based on the distance
        float scaleFactor = distance;
        float effectiveScaleFactor = scaleFactor / lastDistanceScaleFactor;
        Vector3 scaleVector = new Vector3(effectiveScaleFactor, effectiveScaleFactor, 1f);

        // Apply the scale factor to the controls
        Vector3 moveScale = Vector3.Scale(controls_move.transform.localScale, scaleVector);
        Vector3 leftScale = Vector3.Scale(controls_left.transform.localScale, scaleVector);
        Vector3 rightScale = Vector3.Scale(controls_right.transform.localScale, scaleVector);
        Vector3 closeScale = Vector3.Scale(controls_close.transform.localScale, scaleVector);

        // Calculate scale of left/right resize 
        float resizeSize = frame.transform.localScale.x / 2 
            - (moveScale.x / 2 + closeScale.x * 1.1f)
            + moveScale.y;

        leftScale = new Vector3(resizeSize, leftScale.y, leftScale.z);
        rightScale = new Vector3(resizeSize, rightScale.y, rightScale.z);

        // Disable resize controls when controls get too big
        if (resizeSize < controls_move.transform.localScale.y * 2)
        {
            controls_left.SetActive(false);
            controls_right.SetActive(false);
        }
        else
        {
            controls_left.SetActive(true);
            controls_right.SetActive(true);
        }

        // Do not scale controls anymore if they exceed the frame size
        float centralControlSize = moveScale.x + 2 * closeScale.x * 1.1f;
        if (centralControlSize > frame.transform.localScale.x)
        {
            return;
        }

        // Stop scaling if the window gets to close
        if (scaleFactor < 1f)
        {
            return;
        }

        // Update the scale of the controls
        controls_move.transform.localScale = moveScale;
        controls_left.transform.localScale = leftScale;
        controls_right.transform.localScale = rightScale;
        controls_close.transform.localScale = closeScale;

        // Update the last distance scale factor
        lastDistanceScaleFactor = scaleFactor;

    }

    public void FocusControl(Control? control)
    {
        // Set the focus on the specified control
        controls_move.SetActive(control == Control.Move);
        controls_left.SetActive(control == Control.ResizeLeft);
        controls_right.SetActive(control == Control.ResizeRight);
        controls_close.SetActive(control == Control.Close);

        // If no control is specified, activate all controls
        if (control == null)
        {
            controls_move.SetActive(true);
            controls_left.SetActive(true);
            controls_right.SetActive(true);
            controls_close.SetActive(true);
        } 
    }

    public void SetWindowState(WindowState state)
    {
        if (frameRenderer == null)
        {
            frameRenderer = frame.GetComponent<Renderer>();
        }

        switch (state)
        {
            case WindowState.Loading:
                frameRenderer.material = frameMaterialLoading;
                break;
            case WindowState.Error:
                frameRenderer.material = frameMaterialError;
                break;
            case WindowState.Content:
                frameRenderer.material = frameMaterialContent;
                break;
        }
    }

    public void SetFrameContentFromUrl(string url)
    {
        SetWindowState(WindowState.Loading);
        StartCoroutine(DownloadAndSetFrameTextureFromUrl(url));
    }

    IEnumerator DownloadAndSetFrameTextureFromUrl(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Image download failed: " + uwr.error);
                SetWindowState(WindowState.Error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                // automatically set new size ratio to avoid distortion
                float ratio = texture.height / (float) texture.width;
                Debug.Log($"Setting frame content from URL: {url}, ratio: {ratio}={texture.height}/{texture.width}");
                frame.transform.localScale = new Vector3(
                    frame.transform.localScale.x,
                    frame.transform.localScale.x * ratio,
                    frame.transform.localScale.z
                );

                // set texture
                frameMaterialContent.SetTexture("_Texture", texture);
                frameRenderer.material = frameMaterialContent;
                SetWindowState(WindowState.Content);
            }
        }
    }

}
