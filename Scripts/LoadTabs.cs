using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial;


public class LoadTabs: MonoBehaviour, SpatialPointerStateListener
{
    public MqttManager mqttManager;
    public WindowManager windowManager;

    public void OnEvent(Touch touch, SpatialPointerState touchData)
    {
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            Debug.Log("LoadTabs | Touch ended or canceled, loading tabs...");
            if (mqttManager != null)
            {
                windowManager.ClearWindows();
                Debug.Log("LoadTabs | Requesting tab collection from MQTT manager...");
                mqttManager.Publish(
                    "immermind/test",
                    new TabRequestMessage()
                );
                // gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("LoadTabs | MqttManager is not set, cannot request tab collection.");
            }
        }
    }
}