using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;

public class InputManager : MonoBehaviour
{
    public Transform lookAtTarget;

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
        Debug.Log("InputManager Update: " + Touch.activeTouches.Count + " active touches");
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
                            Debug.Log("Touch began on Move");
                            selectedObject = touchData.targetObject.transform.parent.parent.gameObject; // Assuming the object to move is three levels up in the hierarchy
                            Debug.Log("Selected object: " + selectedObject.name);
                            lastPosition = touchData.interactionPosition;

                            // Set the lookAtTarget to the selected object
                            selectedObject.GetComponent<Window>().FollowCamera(true);
                        }
                        else if (touch.phase == TouchPhase.Moved && selectedObject != null)
                        {
                            Vector3 deltaPosition = touchData.interactionPosition - lastPosition;
                            selectedObject.transform.position += deltaPosition;
                            lastPosition = touchData.interactionPosition;
                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            selectedObject.GetComponent<Window>().FollowCamera(false);
                            selectedObject = null; // Deselect the object when the touch ends
                        }
                    }

                    // RESIZE
                    else if (touchData.targetObject.name == "Resize Right" || touchData.targetObject.name == "Resize Left")
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            Debug.Log("Touch began on Resize");
                            selectedObject = touchData.targetObject.transform.parent.parent.gameObject;
                            lastPosition = touchData.interactionPosition;
                        }
                        else if (touch.phase == TouchPhase.Moved && selectedObject != null)
                        {
                            Vector3 deltaPosition = touchData.interactionPosition - lastPosition;

                            // Resize only in the X and Y direction
                            selectedObject.GetComponent<Window>().ChangeScale(deltaPosition.x, deltaPosition.y);

                            lastPosition = touchData.interactionPosition;
                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            selectedObject = null; // Deselect the object when the touch ends
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
