using UnityEngine;


public class WindowControlMove: WindowControl
{
    public float distanceScaling = 1f;
    private Vector3 lastPosition;

    public override void Start()
    {
        base.Start();
    }

    public override void OnPointer(in PointerEvent e, Window window)
    {
        if (e.Phase == PointerPhase.Began)
        {
            lastPosition = e.InteractionPosition;
            window.FollowCamera(true);
            SetHoverLock(true);
            window.FocusControl(Window.Control.Move);
        }
        else if (e.Phase == PointerPhase.Moved)
        {
            Vector3 deltaPosition = e.InteractionPosition - lastPosition;
            
            // scale delta position based on distance
            // TODO: optimize ...
            // float distance = Vector3.Distance(window.transform.position, Camera.main.transform.position);
            // deltaPosition *= distance / distanceScaling;

            // A pinch held still neither moves the window nor counts as moved by the user
            if (deltaPosition.sqrMagnitude > 0f)
            {
                window.transform.position += deltaPosition;
                // the layout leaves the window where the user put it
                window.movedByUser = true;
            }
            lastPosition = e.InteractionPosition;
        }
        else if (e.Phase == PointerPhase.Ended || e.Phase == PointerPhase.Canceled)
        {
            window.FollowCamera(false);
            SetHoverLock(false);
            window.FocusControl(null);
        }
    }
}