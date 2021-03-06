﻿
using coloradoJam;
using UnityEngine;

public class Fleet : MonoBehaviour
{
    
    public static float PLANETARY_ORBIT_SPEED = 100;
    public static int LARGE_PLANET = 30;
    public static int SMALL_PLANET = 3;
    public static float CLOSE_ORBIT = 0.6f;
    public static float FAR_ORBIT = 3;

    // Given by actor
    public float ships = 0;
    public float thrust = 0;
    public float cargo = 0;

    public Team sender;

    // Navigation
    public CelestialBody destination;
    public float angularDeltaV = 0;
    public float inwardDeltaV = 0;
    public float ETA = 0;
    public GPS gps;

    // Orbit
    public CelestialBody orbiting;
    public Vector3 orbitAxis = Vector3.down;
    public float orbitSpeed = PLANETARY_ORBIT_SPEED;

    public Ticket ticket = null;

    // Start is called before the first frame update
    void Start()
    {
        InitGPS();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = gps.GlobalPos(transform);

        if (position.magnitude < 10)
        {
            // Icarus
            destination = null;
            orbiting = null;
            name = "Melted fleet";
            enabled = false;
        }
        if (ETA < -30)
        {
            // This wasnt supposed to happen
            //destination = null;
            orbiting = null;
            name = "Empty hulks";
            ships = 0;
            cargo += ships;
        }

        if (orbiting != null)
        {
            // Fleet is in orbit
            transform.RotateAround(orbiting.transform.position, orbitAxis.normalized, orbitSpeed * Time.deltaTime);
            transform.LookAt(orbiting.transform);
            return;
        }

        // Else go to destination
        if (destination == null)
            return;

        ETA -= Time.deltaTime;
        transform.LookAt(destination.transform);

        // Enter planetary orbit if within range
        Vector3 direction = destination.transform.position - transform.position;
        float distance = direction.magnitude;
        float scale = destination.transform.localScale.x;
        if ( (scale < LARGE_PLANET && distance < scale) ||
            (scale >= LARGE_PLANET && distance < scale * CLOSE_ORBIT) ||
            (scale <= SMALL_PLANET && distance < scale * FAR_ORBIT) )
        {
            EnterOrbit();
            return;
        }

        // Course correction
        if (ETA < 0 && distance < thrust) 
        {
            EnterOrbit();
            return;
        }

        // Else fly towards destination

        // DeltaV towards or away from sun
        direction = - inwardDeltaV * position.normalized;
        transform.Translate(direction * Time.deltaTime, gps.transform);

        // DeltaV with or against orbit
        if (angularDeltaV > 0)
            transform.RotateAround(gps.gravityCenter, Vector3.down, angularDeltaV * Time.deltaTime);
        else
            transform.RotateAround(gps.gravityCenter, Vector3.up, -angularDeltaV * Time.deltaTime);

        if (ETA < 3)
            gps.ShipArriving(this);
    }



    public bool Launch(CelestialBody target)
    {
        if (target == null)
        {
            Statics.LogError("No target", "Fleet", this.name);
            return false;
        }


        if (sender == null)
        {
            Statics.LogError("No sender", "Fleet", this.name);
            return false;
        }



        NavigationData data = CalculateNavigation(target);

        if (data.launchWindow == false)
        {
            destination = null;
            return false;
        }

        destination = target;
        angularDeltaV = data.angularDeltaV;
        inwardDeltaV = data.inwardDeltaV;
        ETA = data.ETA;

        Planet planet = target.GetComponent<Planet>();
        if (planet)
            planet.WarnIncoming(this);

        if (orbiting)
        {
            planet = orbiting.GetComponent<Planet>();
            if (planet)
                planet.LeftOrbit(this);

            orbiting = null;
        }


        return true;
    }


