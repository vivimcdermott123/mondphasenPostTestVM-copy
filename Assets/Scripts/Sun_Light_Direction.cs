using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun_Light_Direction : MonoBehaviour
{
    public Transform Objecttofollow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Objecttofollow.position);
        
    }
}
