using UnityEngine;

// One pointer update. A platform's pointer source builds it and delivers it with
// PointerDispatch.Send; listeners implement IPointerListener.
//
// Contract:
// - The target is captured at Began and receives every later phase of the interaction,
//   even if the pointer leaves it.
// - A click is Began then Ended on the same target.
// - Canceled releases state (drags, locks, focus) but never clicks.
// - PositionFollowsRay is the same for every event of one interaction.
public struct PointerEvent
{
    public PointerPhase Phase;
    public GameObject Target;             // the object hit at Began; stays for the whole interaction
    public Vector3 InteractionPosition;   // world
    public Vector3 DevicePosition;        // hand pinch point or controller grip, world; never zero
    public Vector3 RayOrigin, RayDirection;
    public int PointerId;                 // 0 left, 1 right; visionOS: the touch id

    // True when the source moves InteractionPosition along its ray while the press is held
    // (the Quest's ray drags): the grabbed point stays under the ray, at a depth the source
    // already scales with the hand, so listeners move things by InteractionPosition directly
    // and apply no gain of their own to DevicePosition. False on visionOS and for the Quest's
    // hand-delta drags, where a listener that moves far things, like the grab bar, scales
    // DevicePosition's movement itself.
    public bool PositionFollowsRay;
}
