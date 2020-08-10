using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public List<Fleet> incomingFleets = new List<Fleet>();
    public List<Fleet> orbitingFleets = new List<Fleet>();

    public int value = 0;

    public enum PlanetType
    {
        Ocean,
        Gas,
        Rock,
        Ice
    }

    public Team owner;

    // Start is called before the first frame update
    void Start()
    {
        if (value == 0)
        {
            value = Mathf.RoundToInt((transform.localScale.x + GetComponent<CelestialBody>().orbitalRadiusAU));
        }

        if (value < 1)
            value = 1;

        if (name == "Terra")
            value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void WarnIncoming(Fleet fleet)
    {
        incomingFleets.Add(fleet);
    }

    public void EnteredOrbit(Fleet fleet)
    {
        incomingFleets.Remove(fleet);
        orbitingFleets.Add(fleet);
    }
    public void LeftOrbit(Fleet fleet)
    {
        orbitingFleets.Remove(fleet);
    }

    public CelestialBody CelestialBody
    {
        get { return GetComponent<CelestialBody>(); }
    }
}
