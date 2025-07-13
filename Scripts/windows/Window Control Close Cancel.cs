using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public class WindowControlCloseCancel: WindowControl
{

    public override void Start()
    {
        base.Start();
    }

    public override void OnEvent(Touch touch, SpatialPointerState touchData, Window window)
    {
        window.StopClosing();
    }
}