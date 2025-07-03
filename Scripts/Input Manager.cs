using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public class InputManager : MonoBehaviour
{
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
                Debug.Log("InputManager Update: Phase " + touch.phase);
                SpatialPointerState touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                if (touchData.targetObject != null)
                {
                    Debug.Log("InputManager Update: Touch detected on " + touchData.targetObject.name + " with phase " + touch.phase);
                    SpatialPointerStateListener listener = touchData.targetObject.GetComponent<SpatialPointerStateListener>();
                    if (listener != null)
                    {
                        Debug.Log("InputManager Update: Notifying listener " + listener.GetType().Name);
                        // Notify the listener about the touch event
                        listener.OnEvent(touch, touchData);
                    }
                } 
            }
        }
    }
}
