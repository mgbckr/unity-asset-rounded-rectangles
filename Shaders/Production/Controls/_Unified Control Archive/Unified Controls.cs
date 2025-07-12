using UnityEngine;

public class UnifiedControls : MonoBehaviour
{
    public Material material;
    private RenderTexture A, B;
    // private bool useA = true; // toggle between A and B

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get material
        // material = GetComponent<Renderer>().material;

        A = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
        A.enableRandomWrite = true; A.Create();

        // B = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
        // B.enableRandomWrite = true; B.Create();

        // Set1x1RenderTextureValue(A, 1f); // Set initial value for A
        // Set1x1RenderTextureValue(B, 1f); // Set initial value for B

        material.SetTexture("_BoolTex", A);
        // material.SetColor("_Color", Color.green);
    }

    // Update is called once per frame

    void Update()
    {



        // if (useA)
        // {
        //     Graphics.Blit(A, B, material);
        //     material.SetTexture("_BoolTex", B);
        // }
        // else
        // {
        //     Graphics.Blit(B, A, material);
        //     material.SetTexture("_BoolTex", A);
        // }

        // useA = !useA;

        Debug.Log("Values");
        Debug.Log(ReadBooleanFromRenderTexture(A));
        // Debug.Log(ReadBooleanFromRenderTexture(B));
    }

    public Color ReadBooleanFromRenderTexture(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RFloat, false, true);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        return tex.GetPixel(0, 0);
    }
    
    public void Set1x1RenderTextureValue(RenderTexture rt, float value)
    {
        // Create a temp Texture2D with the desired value
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RFloat, false, true);
        tex.SetPixel(0, 0, new Color(value, 0, 0, 1)); // Only .r used
        tex.Apply();

        // Copy it into the RenderTexture
        Graphics.Blit(tex, rt);
    }
}
