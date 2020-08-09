
using UnityEngine;
using DysonSwarm;

public class CelestialBody : MonoBehaviour
{
    public float orbitalRadiusAU = 0;
    public float orbitalPeriodYears = 0;
    public float rotationPeriodDays = 0;
    public bool tidalLock = false;
    public float tilt = 0;

    protected float daysPerSec = 1;
    protected Transform model;
    
    public float rotationSpeed = 0; // rotation speed in angles per sec
    public float angularVelocity = 0; // orbit speed in angles per sec
    public float orbitalSpeed = 0; // orbit speed in game distance / sec

    // Start is called before the first frame update
    void Start()
    {
        model = transform.Find("Model");

        // Conversion to degrees / days (game sec)
        if (orbitalPeriodYears != 0)
            angularVelocity = 360 / (orbitalPeriodYears * 365 );

        // Conversion to degrees / sec
        if (rotationPeriodDays != 0)
            rotationSpeed = 360 / rotationPeriodDays;

        orbitalSpeed = 2 * Mathf.PI * orbitalRadiusAU / (365 * orbitalPeriodYears); // AU / days
        orbitalSpeed = orbitalSpeed * Statics.AU_TO_IG_RATIO; // game distance / sec

        // Tilt
        model.transform.Rotate(Vector3.left, tilt);

        // Starting pos
        transform.localPosition = new Vector3(orbitalRadiusAU * Statics.AU_TO_IG_RATIO, 0, 0);
        transform.RotateAround(transform.parent.position, Vector3.down, Random.Range(0f, 360f));
    }

    // Update is called once per frame
    void Update()
    {
        float elapsed = Time.deltaTime * GameSpeed;

        // Rotation
        if (tidalLock)
        {
            transform.LookAt(transform.parent);
        }
        else
        {
            if (rotationSpeed >= 0)
                model.Rotate(Vector3.down, rotationSpeed * elapsed);
            else
                model.Rotate(Vector3.up, -rotationSpeed * elapsed);
        }

        // Orbit
        transform.RotateAround(transform.parent.position, Vector3.down, angularVelocity * elapsed);
    }

    public float GameSpeed
    {
        get {
            if (transform.parent.GetComponent<SolarSystem>())
                return transform.parent.GetComponent<SolarSystem>().gameSpeed;
            else if (transform.parent.GetComponent<CelestialBody>())
                return transform.parent.GetComponent<CelestialBody>().GameSpeed;
            else
                return 0;
        }
    }
}
