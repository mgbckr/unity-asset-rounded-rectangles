using UnityEngine;


namespace RoundedRectangles {
    public class RoundedRectangle : MonoBehaviour
    {
        [SerializeField]
        private Texture2D texture;

        [SerializeField]
        private Color color;

        [SerializeField]
        private bool keepBorderConstant = true;
        [SerializeField]
        private bool keepRadiiConstant = true;
        [SerializeField]
        private GameObject scaleReference;

        // dimensions
        private Vector2 originalDimensions;
        // border
        private Vector4 borderSizes;
        // radius
        private Vector4 radii;

        void Start()
        {
            // save dimensions
            if (scaleReference == null)
            {
                originalDimensions = gameObject.transform.localScale;
            }

            // save border from material
            Material material = gameObject.GetComponent<Renderer>().material;
            borderSizes = new Vector4(
                material.GetFloat("_Border_Top"),
                material.GetFloat("_Border_Bottom"),
                material.GetFloat("_Border_Left"),
                material.GetFloat("_Border_Right")
            );
            
            // save radii from material
            radii = new Vector4(
                material.GetFloat("_Corner_Radius_Top_Left"),
                material.GetFloat("_Corner_Radius_Top_Right"),
                material.GetFloat("_Corner_Radius_Bottom_Left"),
                material.GetFloat("_Corner_Radius_Bottom_Right")
            );


            // set texture
            if (texture != null)
            {
                Debug.Log("Setting texture"); 
                GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            }

            // set color
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
            UpdateDimensions();
            UpdateRadiusAndBorder();
        }

        void UpdateDimensions() {
            if (gameObject != null) {
                Material material = gameObject.GetComponent<Renderer>().material;
                
                if (material != null) {
                    
                    float width = gameObject.transform.localScale.x;
                    float height = gameObject.transform.localScale.y;
                    Debug.Log(
                        "Updating dimensions: " + width + " " + height);

                    material.SetFloat("_Width", width);
                    material.SetFloat("_Height", height);
                
                }
            } else {
                Debug.Log("GameObject is null");
            }
        }

        void UpdateRadiusAndBorder()
        {
            if (gameObject != null) {
                Material material = gameObject.GetComponent<Renderer>().material;
                
                if (material != null) {

                    Vector2 currentDimensions = gameObject.transform.localScale;

                    Debug.Log("Current dimensions: " + currentDimensions.x + " " + currentDimensions.y);
                    Debug.Log("Original dimensions: " + originalDimensions.x + " " + originalDimensions.y);

                    float ratioHeight = originalDimensions.y / currentDimensions.y ;
                    float ratioWidth = originalDimensions.x / currentDimensions.x;

                    if (keepBorderConstant)
                    {
                        float ratioBorder = 0f;
                        if (material.GetInt("_Border_Width_Reference") == 1)
                        {
                            ratioBorder = ratioWidth;
                        }
                        else
                        {
                            ratioBorder = ratioHeight;
                        }
                        material.SetFloat("_Border_Top",    borderSizes.x * ratioBorder);
                        material.SetFloat("_Border_Bottom", borderSizes.y * ratioBorder);
                        material.SetFloat("_Border_Left",   borderSizes.z * ratioBorder);
                        material.SetFloat("_Border_Right",  borderSizes.w * ratioBorder);
                    }
  
                    if (keepRadiiConstant) 
                    {
                        float ratioRadius = 0f;
                        if (material.GetInt("_Corner_Radius_Width_Reference") == 1)
                        {
                            ratioRadius = ratioWidth;
                        }
                        else
                        {
                            ratioRadius = ratioHeight;
                        }
                        material.SetFloat("_Corner_Radius_Top_Left",        radii.x * ratioRadius);
                        material.SetFloat("_Corner_Radius_Top_Right",       radii.y * ratioRadius);
                        material.SetFloat("_Corner_Radius_Bottom_Left",     radii.z * ratioRadius);
                        material.SetFloat("_Corner_Radius_Bottom_Right",    radii.w * ratioRadius);
                    }
                }
            } else {
                Debug.Log("GameObject is null");
            }
        }
    }
}