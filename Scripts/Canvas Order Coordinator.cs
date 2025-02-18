using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class CanvasOrderCoordinator : MonoBehaviour
{
    [Tooltip("The GameObject that the canvases are sorted based on.")]
    public GameObject originGameObject;

    [Tooltip("Find canvases every N frames. 0 means no update.")]
    public int findCanvasesEveryNFrames = 0;

    [Tooltip("Set to true if you want to find all canvases at the start of the game.")]
    public bool findCanvasesOnStart = true;

    [Tooltip("Update sorting order every N frames. 0 means no update.")]
    public int updateSortingOrderEveryNFrames = 1;

    [Tooltip("Set to true if you want to update the sorting order at the start of the game.")]
    public bool updateSortingOrderOnStart = true;

    public HashSet<Canvas> canvases;

    private int frameCountFindCanvases = 0;

    private int frameCountUpdateSortingOrder = 0;


    void Start()
    {
        canvases = new HashSet<Canvas>();
        if (findCanvasesOnStart)
        {
            Canvas[] existingCanvases = 
                GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            canvases.UnionWith(existingCanvases);

            if (updateSortingOrderOnStart)
            {
                FindCanvases();
            }

            if (updateSortingOrderOnStart)
            {
                UpdateSortingOrder(canvases);
            }
        }
    }

    void Update()
    {
        if (findCanvasesEveryNFrames > 0)
        {
            frameCountFindCanvases++;
            if (frameCountFindCanvases >= findCanvasesEveryNFrames)
            {
                frameCountFindCanvases = 0;
                FindCanvases();
            }
        }

        if (updateSortingOrderEveryNFrames > 0)
        {
            frameCountUpdateSortingOrder++;
            if (frameCountUpdateSortingOrder >= updateSortingOrderEveryNFrames)
            {
                frameCountUpdateSortingOrder = 0;
                UpdateSortingOrder(canvases);
            }
        }

        // print player position for debugging
        Debug.Log($"Player position: {originGameObject.transform.position}");
        Debug.Log($"Camera position: {Camera.main.transform.position}");
    }

    // Find all canvases in the scene.
    public Canvas[] FindCanvases()
    {
        Canvas[] existingCanvases = 
            GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        canvases = new HashSet<Canvas>(existingCanvases);
        return existingCanvases;
    }

    // Update sorting order every frame (or move this to Start() if distances don't change).
    public void UpdateSortingOrder(HashSet<Canvas> canvasSet, int startValue = 1)
    {
        if (originGameObject == null)
        {
            Debug.Log("Player transform is not assigned!");
            return;
        }
        
        // Sort canvases based on distance from the player.
        // Closer canvases should have a higher sorting order.
        IEnumerable<Canvas> enumerator = canvasSet.OrderBy(canvas => 
            Vector3.Distance(
                canvas.transform.position, 
                originGameObject.transform.position));

        // Set sorting order so that the closest canvas gets the highest value.
        // For example, if there are 5 canvases, the closest gets 5, next gets 4, etc.
        Debug.Log("Setting sorting order...");
        int sortingValue = startValue + canvasSet.Count - 1;
        foreach (Canvas canvas in enumerator)
        {
            canvas.sortingOrder = sortingValue;
            Debug.Log($"Canvas {canvas.name} has sorting order {sortingValue}" +
                $" and distance {Vector3.Distance(canvas.transform.position, originGameObject.transform.position)}");
            sortingValue--;
        }
    }
}
