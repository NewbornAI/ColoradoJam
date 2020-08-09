
using System.Collections.Generic;
using DysonSwarm;
using UnityEngine;

public class Team : MonoBehaviour
{

    public Planet home;
    public List<Planet> sensors = new List<Planet>();

    // Prefabs
    public Fleet fleetPrefab;

    public int players = 0;


    // Start is called before the first frame update
    void Start()
    {
    }


    public void Launch()
    {
        Fleet fleet = CreateFleet();
        fleet.ships = 1;

        Fleet.NavigationData[] data = new Fleet.NavigationData[sensors.Count];
        List<Planet> validDestinations = new List<Planet>();

        for (int i = 0; i < sensors.Count; i++)
        {
            data[i] = fleet.CalculateNavigation(sensors[i].GetComponent<CelestialBody>());

            if (data[i].launchWindow && data[i].ETA < TimeRemaining) // Planet can be reached
                validDestinations.Add(sensors[i]);
        }

        bool launched = false;

        if (validDestinations.Count > 0)
        {
            CelestialBody destination = validDestinations[Random.Range(0, validDestinations.Count)].GetComponent<CelestialBody>();
            if (fleet.Launch(destination)) // Launch to random reachable planet
                launched = true;
        }

        if (!launched)
        {
            // Transfer ships to next launch
            //fleet.Die();
        }
    }


    protected Fleet CreateFleet()
    {
        Fleet newFleet = Instantiate(fleetPrefab.gameObject, home.transform).GetComponent<Fleet>();
        newFleet.transform.localScale = new Vector3(0.1f / transform.parent.localScale.x, 0.1f / transform.parent.localScale.x, 0.1f / transform.parent.localScale.x); // Always the same global scale
        newFleet.sender = this;
        newFleet.name = this.name;
        return newFleet;
    }


    protected float TimeRemaining
    {
        get { return Statics.GAME_DURATION - transform.parent.parent.GetComponent<SolarSystem>().elapsed; }
    }
}
