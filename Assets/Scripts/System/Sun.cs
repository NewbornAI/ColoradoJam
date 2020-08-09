using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{

    public enum Category
    {
        RedDwarf,
        Orange,
        RedGiant,
        BlueGiant,
        NeutronStar
    }

    public Category starCategory;

    public float habitableZoneStart = 0.5f;
    public float habitableZoneEnd = 1.5f;


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    //public float tiltSpeed = 10;

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(new Vector3(0, tiltSpeed * Time.deltaTime, 0));

        //if (tiltSpeed > 0)
        //{
        //    if (transform.position.y > 2)
        //        tiltSpeed = -tiltSpeed;
        //}
        //else
        //{
        //    if (transform.position.y < -2)
        //        tiltSpeed = -tiltSpeed;
        //}
    }
}
