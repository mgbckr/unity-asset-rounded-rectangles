// Reacts to pointer events. PointerDispatch looks for it on the event's Target only,
// so it sits on the object whose collider is hit.
public interface IPointerListener { void OnPointer(in PointerEvent e); }
