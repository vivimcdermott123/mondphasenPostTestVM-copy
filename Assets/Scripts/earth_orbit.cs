using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earth_orbit : MonoBehaviour
{
    public Transform Stern;
    public GameObject SystemSpeedInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

        float system_speed = SystemSpeedInput.GetComponent<Speed_Control>().system_speed;
        transform.RotateAround(Stern.position, Vector3.up, - 0.548f * system_speed * Time.deltaTime);

        
    }
}
