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


        public void InstantiateWindow(string url, string title, Vector3 position)
        {
            GameObject windowInstance = Instantiate(
                windowPrefab,
                position,
                transform.rotation);

            windowInstance.transform.SetParent(transform, false);

            windowInstance.name = "Window: " + title;

            if (url != null)
            {
                Debug.Log($"Setting content for window '{title}' from URL: {url}");
                windowInstance.GetComponent<Window>().SetFrameContentFromUrl(url);
            }

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


        public void TestInstantiateWindows()
        {
            if (windowPrefab == null)
            {
                Debug.LogError("Window prefab is not assigned in the WindowManager.");
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                string name = "" + i;
                Vector3 position = transform.position + new Vector3(0.41f * i, 0, 0);
                InstantiateWindow(null, name, position);
            }

            string basePath = "file://" + Application.dataPath + "/External/Rounded Rectangles/Assets/Test/";
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    InstantiateWindow(
                        basePath + "test1.png",
                        "Test 1." + j,
                        transform.position + new Vector3(0.41f * (j * 3 + 1), 0.55f * (1 + i), 0));

                    InstantiateWindow(
                        basePath + "test2.png",
                        "Test 2." + j,
                        transform.position + new Vector3(0.41f * (j * 3 + 2), 0.55f * (1 + i), 0));

                    InstantiateWindow(
                        basePath + "test3.png",
                        "Test 3." + j,
                        transform.position + new Vector3(0.41f * (j * 3 + 3), 0.55f * (1 + i), 0));
                }
            }
        }
    }
}