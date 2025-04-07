using UnityEngine;

public class AnimateCornerFraction : MonoBehaviour
{

    public float speed = 1f;

    void Start()
    {


    }

    void Update()
    {

        float pulse = Mathf.PingPong(Time.time * speed, 1f);
        float sawtooth = (Time.time % (1/speed)) / (1/speed);

        // animate the "End" property of the underlying shader
        Material material = GetComponent<Renderer>().material;
        material.SetFloat("_Origin", sawtooth);




    }
}
