using UnityEngine;

public class AnimateCornerFraction : MonoBehaviour
{
    void Start()
    {


    }

    void Update()
    {



        float pulse = Mathf.PingPong(Time.time * 0.3f, 1f);
        float scale = Mathf.Lerp(0f, 1f, pulse);

        float pulse2 = Mathf.PingPong((Time.time + 0.3f) * 0.3f, 1f);
        float scale2 = Mathf.Lerp(0f, 1f, pulse2);

        

        // animate the "End" property of the underlying shader
        Material material = GetComponent<Renderer>().material;
        material.SetFloat("_Start", scale);
        material.SetFloat("_End", scale2);




    }
}
