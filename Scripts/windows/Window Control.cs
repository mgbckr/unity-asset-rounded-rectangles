using UnityEngine;


public class WindowControl: MonoBehaviour, IPointerListener
{
    public string type = null;
    private Window window;
    // This control's own copy of its material, which renderer.material makes on the first
    // SetHoverLock. Unity does not destroy such copies with their renderer, so OnDestroy does.
    private Material hoverLockMaterial;

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

    // Keeps the control highlighted while a drag holds it (_Hover_Lock)
    protected void SetHoverLock(bool locked)
    {
        if (hoverLockMaterial == null)
        {
            hoverLockMaterial = GetComponent<Renderer>().material;
        }
        hoverLockMaterial.SetFloat("_Hover_Lock", locked ? 1f : 0f);
    }

    protected virtual void OnDestroy()
    {
        if (hoverLockMaterial != null)
        {
            Destroy(hoverLockMaterial);
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