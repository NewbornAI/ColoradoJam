using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using coloradoJam;

public class SolarSystem : MonoBehaviour
{

    public float gameSpeed = 0;

    // List of bodies (filled at start)
    public List<Planet> planets = new List<Planet>();
    public List<Team> teams = new List<Team>();
    public SystemController controller;
    
    protected float launchInterval = 0;

    public Team blue;
    public Team green;
    public Team red;
    public Team yellow;

    void Awake()
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
        int first = Random.Range(0, teams.Count); // Random first player
        while (first > 0)
        {
            Team temp = teams[0];
            teams.RemoveAt(0);
            teams.Add(temp);
            first--;
        }

        blue = transform.Find("Teams").Find("Blue").GetComponent<Team>();
        green = transform.Find("Teams").Find("Green").GetComponent<Team>();
        red = transform.Find("Teams").Find("Red").GetComponent<Team>();
        yellow = transform.Find("Teams").Find("Yellow").GetComponent<Team>();


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
            float totalShips = 0;
            foreach (Fleet fleet in planet.orbitingFleets)
                totalShips += fleet.ships;

            foreach (Fleet fleet in planet.orbitingFleets)
            {
                if (fleet.ticket != null)
                {
                    fleet.ticket.winnings += Mathf.RoundToInt(fleet.ships-1 / totalShips * planet.value); // player gets rewards
                }

                fleet.sender.winnings += 1 / totalShips * planet.value; // team gets the rest

            }

        }

        foreach (Team team in teams)
        {
            float totalShips = Statics.GAME_DURATION / Statics.LAUNCH_COOLDOWN; // npc ships

            foreach (Ticket ticket in team.tickets)
                totalShips += ticket.numberOfShips; // total ships sent by team

            foreach (Ticket ticket in team.tickets)
                ticket.winnings = Mathf.RoundToInt(ticket.numberOfShips * team.winnings / totalShips);
        }

        controller.GameOver();
    }

}
