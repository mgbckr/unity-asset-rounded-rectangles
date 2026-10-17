using UnityEngine;

// The camera that stands for the user's head.
//
// Camera.main only returns an enabled camera tagged MainCamera. PolySpatial disables the Unity
// camera on visionOS, where RealityKit renders, so Camera.main is null there. PolySpatial also
// adds hidden helper cameras (line renderer baking support) that sit at the origin, so the first
// camera found must not stand in for the head. Order: Camera.main, then the active object tagged
// MainCamera even with its camera disabled, then any camera for scenes without a tagged one.
public static class HeadCamera
{
    public static Camera Find()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject tagged = GameObject.FindWithTag("MainCamera");
            if (tagged != null)
            {
                cam = tagged.GetComponent<Camera>();
            }
        }
        if (cam == null)
        {
            cam = Object.FindFirstObjectByType<Camera>();
        }
        return cam;
    }
}
