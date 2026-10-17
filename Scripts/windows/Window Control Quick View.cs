using UnityEngine;


public class WindowControlQuickView: WindowControl
{

    public const string TYPE = "quickview";

    [Tooltip("How far in front of the user quick view brings the window, in metres")]
    public float distance = 1f;

    // constructor
    public WindowControlQuickView()
    {
        base.type = TYPE;
    }

    public override void Start()
    {
        base.Start();
    }

    // A click (Ended) brings the window in front of the user and the next one takes it back;
    // the window keeps what to restore
    public override void OnPointer(in PointerEvent e, Window window)
    {
        if (e.Phase != PointerPhase.Ended)
        {
            return;
        }

        if (window.inQuickView)
        {
            window.ExitQuickView();
        }
        else
        {
            window.EnterQuickView(distance);
        }
    }
}