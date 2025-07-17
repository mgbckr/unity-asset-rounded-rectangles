using System.Collections.Generic;
using UnityEngine;

namespace Frames
{

    public class WindowManager : MonoBehaviour
    {

        public GameObject windowPrefab;

        // list of instantiated windows
        private List<GameObject> instantiatedWindows = new List<GameObject>();

        public void Start()
        {
            // TestInstantiateWindows();
        }

        public void InstantiateWindow(string title, Vector3 position)
        {
            GameObject windowInstance = Instantiate(
                windowPrefab,
                position,
                transform.rotation);

            windowInstance.transform.SetParent(transform, false);

            windowInstance.name = "Window: " + title;

            instantiatedWindows.Add(windowInstance);
        }

        public void ClearWindows()
        {
            foreach (GameObject window in instantiatedWindows)
            {
                Destroy(window);
            }
            instantiatedWindows.Clear();
        }
    }
}