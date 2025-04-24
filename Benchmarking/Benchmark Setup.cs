using UnityEngine;
using System.Collections.Generic;

public class BenchmarkSetup : MonoBehaviour
{

    public float rotationSpeed = 360f;
    public int replicates = 1;
    public float offset = 0.01f;

    private string objectName = "Quad";
    private Transform objectTransform;
    private Transform[] clones;

    void Start()
    {
        objectTransform = transform.Find(objectName);
        if (objectTransform == null)
        {
            Debug.LogError("Object not found: '" + objectName);
            return;
        }
        else
        {
            // replicate the object
            clones = new Transform[replicates];
            clones[0] = objectTransform;
            for (int i = 1; i < replicates; i++)
            {
                Transform clone = Instantiate(
                    objectTransform, 
                    objectTransform.position + new Vector3(i * offset, i * offset, 0), 
                    Quaternion.identity);
                clone.name = objectName + "_" + i;
                clone.SetParent(transform);
                clones[i] = clone;
            }
        }
    }

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        if (objectTransform != null)
        {
            // Rotate around the Y axis
            for (int i = 0; i < clones.Length; i++)
            {
                Transform clone = clones[i];
                clone.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            }
        }
    }
}
