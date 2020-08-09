using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPS : MonoBehaviour
{
    public List<Sun> suns = new List<Sun>();
    public Vector3 gravityCenter = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (suns.Count > 0)
            gravityCenter = suns[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Vector3 GlobalPos(Transform body)
    {

        return body.position - gravityCenter;
    }

}
