using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DysonSwarm;

public class SolarSystem : MonoBehaviour
{

    public float gameSpeed = 0;

    // List of bodies (filled at start)
    public List<Planet> planets = new List<Planet>();
    public List<Team> teams = new List<Team>();
    public SystemController controller;
    
    protected float launchInterval = 0;


    // Start is called before the first frame update
    void Start()
    {
        GenerateSystem();
        UpdateSensors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GenerateSystem()
    {
        // Initialize
        planets.AddRange(transform.GetComponentsInChildren<Planet>());
        teams.AddRange(transform.Find("Teams").GetComponentsInChildren<Team>());

        launchInterval = Statics.LAUNCH_COOLDOWN / teams.Count;

    }


    public void StartPlaying()
    {
        gameSpeed = 1;

    }



    protected float launchTimer = 0;
    protected int teamIndex = 0;
    public float elapsed = 0;


    void FixedUpdate()
    {
        if (elapsed > Statics.GAME_DURATION)
        {
            gameSpeed = 0;
            enabled = false;
            GameOver();
        }


        if (gameSpeed > 0)
        {
            elapsed += Time.deltaTime;
            launchTimer += Time.deltaTime;

            if (launchTimer >= Statics.LAUNCH_COOLDOWN) // Reset
            {
                launchTimer = 0;
                teamIndex = 0;
            }

            if (teamIndex < teams.Count && launchTimer >= teamIndex * launchInterval) // Launch in turns
            {
                teams[teamIndex].Launch();
                teamIndex++;
            }
            
        }

    }

    public void UpdateSensors()
    {
        foreach (Team team in teams)
        {
            team.sensors = planets;
            team.sensors.Remove(team.home);
        }
    }


    public void GameOver()
    {
        int totalValue = 0;
        foreach(Planet planet in planets)
        {
            totalValue += planet.value;
        }

        controller.GameOver();
    }

}
