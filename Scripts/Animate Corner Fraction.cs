using UnityEngine;

public class AnimateCornerFraction : MonoBehaviour
{
    void Start()
    {


    }

    void Update()
    {



        float pulse = Mathf.PingPong(Time.time * 1, 1f);
        float scale = Mathf.Lerp(0f, 1f, pulse);

        // animate the "End" property of the underlying shader
        Material material = GetComponent<Renderer>().material;
        material.SetFloat("_End", scale);




    }
}
