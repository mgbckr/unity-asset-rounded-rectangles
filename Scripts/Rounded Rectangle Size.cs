using UnityEngine;

public class RoundedRectangleSize : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updateDimensions();
    }

    // update width and height in shader
    void updateDimensions() {
        if (gameObject != null) {
            float width = gameObject.transform.localScale.x;
            float height = gameObject.transform.localScale.y;
            Debug.Log(
                "Updating dimensions: " + width + " " + height);
            Material material = gameObject.GetComponent<Renderer>().material;
            if (material != null) {
                float max = Mathf.Max(width, height);
                float normalizedWidth = width / max;
                float normalizedHeight = height / max;
                material.SetFloat("_Width", normalizedWidth);
                material.SetFloat("_Height", normalizedHeight);
            }
        } else {
            Debug.Log("GameObject is null");
        }
    }
}
