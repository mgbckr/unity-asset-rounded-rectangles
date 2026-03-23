using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;


public class WindowControlQuickView: WindowControl
{

    public const string TYPE = "quickview";

    private bool quickView = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector2 originalScale;
    public float distance = 1f;

    // constructor
    public WindowControlQuickView()
    {
        base.type = TYPE;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void OnEvent(Touch touch, SpatialPointerState touchData, Window window)
    {
        if (touch.phase == TouchPhase.Began)
        {
        }
        else if (touch.phase == TouchPhase.Moved)
        {
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            if (quickView)
            {
                // Reset to original position and rotation
                window.transform.position = originalPosition;
                window.transform.rotation = originalRotation;
                window.SetScale(originalScale.x, originalScale.y);
                quickView = false;
            }
            else
            {
                if (window.cameraTransform != null)
                {
                    originalPosition = window.transform.position;
                    originalRotation = window.transform.rotation;
                    originalScale = window.GetScale();
                    window.transform.position = window.cameraTransform.position + window.cameraTransform.forward * distance;
                    window.transform.LookAt(window.cameraTransform.position);
                    window.transform.Rotate(0, 180, 0); // Adjust rotation to face the camera
                    quickView = true;
                }
                else
                {
                    Debug.LogWarning("WindowControlQuickView: No camera found to align to.");
                }
            }


        }
    }
}