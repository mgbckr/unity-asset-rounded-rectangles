using UnityEngine;

public class SetContent : MonoBehaviour
{
    private GameObject plane;

    [SerializeField]
    private Texture2D texture;

    [SerializeField]
    private Color color;

    void Start()
    {
        if (texture != null)
        {
            Debug.Log("Setting texture"); 
            GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        }

        if (color != null)
        {
             // Get the Mesh from the MeshFilter component
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            // Retrieve the vertices of the mesh
            Vector3[] vertices = mesh.vertices;
            Color[] colors = new Color[vertices.Length];

            // Set a color for each vertex. For example, using the vertex's position to create a color.
            for (int i = 0; i < vertices.Length; i++)
            {
                // This example creates a gradient color based on the x, y, and z coordinates
                colors[i] = color;
            }

            // Apply the color array to the mesh
            mesh.colors = colors;

        }
    }

    // Update is called once per frame 
    void Update()
    {
        
    }
}
