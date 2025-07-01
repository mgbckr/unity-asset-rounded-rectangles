using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public class WindowControlResize: WindowControl
{

    private Renderer renderer;
    private Vector3 lastPosition;

    public void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public override void OnEvent(Touch touch, SpatialPointerState touchData, Window window)
    {
        if (touch.phase == TouchPhase.Began)
        {
            Debug.Log("Touch began on Resize");
            lastPosition = touchData.interactionPosition;
            renderer.material.SetInteger("_Hover_Lock", 1);
            window.FocusControl(
                name == "Resize Right" 
                ? Window.Control.ResizeRight 
                : Window.Control.ResizeLeft);
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector3 deltaPosition = touchData.interactionPosition - lastPosition;

            float deltaX = name == "Resize Right" ? deltaPosition.x : -deltaPosition.x;

            // // Resize only in the X and Y direction
            // window.ChangeScale(
            //     deltaX, 
            //     - deltaPosition.y);

            // Smooth scaling and account for direction
            float diffScale = 
                Mathf.Sign(deltaX - deltaPosition.y)
                * Mathf.Sqrt(
                    Mathf.Pow(deltaX, 2f) + Mathf.Pow(deltaPosition.y, 2f));
            window.ChangeScale(diffScale, diffScale);

            lastPosition = touchData.interactionPosition;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            renderer.material.SetInteger("_Hover_Lock", 0);
            window.FocusControl(null);
        }
    }
}