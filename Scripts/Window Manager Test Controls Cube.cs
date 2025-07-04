using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class WindowManagerTestControlsCube : MonoBehaviour, SpatialPointerStateListener
{

    private float rotationSinceLastCounterIncrease = 0f;
    private float lastRotation = 0f;
    private int counter = 0;
    private float rotationThreshold = 10f; // Adjust this value to set the sensitivity of the counter increment
    public WindowManager windowManager;
    public TextMeshPro counterText;


    public void Start()
    {
    }

    public void OnEvent(Touch touch, SpatialPointerState touchData)
    {
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            // Reset the rotation when a new touch begins
            lastRotation = 0f;
            counter = 0;
            rotationSinceLastCounterIncrease = 0f; // Reset the counter increment
            UpdateCounterText();
            windowManager.ClearWindows(); // Clear existing windows when a new touch begins
        }
        else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            // Optionally, you can handle the end of the touch here
            Debug.Log("Touch ended on " + gameObject.name);
            for (int i = 0; i < counter; i++)
            {
                Debug.Log("Counter: " + i);
                windowManager.InstantiateWindow(
                    // null,
                    "https://picsum.photos/800/600?random=" + i,
                    "Test Window " + i,
                    transform.position + new Vector3(0.41f * i, 0, 0));
            }
        }
        else
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            // Calculate the rotation based on the touch movement
            float deltaRotation = touch.delta.x * 0.1f; // Adjust sensitivity as needed
            lastRotation += deltaRotation * 5;

            // Apply the rotation to the GameObject
            transform.rotation *= Quaternion.Euler(0, lastRotation, 0);

            // Increment the counter and update the text depending on rotation
            rotationSinceLastCounterIncrease += Mathf.Abs(deltaRotation);
            if (rotationSinceLastCounterIncrease >= rotationThreshold)
            {
                counter++;
                rotationSinceLastCounterIncrease = 0f; // Reset the counter increment
                UpdateCounterText();
            }
        }
    }

    private void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = "Counter: " + counter;
        }
        else
        {
            Debug.LogWarning("Counter TextMeshPro is not assigned.");
        }
    }
}