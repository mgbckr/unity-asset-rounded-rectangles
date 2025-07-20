using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;

 
public class WindowMenu: MonoBehaviour
{
    private SpatialPointerStateListener[] items;
    private Window window;

    public virtual void Start()
    {
        // Find the Window component in the parent hierarchy
        window = WindowControl.FindParentByName(transform, "Window")?.GetComponent<Window>();
        if (window == null)
        {
            Debug.LogError("No Window component found in the parent hierarchy.");
        }
    }
}