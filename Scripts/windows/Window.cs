using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Networking; // Required for UnityWebRequest and DownloadHandlerTexture


public class Window : MonoBehaviour
{

    public float minimumWidth = 0.25f;
    public float minScaleFactor = 1.0f;
    public float closingDelay = 3f;

    // Hidden until opening a window's content is implemented
    public bool showOpenControl = false;

    private float closingTime = -1f;

    private Material frameMaterialContentEmpty;
    public Material frameMaterialContent;
    public Material frameMaterialError;
    public Material frameMaterialLoading;

    public Transform cameraTransform;

    private bool followCamera = false;

    public TextMeshPro title;
    private GameObject frame;
    private GameObject controls;
    private GameObject controls_move;
    private GameObject controls_left;
    private GameObject controls_right;
    private GameObject controls_close;
    private GameObject controls_open;
    // The controls ScaleControlsWithDistance sizes: move, left, right, close, open
    private Transform[] distanceScaledControls;

    private GameObject closingDialog;
    private GameObject closingDialog_timer;

    private Renderer frameRenderer;

    private float lastDistanceScaleFactor = 1f;

    private bool hasContent = false;

    public Texture2D currentContentTexture;

    // Where the layout wants the window, local to its parent. The window glides there when
    // the layout changes, unless the user moved it or it is in quick view.
    private Vector3? layoutPosition;
    public float layoutGlideSpeed = 8f;
    [NonSerialized] public bool movedByUser = false;
    // Set by EnterQuickView and ExitQuickView
    [NonSerialized] public bool inQuickView = false;

    // What ExitQuickView restores. Position and rotation are local to the parent, the tab
    // group, so a group moved during quick view (grab bar, recentering) takes them along.
    private Vector3 quickViewReturnPosition;
    private Quaternion quickViewReturnRotation;
    private Vector2 quickViewReturnScale;
    private bool quickViewReturnMovedByUser;
    private readonly Vector3[] quickViewReturnControlScales = new Vector3[5];
    private float quickViewReturnDistanceScaleFactor;

    public bool HasContent => hasContent;
    public bool IsClosing => closingTime > 0;

    public Action onClosed;
    public Action<WindowControl> onControlClicked;

    public enum Control
    {
        Move,
        ResizeLeft,
        ResizeRight,
        Close,
        Open
    }

    public enum WindowState
    {
        Loading,
        Error,
        Content,
        Title,
        Closing
    }

    void Start()
    {
    }

    void Awake()
    {
        // Doing this here in Awake rather than Start because Awake is called
        // when a Prefab is instantiated. Start is not.

        // Find the camera transform in the scene
        Camera cam = Camera.main;
        if (cam == null)
        {
            cam = FindFirstObjectByType<Camera>();
        }
        cameraTransform = cam != null ? cam.transform : null;
        if (cameraTransform == null)
        {
            Debug.LogWarning("No camera found in the scene. Please ensure there is a Camera. Control scaling will be disabled.");
        }

        // Find the frame GameObject in the children of this GameObject
        frame = transform.Find("Frame").gameObject;

        // Find the title TextMeshPro component in the children of this GameObject
        title = transform.Find("Title").Find("Content").GetComponent<TextMeshPro>();
        title.text = "";
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);

        // Find the controls GameObject iin the children of this GameObject
        controls = transform.Find("Controls").gameObject;
        controls_move = controls.transform.Find("Move").gameObject;
        controls_left = controls.transform.Find("Resize Left").gameObject;
        controls_right = controls.transform.Find("Resize Right").gameObject;
        controls_close = controls.transform.Find("Close").gameObject;
        controls_open = controls.transform.Find("Open").gameObject;
        controls_open.SetActive(showOpenControl);
        distanceScaledControls = new[]
        {
            controls_move.transform, controls_left.transform, controls_right.transform,
            controls_close.transform, controls_open.transform,
        };

        closingDialog = transform.Find("Closing Dialog").gameObject;
        closingDialog_timer = closingDialog.transform.Find("Timer").gameObject;
        closingDialog.SetActive(false);

