using UnityEngine;


public class WindowControlClose: WindowControl
{

    public override void Start()
    {
        base.Start();
    }

    // Began only: a Moved arrives every frame and would restart the countdown, and
    // pinch-down responds before the ray can drift
    public override void OnPointer(in PointerEvent e, Window window)
    {
        if (e.Phase == PointerPhase.Began)
        {
            window.StartClosing();
        }
    }
}