using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public class WindowControlQuickView: WindowControl
{

    private bool quickView = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector2 originalScale;
    public float distance = 1f;

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
                originalPosition = window.transform.position;
                originalRotation = window.transform.rotation;
                originalScale = window.GetScale();
                window.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
                window.transform.LookAt(Camera.main.transform.position);
                window.transform.Rotate(0, 180, 0); // Adjust rotation to face the camera
                quickView = true;
            }


        }
    }
}