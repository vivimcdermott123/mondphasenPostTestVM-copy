using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class moon_rotation : MonoBehaviour
{
    public Transform ObjecttoOrbit;
    public GameObject SystemSpeedInput;


    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        float system_speed = SystemSpeedInput.GetComponent<Speed_Control>().system_speed;
        transform.RotateAround(ObjecttoOrbit.position, Vector3.up, -0.367f * system_speed * Time.deltaTime);
       
    }
}