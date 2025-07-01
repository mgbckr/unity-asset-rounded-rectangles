using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public class InputManager : MonoBehaviour
{
    public Transform lookAtTarget;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void Start()
    {
    }

    void Update()
    {
        // Debug.Log("InputManager Update: " + Touch.activeTouches.Count + " active touches");
        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                SpatialPointerState touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                if (touchData.targetObject != null)
                {
                    SpatialPointerStateListener listener = touchData.targetObject.GetComponent<SpatialPointerStateListener>();
                    if (listener != null)
                    {
                        // Notify the listener about the touch event
                        listener.OnEvent(touch, touchData);
                    }
                } 
            }
        }
    }
}