    public void EnterOrbit()
    {
        float orbitDistance = 1;
        float scale = destination.transform.localScale.x;

        if (scale > LARGE_PLANET)
        {
            orbitDistance = CLOSE_ORBIT;
            orbitSpeed = PLANETARY_ORBIT_SPEED * orbitDistance;
        }
        else if (scale < SMALL_PLANET)
            orbitDistance = FAR_ORBIT;

        if (destination)
        {
            // Reached destination
            transform.parent = destination.transform;
            orbiting = destination;

            Planet planet = destination.GetComponent<Planet>();
            if (planet)
                planet.EnteredOrbit(this);

            destination = null;

        }
        else if (!orbiting)
        {
            Statics.LogError("Nothing to orbit","Fleet",name);
            return;
        }

        if (transform.parent != orbiting.transform)
            transform.parent = orbiting.transform;


        Vector3 pos = transform.localPosition.normalized * orbitDistance;
        transform.localPosition = pos;

        //orbitAxis = Vector3.Cross(pos, Vector3.RotateTowards(pos, -pos, 90, 0));
        orbitAxis = Vector3.Cross(pos, orbiting.transform.position);
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <returns>false is window is closed</returns>
    public NavigationData CalculateNavigation (CelestialBody target)
    {
        if (!gps)
            InitGPS();
        NavigationData data = new NavigationData();

        Vector3 current = gps.GlobalPos(transform);
        CelestialBody start = transform.parent.GetComponent<CelestialBody>();
        float distanceAU = Mathf.Abs(target.orbitalRadiusAU - current.magnitude * 0.01f);
        float startingDeltaAngle = Vector3.SignedAngle(current, target.transform.position - gps.transform.position, Vector3.down);
        float angularThrustRatio = start.angularVelocity / start.orbitalSpeed; // change in degrees per sec for each unit of thrust
        float deltaSpeed = start.angularVelocity - target.angularVelocity;
        float distanceIG = distanceAU * Statics.AU_TO_IG_RATIO;

        // Already there
        if (distanceAU < 0.1f)
            return data;


        // Equation to find where trajectory meets target
        // ETA = (targetAngle - currentAngle) / (start.speed + lateralthrust * ratio - target.speed) = distance / inwardDeltaV
        // deltaSpeed = start - target
        // ETA = deltaAngle / (deltaSpeed + angularDeltaV)
        // ETA = distance / inwardDeltaV
        // (deltaAngle * inwardDeltaV) / distance - deltaSpeed = angularDeltaV


        // Try Hohmann transfer

        float inwardThrustRatio = 0.1f;
        float inwardThrust = thrust * inwardThrustRatio;
        float neededAngularDeltaV = (startingDeltaAngle * inwardThrust) / distanceIG - deltaSpeed;
        float lateralThrust = Mathf.Abs(neededAngularDeltaV / angularThrustRatio);
        float timeToArrival;

        if (lateralThrust > thrust * (1 - inwardThrustRatio)) // Not enough deltaV
        {
            // Transfer impossible, Launch window is closed

            float maxAngularDeltaV = thrust * 0.9f * angularThrustRatio;
            float windowAngle = (maxAngularDeltaV + deltaSpeed) * distanceIG / inwardThrust; // Next window

            data.launchWindow = false;
            return data;
        }
       
        // Use golden ratio

        // ETA = deltaAngle / (deltaSpeed + angularDeltaV) = distance / inwardDeltaV
        if (neededAngularDeltaV > 0)
        {
            // deltaAngle / (deltaSpeed + angularDeltaV) = distance / (thrust - angularDeltaV / ratio)
            // deltaAngle * (thrust - angularDeltaV / ratio) = distance * (deltaSpeed + angularDeltaV)
            // - deltaAngle * angularDeltaV / ratio - angularDeltaV * distance  = distance * deltaSpeed - deltaAngle * thrust
            // angularDeltaV = (distance * deltaSpeed - deltaAngle * thrust ) / ( -deltaAngle / ratio - distance )
            neededAngularDeltaV = (distanceIG * deltaSpeed - startingDeltaAngle * thrust) / (-startingDeltaAngle / angularThrustRatio - distanceIG);
            inwardThrust = thrust - neededAngularDeltaV / angularThrustRatio;
        }
        else
        {
            // deltaAngle / (deltaSpeed + angularDeltaV) = distance / (thrust + angularDeltaV / ratio)
            // angularDeltaV = (distance * deltaSpeed - deltaAngle * thrust ) / ( deltaAngle / ratio - distance )
            neededAngularDeltaV = (distanceIG * deltaSpeed - startingDeltaAngle * thrust) / (startingDeltaAngle / angularThrustRatio - distanceIG);
            inwardThrust = thrust + neededAngularDeltaV / angularThrustRatio;
        }
        lateralThrust = Mathf.Abs(neededAngularDeltaV / angularThrustRatio);
        timeToArrival = startingDeltaAngle / (deltaSpeed + neededAngularDeltaV);


        if (timeToArrival < 0 || lateralThrust + inwardThrust > thrust ) // Not a valid trajectory
        {
            // Go slower until it works
            inwardThrustRatio = 0.9f;
            float step = 0.1f;
            inwardThrust = thrust * inwardThrustRatio;
            neededAngularDeltaV = (startingDeltaAngle * inwardThrust) / distanceIG - deltaSpeed;
            lateralThrust = Mathf.Abs(neededAngularDeltaV / angularThrustRatio);
            while (lateralThrust > thrust * (1 - inwardThrustRatio) && inwardThrustRatio > 0)
            {
                inwardThrustRatio -= step;
                inwardThrust = thrust * inwardThrustRatio;
                neededAngularDeltaV = (startingDeltaAngle * inwardThrust) / distanceIG - deltaSpeed;
                lateralThrust = Mathf.Abs(neededAngularDeltaV / angularThrustRatio);
            }
            // refine
            inwardThrustRatio += step;
            step = 0.01f;
            while (lateralThrust > thrust * (1 - inwardThrustRatio))
            {
                inwardThrustRatio -= step;
                inwardThrust = thrust * inwardThrustRatio;
                neededAngularDeltaV = (startingDeltaAngle * inwardThrust) / distanceIG - deltaSpeed;
                lateralThrust = Mathf.Abs(neededAngularDeltaV / angularThrustRatio);
            }

            timeToArrival = startingDeltaAngle / (deltaSpeed + neededAngularDeltaV);
        }

        


        if (target.orbitalRadiusAU > current.magnitude * 0.01f) // higher orbit
            inwardThrust = -inwardThrust;

        data.angularDeltaV = neededAngularDeltaV;
        data.inwardDeltaV = inwardThrust;
        data.ETA = timeToArrival;
        data.launchWindow = true;

        return data;
    }


    public float LoseShips(float amount)
    {
        float lost = Mathf.Min(amount, ships);
        ships -= lost;

        if (ships < Statics.MIN)
            Die();

        return lost;
    }

    public void InitGPS()
    {
        Transform system = transform.parent.parent;
        gps = system.GetComponent<GPS>();
        while (!gps)
        {
            system = system.parent;
            gps = system.GetComponent<GPS>();
        }
    }

    public void Die()
    {
        Planet dock;

        if (destination)
        {
            dock = destination.GetComponent<Planet>();
            if (dock)
                dock.incomingFleets.Remove(this);
        }
        
        if (orbiting)
        {
            dock = orbiting.GetComponent<Planet>();
            if (dock)
                dock.incomingFleets.Remove(this);
        }

        Destroy(this.gameObject);
    }

    public class NavigationData
    {
        public CelestialBody destination;
        public bool launchWindow = false;
        public float angularDeltaV = 0;
        public float inwardDeltaV = 0;
        public float ETA = 0;
    }
}