        // Get the Renderer component of the frame
        frameRenderer = frame.GetComponent<Renderer>();

        // save original material
        frameMaterialContentEmpty = frameRenderer.material;
        // Copy content material
        frameMaterialContent = new Material(frameMaterialContent);

        // Set window state to loading
        SetWindowState(WindowState.Loading);
        ChangeScale(0f);
    }

    void Update()
    {

        UpdateControls();
        GlideToLayoutPosition();

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

        if (closingTime > 0)
        {
            // Check if the closing time has passed
            if (Time.time >= closingTime)
            {
                // Close the window
                Debug.Log("Closing window after delay.");
                onClosed?.Invoke();
                Destroy(gameObject);
            }
            else
            {
                // Update the timer text
                float remainingTime = closingTime - Time.time;
                closingDialog_timer.GetComponent<TextMeshPro>().text = remainingTime.ToString("F0");
            }
        }
    }

    public void SetTitle(string newTitle)
    {
        title.text = newTitle;
    }

    // URP sorts transparent renderers by the distance to the centre of their bounds, and the
    // title shares the transparent queue with the frame. The text's centre lies off the frame's
    // centre in the frame's plane, so from most viewpoints, straight ahead included, the frame
    // sorted nearer and drew over the title. The title's renderers, with TextMeshPro's
    // sub-meshes for fallback-font glyphs, get the frame's area as their bounds, centred just in
    // front of the frame: the title draws after its own frame from every side, and windows still
    // sort among themselves by distance. A render queue above the frame's would also let the
    // titles of farther windows draw over nearer ones. visionOS sorts with its sorting groups.
    private const float TitleSortOffset = 0.001f;

    private readonly List<Renderer> titleRenderers = new List<Renderer>();

    private void UpdateTitleSortBounds()
    {
        if (title == null || frameRenderer == null)
        {
            return;
        }

        Transform titleTransform = title.transform;
        Bounds frameBounds = frameRenderer.localBounds;
        Vector3 centre = titleTransform.InverseTransformPoint(
            frame.transform.TransformPoint(frameBounds.center) - transform.forward * TitleSortOffset);
        Vector3 corner = titleTransform.InverseTransformPoint(
            frame.transform.TransformPoint(frameBounds.center + frameBounds.extents));
        var bounds = new Bounds(centre, new Vector3(
            2f * Mathf.Abs(corner.x - centre.x),
            2f * Mathf.Abs(corner.y - centre.y),
            2f * Mathf.Abs(centre.z)));

        // Sub-meshes are children of the text, in its local space
        title.GetComponentsInChildren(true, titleRenderers);
        foreach (Renderer titleRenderer in titleRenderers)
        {
            titleRenderer.localBounds = bounds;
        }
    }

    // After every mesh rebuild of the title, which can add sub-meshes
    private void OnTextChanged(UnityEngine.Object changed)
    {
        if (changed == title)
        {
            UpdateTitleSortBounds();
        }
    }

    public void SetLayoutPosition(Vector3 localPosition)
    {
        layoutPosition = localPosition;
    }

    private void GlideToLayoutPosition()
    {
        if (layoutPosition == null || movedByUser || inQuickView)
        {
            return;
        }

        Vector3 target = layoutPosition.Value;
        if ((transform.localPosition - target).sqrMagnitude < 1e-8f)
        {
            return;
        }

        // Frame-rate independent easing; snaps once it is within a tenth of a millimeter
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, target, 1f - Mathf.Exp(-layoutGlideSpeed * Time.deltaTime));
        if ((transform.localPosition - target).sqrMagnitude < 1e-8f)
        {
            transform.localPosition = target;
        }
    }

    public void FollowCamera(bool track)
    {
        followCamera = track;
    }

    // Brings the window to distance in front of the user, facing them, until ExitQuickView
    public void EnterQuickView(float distance)
    {
        if (inQuickView)
        {
            return;
        }
        if (cameraTransform == null)
        {
            Debug.LogWarning("Window: no camera found to bring the window in front of.");
            return;
        }

        quickViewReturnPosition = transform.localPosition;
        quickViewReturnRotation = transform.localRotation;
        quickViewReturnScale = GetScale();
        quickViewReturnMovedByUser = movedByUser;
        for (int i = 0; i < distanceScaledControls.Length; i++)
        {
            quickViewReturnControlScales[i] = distanceScaledControls[i].localScale;
        }
        quickViewReturnDistanceScaleFactor = lastDistanceScaleFactor;

        transform.position = cameraTransform.position + cameraTransform.forward * distance;
        transform.LookAt(cameraTransform.position);
        transform.Rotate(0, 180, 0); // Adjust rotation to face the camera
        inQuickView = true;

        // The controls take their size for the new distance now, not a frame later
        UpdateControls();
    }

    // Takes the window back, relative to its parent: one the layout places to its place in the
    // current layout, one the user had moved (or one outside a layout) to where it was. The
    // group may have moved in the meantime; the window goes along. Size, controls and
    // movedByUser are as before quick view, so a move during quick view does not detach the
    // window from the layout.
    public void ExitQuickView()
    {
        if (!inQuickView)
        {
            return;
        }

        inQuickView = false;
        movedByUser = quickViewReturnMovedByUser;
        transform.localPosition = !movedByUser && layoutPosition != null
            ? layoutPosition.Value
            : quickViewReturnPosition;
        transform.localRotation = quickViewReturnRotation;

        // The controls as they were before quick view: scaling them back up from their
        // quick-view size can fail for good (see ScaleControlsWithDistance)
        for (int i = 0; i < distanceScaledControls.Length; i++)
        {
            distanceScaledControls[i].localScale = quickViewReturnControlScales[i];
        }
        lastDistanceScaleFactor = quickViewReturnDistanceScaleFactor;
        SetScale(quickViewReturnScale.x, quickViewReturnScale.y);
        // Now, not a frame later, and for where the window is now
        UpdateControls();
    }

    public void StartClosing()
    {
        Debug.Log("Closing window...");
        // Start the closing dialog
        SetWindowState(WindowState.Closing);
        closingDialog_timer.GetComponent<TextMeshPro>().text = closingDelay.ToString("F0");

        closingTime = Time.time + closingDelay;
    }

    public void StopClosing()
    {
        Debug.Log("Stopping closing window...");
        // Stop the closing dialog
        SetWindowState(WindowState.Content);
        closingTime = -1f;
    }


    public void SetScale(float x, float y)
    {
        Vector2 currentScale = new Vector2(
            frame.transform.localScale.x,
            frame.transform.localScale.y
        );
        Vector2 newScale = new Vector2(x, y);
        Debug.Log($"Current scale: {currentScale}");
        Debug.Log($"New scale: {newScale}");

        var diff = newScale - currentScale;
        ChangeScale(diff.x, diff.y);
    }

    public Vector2 GetScale()
    {
        return new Vector2(
            frame.transform.localScale.x,
            frame.transform.localScale.y
        );
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

        Debug.Log($"Changing scale by deltaX: {deltaX}, deltaY: {deltaY}");

        frame.transform.localScale += new Vector3(deltaX, deltaY, 0f);
        Vector3 currentScale = frame.transform.localScale;
        if (frame.transform.localScale.x < minimumWidth)
        {
            frame.transform.localScale = new Vector3(minimumWidth, currentScale.y, currentScale.z);
        }
        if (frame.transform.localScale.y < 0.1f)
        {
            frame.transform.localScale = new Vector3(currentScale.x, 0.1f, currentScale.z);
        }
        title.transform.localScale = new Vector3(
            frame.transform.localScale.x * 0.8f,
            frame.transform.localScale.y * 0.8f,
            1f
        );

        float minDim = Mathf.Min(frame.transform.localScale.x, frame.transform.localScale.y) * 0.8f;
        closingDialog.transform.localScale = new Vector3(minDim, minDim, 1f);
        UpdateTitleSortBounds();
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

        // Mirrors the close control on the other side of the move control
        controls_open.transform.localPosition = new Vector3(
            +(controls_move.transform.localScale.x / 2 + controls_open.transform.localScale.x / 2 * 1.1f),
            -controls_open.transform.localScale.y / 2,
            0
        );

        controls_left.transform.localPosition = new Vector3(
            -(
                frame.transform.localScale.x / 2
                - controls_left.transform.localScale.x / 2
                + controls_left.transform.localScale.y / 2),
            0,
            0
        );

        controls_right.transform.localPosition = new Vector3(
            +(
                frame.transform.localScale.x / 2
                - controls_right.transform.localScale.x / 2
                + controls_right.transform.localScale.y / 2),
            0,
            0
        );
    }


    // Sizes the controls with the distance to the camera, relative to their size from the last
    // call. A step that would make the move and close controls wider than the frame is not
    // taken at all, so after a jump in distance (e.g. a window created far away) the controls
    // keep their size until the window comes closer; ExitQuickView therefore restores them
    // instead of scaling them back up from their quick-view size.
    private void ScaleControlsWithDistance()
    {
        if (cameraTransform == null) return;

        // Calculate the distance from the camera
        float distance = Vector3.Distance(frame.transform.position, cameraTransform.position);

        // The scale factor is the distance, but at least 1: closer than a metre the controls
        // have their base size, also right after a jump closer (quick view brings a window to
        // a metre, which rounding can put just under it)
        float scaleFactor = Mathf.Max(1f, distance);
        float effectiveScaleFactor = scaleFactor / lastDistanceScaleFactor;
        Vector3 scaleVector = new Vector3(effectiveScaleFactor, effectiveScaleFactor, 1f);

        // Apply the scale factor to the controls
        Vector3 moveScale = Vector3.Scale(controls_move.transform.localScale, scaleVector);
        Vector3 leftScale = Vector3.Scale(controls_left.transform.localScale, scaleVector);
        Vector3 rightScale = Vector3.Scale(controls_right.transform.localScale, scaleVector);
        Vector3 closeScale = Vector3.Scale(controls_close.transform.localScale, scaleVector);
        Vector3 openScale = Vector3.Scale(controls_open.transform.localScale, scaleVector);

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

        // Update the scale of the controls
        controls_move.transform.localScale = moveScale;
        controls_left.transform.localScale = leftScale;
        controls_right.transform.localScale = rightScale;
        controls_close.transform.localScale = closeScale;
        controls_open.transform.localScale = openScale;

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
        controls_open.SetActive(showOpenControl && control == Control.Open);

        // If no control is specified, activate all controls
        if (control == null)
        {
            controls_move.SetActive(true);
            controls_left.SetActive(true);
            controls_right.SetActive(true);
            controls_close.SetActive(true);
            controls_open.SetActive(showOpenControl);
        }
    }

    public void SetWindowState(WindowState state)
    {
        if (frameRenderer == null)
        {
            frameRenderer = frame.GetComponent<Renderer>();
        }

        title.gameObject.SetActive(false);
        closingDialog.SetActive(false);
        switch (state)
        {
            case WindowState.Loading:
                frameRenderer.material = frameMaterialLoading;
                break;
            case WindowState.Error:
                frameRenderer.material = frameMaterialError;
                break;
            case WindowState.Content:
                if (hasContent)
                {
                    frameRenderer.material = frameMaterialContent;
                }
                else
                {
                    frameRenderer.material = frameMaterialContentEmpty;
                    title.gameObject.SetActive(true);
                }
                break;
            case WindowState.Title:
                frameRenderer.material = frameMaterialContentEmpty;
                title.gameObject.SetActive(true);
                break;
            case WindowState.Closing:
                frameRenderer.material = frameMaterialContentEmpty;
                closingDialog.gameObject.SetActive(true);
                break;
        }
    }

    public void SetFrameContent(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null. Cannot set frame content.");
            return;
        }

        // automatically set new size ratio to avoid distortion
        float ratio = texture.height / (float)texture.width;
        Debug.Log($"Setting frame content, ratio: {ratio}={texture.height}/{texture.width}");
        frame.transform.localScale = new Vector3(
            frame.transform.localScale.x,
            frame.transform.localScale.x * ratio,
            frame.transform.localScale.z
        );

        // set texture 
        frameMaterialContent.SetTexture("_Texture", texture);
        frameRenderer.material = frameMaterialContent;

        currentContentTexture = texture;
        hasContent = true; // Mark that the window has content
        // A new screenshot during the close countdown keeps the closing dialog up
        if (!IsClosing)
        {
            SetWindowState(WindowState.Content);
        }
        ChangeScale(0f);
    }

    public void OnControlClicked(WindowControl control)
    {
        Debug.Log($"Control clicked: {control.type}");
        onControlClicked?.Invoke(control);
    }

    // The window's own material copies (see Awake) aren't destroyed with it. The content
    // texture belongs to whoever set it.
    void OnDestroy()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        if (frameMaterialContent != null)
        {
            Destroy(frameMaterialContent);
        }
        if (frameMaterialContentEmpty != null)
        {
            Destroy(frameMaterialContentEmpty);
        }
    }

    // public void SetFrameContentFromUrl(string url)
    // {
    //     SetWindowState(WindowState.Loading);

    //     // handle data URLs
    //     if (url.StartsWith("data:image/"))
    //     {
    //         StartCoroutine(SetFrameContentFromDataUrl(url));
    //     }
    //     else
    //     {
    //         StartCoroutine(DownloadAndSetFrameTextureFromUrl(url));
    //     }
    // }

    // IEnumerator SetFrameContentFromDataUrl(string dataUrl)
    // {
    //     // Extract base64 data from the URL
    //     string base64Data = dataUrl.Substring(dataUrl.IndexOf(",") + 1);
    //     byte[] imageData = System.Convert.FromBase64String(base64Data);
    //     Texture2D texture = new Texture2D(2, 2);
    //     texture.LoadImage(imageData);

    //     // automatically set new size ratio to avoid distortion
    //     float ratio = texture.height / (float)texture.width;
    //     Debug.Log($"Setting frame content from data URL, ratio: {ratio}={texture.height}/{texture.width}");
    //     frame.transform.localScale = new Vector3(
    //         frame.transform.localScale.x,
    //         frame.transform.localScale.x * ratio,
    //         frame.transform.localScale.z
    //     );

    //     // set texture
    //     frameMaterialContent.SetTexture("_Texture", texture);
    //     frameRenderer.material = frameMaterialContent;

    //     hasContent = true; // Mark that the window has content
    //     SetWindowState(WindowState.Content);
    //     ChangeScale(0f);

    //     yield return null; // Wait for the end of the frame to ensure the texture is applied
    // }

    // IEnumerator DownloadAndSetFrameTextureFromUrl(string url)
    // {
    //     using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
    //     {
    //         yield return uwr.SendWebRequest();

    //         if (uwr.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.LogError("Image download failed: " + uwr.error);
    //             Debug.LogError("Response: " + uwr.downloadHandler.text); // helpful if it's HTML error
    //             SetWindowState(WindowState.Error);
    //         }
    //         else
    //         {
    //             Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

    //             // automatically set new size ratio to avoid distortion
    //             float ratio = texture.height / (float)texture.width;
    //             Debug.Log($"Setting frame content from URL: {url}, ratio: {ratio}={texture.height}/{texture.width}");
    //             frame.transform.localScale = new Vector3(
    //                 frame.transform.localScale.x,
    //                 frame.transform.localScale.x * ratio,
    //                 frame.transform.localScale.z
    //             );

    //             // set texture
    //             frameMaterialContent.SetTexture("_Texture", texture);
    //             frameRenderer.material = frameMaterialContent;

    //             hasContent = true; // Mark that the window has content
    //             SetWindowState(WindowState.Content);
    //             ChangeScale(0f);

    //         }
    //     }
    // }

}
