using UnityEngine;

public class FrameFraction : MonoBehaviour
{

    public Vector4 padding;
    public Vector4 cornerRadius;
    public float borderWidth;
    public float relativeOrigin;
    public float relativeStart;
    public float relativeEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Format();
        
    }

    void Format()
    {

        // Get scale
        float width = transform.localScale.x;
        float height = transform.localScale.y;
        float maxScale = Mathf.Max(width, height);
        Vector2 relativeScale = new Vector2(width / maxScale, height / maxScale);
        Vector4 scalePadding = new Vector4(
            0f,
            1 - relativeScale.x,
            0f,
            1 - relativeScale.y
        );

        // Get the material from the renderer
        Material material = GetComponent<Renderer>().material;

        // Set the basic properties of the material
        material.SetVector("_Tiling", relativeScale);
        material.SetVector("_Padding_left_right_bottom_top", scalePadding + padding / maxScale);
        material.SetVector("_Corner_Radius_lb_lt_rb_rt", cornerRadius / maxScale);
        material.SetFloat("_Border_Width", borderWidth / maxScale);

        // left, right, bottom, top edge
        Vector4 edgeLengths = 
            new Vector4(padding.z, padding.z, padding.x, padding.x) +
            new Vector4(padding.w, padding.w, padding.y, padding.y) +
            new Vector4(cornerRadius.x, cornerRadius.z, cornerRadius.x, cornerRadius.y) +
            new Vector4(cornerRadius.y, cornerRadius.w, cornerRadius.z, cornerRadius.w);
        edgeLengths = new Vector4(height, height, width, width) - edgeLengths;

        // left-bottom, left-top, right-bottom, right-top corner (quarter circumference)
        // For the radii we subtract half the border width.
        // Formula: 2 * pi * r / 4
        float half = borderWidth / 2f;
        Vector4 halfBorder = new Vector4(half, half, half, half);
        Vector4 cornerLengths = (cornerRadius - halfBorder) * Mathf.PI / 2f;

        float[] orderedLengths = new float[8] {
            edgeLengths.x,
            cornerLengths.x,
            edgeLengths.z,
            cornerLengths.z,
            edgeLengths.y,
            cornerLengths.w,
            edgeLengths.w,
            cornerLengths.y
        };

        float[] cumsum = new float[9];
        cumsum[0] = 0;
        for (int i = 0; i < 8; i++)
        {
            cumsum[i + 1] = cumsum[i] + orderedLengths[i];
        }
        float circumference = cumsum[8];

        Vector4 startEdges = new Vector4(
            Mathf.Min(relativeStart * circumference, cumsum[1]) / maxScale,
            0,
            0,
            0
        );

        // material.SetFloat("_Origin", settings.x);
        material.SetVector("_First_Start_Offset_left_right_bottom_top", startEdges);
        material.SetVector("_Second_Start_Offset_left_right_bottom_top", startEdges);
        // material.SetFloat("_End", settings.z);
    }
}
