using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public class WindowControlMove: WindowControl
{
    public float distanceScaling = 1f;
    private Renderer controlRenderer;
    private Vector3 lastPosition;

    public override void Start()
    {
        base.Start();
        controlRenderer = GetComponent<Renderer>();
    }

    public override void OnEvent(Touch touch, SpatialPointerState touchData, Window window)
    {
        if (touch.phase == TouchPhase.Began)
        {
            lastPosition = touchData.interactionPosition;
            window.FollowCamera(true);
            controlRenderer.material.SetInteger("_Hover_Lock", 1);
            window.FocusControl(Window.Control.Move);
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector3 deltaPosition = touchData.interactionPosition - lastPosition;
            
            // scale delta position based on distance
            // TODO: optimize ...
            // float distance = Vector3.Distance(window.transform.position, Camera.main.transform.position);
            // deltaPosition *= distance / distanceScaling;

            window.transform.position += deltaPosition;
            lastPosition = touchData.interactionPosition;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            window.FollowCamera(false);
            controlRenderer.material.SetInteger("_Hover_Lock", 0);
            window.FocusControl(null);
        }
    }
}