using UnityEngine;


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

    public override void OnPointer(in PointerEvent e, Window window)
    {
        if (e.Phase == PointerPhase.Began)
        {
        }
        else if (e.Phase == PointerPhase.Moved)
        {
        }
        else if (e.Phase == PointerPhase.Ended)
        {
            if (quickView)
            {
                // Reset to original position and rotation
                window.transform.position = originalPosition;
                window.transform.rotation = originalRotation;
                window.SetScale(originalScale.x, originalScale.y);
                quickView = false;
                // if the layout changed meanwhile, the window glides to its new place
                window.inQuickView = false;
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
                    window.inQuickView = true;
                }
                else
                {
                    Debug.LogWarning("WindowControlQuickView: No camera found to align to.");
                }
            }


        }
    }
}