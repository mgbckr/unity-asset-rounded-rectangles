using UnityEngine;

public class Benchmark : MonoBehaviour
{
    public int numberOfDuplicates = 10; // Number of duplicates you want
    public Vector3 offset = new Vector3(0.001f, 0, 0.001f ); // Offset between objects to avoid overlap

    void Start()
    {
        // get quad component
        // Get the Quad component
        GameObject quad = gameObject.transform.Find("Quad").gameObject;
        if (quad != null)
        {
            for (int i = 0; i < numberOfDuplicates; i++)
            {
                // Instantiate a copy of the object at an offset position
                // and update padding of shader of new quad
                GameObject newQuad = Instantiate(
                    quad, 
                    gameObject.transform.position + offset * (i + 1), 
                    transform.rotation);
                newQuad.transform.SetParent(gameObject.transform); // Set the parent to the original object

                // set shader prperty "Padding"
                Material material = newQuad.GetComponent<Renderer>().material;
                if (material != null)
                {
                    // material.SetVector("_Padding_left_right_bottom_top", 
                    //     new Vector4(
                    //         0.05f+ 0.01f * (i + 1), 
                    //         0.05f+ 0.01f * (i + 1), 
                    //         0.05f+ 0.01f * (i + 1), 
                    //         0.05f+ 0.01f * (i + 1)) );
                    material.SetColor("_Main_Color", 
                        new Color(
                            1f - 1f/numberOfDuplicates * (i + 1),
                            0f,
                            0f,
                            1f));
                    
                    // set 
                }
                else
                {
                    Debug.LogWarning("Material not found on the quad.");
                }

                
            }
        }
        else
        {
            Debug.LogWarning("Quad not found in the scene.");
        }


    }
}
