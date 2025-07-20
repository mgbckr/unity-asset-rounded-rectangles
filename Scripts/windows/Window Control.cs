using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;


public class WindowControl: MonoBehaviour, SpatialPointerStateListener
{
    public string type = null;
    private Window window;

    public virtual void OnEvent(Touch touch, SpatialPointerState touchData, Window window) {}

    public virtual void Start()
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
        if (touch.phase == TouchPhase.Ended && type != null && type.Length > 0)
        {
            window.OnControlClicked(this);
        }
    }

    public static Transform FindParentByName(Transform child, string parentName)
    {
        Transform current = child.parent;
        while (current != null)
        {
            if (current.name.StartsWith(parentName))
                return current;
            current = current.parent;
        }
        return null; // not found
    }

}