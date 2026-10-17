// Delivers a pointer source's events to the listener on the target.
public static class PointerDispatch
{
    // Finds IPointerListener on e.Target only, as the old InputManager did; skips a missing
    // or destroyed Target
    public static void Send(in PointerEvent e)
    {
        // Unity's == is also true for a destroyed object
        if (e.Target == null)
        {
            return;
        }

        IPointerListener listener = e.Target.GetComponent<IPointerListener>();
        if (listener != null)
        {
            listener.OnPointer(e);
        }
    }
}
