using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed_Control : MonoBehaviour
{
    [Range(-5, 5)]
    public float system_speed = 0f ;
    public float change_speed = 1;
    private float speed_save;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void speed_control()
    {

        float system_speed = Input.GetAxis("Horizontal") * change_speed;        

    }

    public void pause()
    {

        if(system_speed != 0)
        {
            speed_save = system_speed;
            system_speed = 0 ;
        }

        else if(system_speed ==0)
        {

            system_speed = speed_save;

        }

    }
   
}
