using UnityEngine;


public class WindowControl: MonoBehaviour, IPointerListener
{
    public string type = null;
    private Window window;

    public virtual void OnPointer(in PointerEvent e, Window window) {}

    public virtual void Start()
    {
        // Find the Window component in the parent hierarchy
        window = FindParentByName(transform, "Window")?.GetComponent<Window>();
        if (window == null)
        {
            Debug.LogError("No Window component found in the parent hierarchy.");
        }
    }

    public void OnPointer(in PointerEvent e)
    {
        OnPointer(e, window);
        // A click is Began then Ended; Canceled never clicks
        if (e.Phase == PointerPhase.Ended && type != null && type.Length > 0)
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