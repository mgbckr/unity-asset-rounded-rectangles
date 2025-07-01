using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public abstract class WindowControl: MonoBehaviour, SpatialPointerStateListener
{
    private Window window;

    public abstract void OnEvent(Touch touch, SpatialPointerState touchData, Window window);

    public void Start()
    {
        // Find the Window component in the parent hierarchy
        window = FindParentByName(transform, "Window")?.GetComponent<Window>();
        if (window == null)
        {
            Debug.LogError("No Window component found in the parent hierarchy.");
        }
    }

    public void OnEvent(Touch touch, SpatialPointerState touchData)
    {
        OnEvent(touch, touchData, window);
    }

    public static Transform FindParentByName(Transform child, string parentName)
    {
        Transform current = child.parent;
        while (current != null)
        {
            if (current.name == parentName)
                return current;
            current = current.parent;
        }
        return null; // not found
    }

}