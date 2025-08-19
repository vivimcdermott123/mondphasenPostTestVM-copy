using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earth_rotation : MonoBehaviour
{
    public Transform ObjecttoOrbit;
    Vector3 VectortoOrbit;
    public GameObject System_Speed_Input;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        float system_speed = System_Speed_Input.GetComponent<Speed_Control>().system_speed;
        VectortoOrbit = new Vector3(1, -2.3f, 0);
        transform.RotateAround(ObjecttoOrbit.position, VectortoOrbit, 200 * system_speed * Time.deltaTime);


    }
}