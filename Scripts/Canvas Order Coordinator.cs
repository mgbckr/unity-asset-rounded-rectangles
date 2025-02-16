using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class CanvasOrderCoordinator : MonoBehaviour
{
    [Tooltip("The GameObject that the canvases are sorted based on.")]
    public GameObject originGameObject;
    
    [Tooltip("Set to true if you want to find all canvases at the start of the game.")]
    public bool findCanvasesOnStart = true;

    [Tooltip("Set to true if you want to update the canvases every frame. Potentially slow.")]
    public bool resetCanvasesOnUpdate = false;

    public HashSet<Canvas> canvasSet;


    void Start()
    {
        canvasSet = new HashSet<Canvas>();
        if (findCanvasesOnStart)
        {
            Canvas[] existingCanvases = 
                GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            canvasSet.UnionWith(existingCanvases);
        }
    }

    void Update()
    {
        if (resetCanvasesOnUpdate)
        {
            FindCanvases();
        }
        UpdateSortingOrder(canvasSet);
    }

    // Find all canvases in the scene.
    void FindCanvases()
    {
        Canvas[] existingCanvases = 
            GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        canvasSet = new HashSet<Canvas>(existingCanvases);
    }

    // Update sorting order every frame (or move this to Start() if distances don't change).
    void UpdateSortingOrder(HashSet<Canvas> canvasSet)
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
        int sortingValue = 1;
        foreach (Canvas canvas in enumerator)
        {
            canvas.sortingOrder = sortingValue;
            sortingValue++;
        }
    }
}
