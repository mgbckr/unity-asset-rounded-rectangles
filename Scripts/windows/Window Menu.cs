using UnityEngine;

 
public class WindowMenu: MonoBehaviour
{
    private IPointerListener[] items;
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