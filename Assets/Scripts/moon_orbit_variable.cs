using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class moon_orbit_variable : MonoBehaviour
{
    public Transform ObjecttoOrbit;
    public float Vectorx = 1;
    public float Vectory = 1;
    public float Vectorz = 1;
    Vector3 VectortoOrbit;
    public GameObject SystemSpeedInput;


    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        float system_speed = SystemSpeedInput.GetComponent<Speed_Control>().system_speed;
        VectortoOrbit = new Vector3(Vectorx, Vectory, Vectorz);
        transform.RotateAround(ObjecttoOrbit.position, VectortoOrbit, -3.67f * system_speed * Time.deltaTime);
        

    }
}