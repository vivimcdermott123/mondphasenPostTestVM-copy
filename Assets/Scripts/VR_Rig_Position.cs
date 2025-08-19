using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Rig_Position : MonoBehaviour
{
    public Transform VR_Anchor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = VR_Anchor.transform.position;

    }
}
