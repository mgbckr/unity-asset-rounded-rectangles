using UnityEngine;


public class WindowControlCloseCancel: WindowControl
{

    public override void Start()
    {
        base.Start();
    }

    // Began only, like WindowControlClose
    public override void OnPointer(in PointerEvent e, Window window)
    {
        if (e.Phase == PointerPhase.Began)
        {
            window.StopClosing();
        }
    }
}