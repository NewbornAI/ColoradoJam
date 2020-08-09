using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    //static int ORBIT_POINTS = 45;

    public List<Fleet> incomingFleets = new List<Fleet>();
    public List<Fleet> orbitingFleets = new List<Fleet>();

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
        //DrawOrbit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void DrawOrbit()
    //{
    //    LineRenderer orbitLine = GetComponentInChildren<LineRenderer>();
    //    if (!orbitLine)
    //        return;
    //    orbitLine.transform.parent = transform.parent;

    //    Vector3[] orbitPoints = new Vector3[ORBIT_POINTS];
    //    float step = 360 / ORBIT_POINTS;
    //    for (int i = 0; i < ORBIT_POINTS; i++)
    //    {
    //        orbitPoints[i] = transform.position;
    //        transform.RotateAround(transform.parent.position, Vector3.down, step);
    //    }
    //    orbitLine.positionCount = ORBIT_POINTS;
    //    orbitLine.SetPositions(orbitPoints);
    //}

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
