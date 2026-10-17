using UnityEngine;


public class WindowControlResize: WindowControl
{
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
            Debug.Log("Touch began on Resize");
            lastPosition = e.InteractionPosition;
            controlRenderer.material.SetInteger("_Hover_Lock", 1);
            window.FocusControl(
                name == "Resize Right" 
                ? Window.Control.ResizeRight 
                : Window.Control.ResizeLeft);
        }
        else if (e.Phase == PointerPhase.Moved)
        {
            Vector3 deltaPosition = e.InteractionPosition - lastPosition;

            float deltaX = name == "Resize Right" ? deltaPosition.x : -deltaPosition.x;

            // // Resize only in the X and Y direction
            // window.ChangeScale(
            //     deltaX, 
            //     - deltaPosition.y);

            // Smooth scaling and account for direction
            float diffScale = 
                Mathf.Sign(deltaX - deltaPosition.y)
                * Mathf.Sqrt(
                    Mathf.Pow(deltaX, 2f) + Mathf.Pow(deltaPosition.y, 2f));
            window.ChangeScale(diffScale);

            lastPosition = e.InteractionPosition;
        }
        else if (e.Phase == PointerPhase.Ended || e.Phase == PointerPhase.Canceled)
        {
            controlRenderer.material.SetInteger("_Hover_Lock", 0);
            window.FocusControl(null);
        }
    }
}