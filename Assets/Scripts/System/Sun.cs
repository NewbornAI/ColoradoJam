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

    public float rotationSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = 360 / 25;
    }

    //public float tiltSpeed = 10;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
    }
}
