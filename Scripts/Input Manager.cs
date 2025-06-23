using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;

public class InputManager : MonoBehaviour
{

    private GameObject selectedObject;
    private Vector3 lastPosition;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                SpatialPointerState touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                if (touchData.targetObject != null && touchData.Kind != SpatialPointerKind.Touch)
                {
                    Debug.Log("Touch interaction with: " + touchData.targetObject.name);

                    // MOVE
                    if (touchData.targetObject.name == "Move")
                    {

                        if (touch.phase == TouchPhase.Began)
                        {
                            selectedObject = touchData.targetObject.transform.parent.parent.parent.gameObject; // Assuming the object to move is three levels up in the hierarchy
                            lastPosition = touchData.interactionPosition;
                        }
                        else if (touch.phase == TouchPhase.Moved && selectedObject != null)
                        {
                            Vector3 deltaPosition = touchData.interactionPosition - lastPosition;
                            selectedObject.transform.position += deltaPosition;
                            lastPosition = touchData.interactionPosition;
                        }
                    }

                    // RESIZE
                    else if (touchData.targetObject.name == "Resize Right" || touchData.targetObject.name == "Resize Left")
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            selectedObject = touchData.targetObject.transform.parent.parent.gameObject;
                            lastPosition = touchData.interactionPosition;
                        }
                        else if (touch.phase == TouchPhase.Moved && selectedObject != null)
                        {
                            Vector3 deltaPosition = touchData.interactionPosition - lastPosition;
                            selectedObject.transform.localScale += new Vector3(deltaPosition.x, deltaPosition.y, 0); // Resize only in the X and Y direction

                            // // We need tp update the position as well
                            // if (touchData.targetObject.name == "Resize Left")
                            // {
                            //     selectedObject.transform.position += new Vector3(
                            //         - deltaPosition.x / 2,
                            //         - deltaPosition.y / 2,
                            //         0);
                            // }
                            // else if (touchData.targetObject.name == "Resize Right")
                            // {
                            //     selectedObject.transform.position += new Vector3(
                            //         + deltaPosition.x / 2,
                            //         - deltaPosition.y / 2,
                            //         0);
                            // }

                            lastPosition = touchData.interactionPosition;
                        }
                    }

                }
            }
        }
        if (Touch.activeTouches.Count == 0 && selectedObject != null)
        {
            selectedObject = null; // Deselect the object when no touches are active
        }
    }
}
