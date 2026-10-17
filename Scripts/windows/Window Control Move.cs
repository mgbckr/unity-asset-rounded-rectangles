using UnityEngine;


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

    public override void OnPointer(in PointerEvent e, Window window)
    {
        if (e.Phase == PointerPhase.Began)
        {
            lastPosition = e.InteractionPosition;
            window.FollowCamera(true);
            controlRenderer.material.SetInteger("_Hover_Lock", 1);
            window.FocusControl(Window.Control.Move);
        }
        else if (e.Phase == PointerPhase.Moved)
        {
            Vector3 deltaPosition = e.InteractionPosition - lastPosition;
            
            // scale delta position based on distance
            // TODO: optimize ...
            // float distance = Vector3.Distance(window.transform.position, Camera.main.transform.position);
            // deltaPosition *= distance / distanceScaling;

            window.transform.position += deltaPosition;
            lastPosition = e.InteractionPosition;
            // the layout leaves the window where the user put it
            window.movedByUser = true;
        }
        else if (e.Phase == PointerPhase.Ended || e.Phase == PointerPhase.Canceled)
        {
            window.FollowCamera(false);
            controlRenderer.material.SetInteger("_Hover_Lock", 0);
            window.FocusControl(null);
        }
    }
}