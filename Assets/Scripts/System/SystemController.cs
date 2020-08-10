using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using coloradoJam;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

public class SystemController : MonoBehaviour
{

    public SolarSystem system;
    public UIHandler UI;

    public SolarSystem systemPrefab;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        StartPlaying();
    }

    public void StartPlaying()
    {
        LoadPlayers();
        system.StartPlaying();
    }

    public void LoadPlayers()
    {
        Tickets tickets = new Tickets();

        try
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Tickets.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            tickets = (Tickets)formatter.Deserialize(stream); 
            stream.Close();
        }
        catch (Exception e)
        {
            if (e != null) { }

            Statics.LogDebug("Could not open " + "Tickets.bin");
        }


        Team team = null;
        foreach (Ticket ticket in tickets.list)
        {
            switch(ticket.team)
            {
                case Teams.Blue:
                    team = system.blue;
                    break;
                case Teams.Green:
                    team = system.green;
                    break;
                case Teams.Red:
                    team = system.red;
                    break;
                case Teams.Yellow:
                    team = system.yellow;
                    break;
            }

            if (team)
                team.tickets.Add(ticket);
        }
    }


    public void GameOver()
    {
        // Score screen

        // Save winnings in file
        string time = DateTime.Now.ToString();
        time = time.Replace(" ", "_");
        time = time.Replace("/", "");
        time = time.Replace(":", "");
        string saveFile = "Payout_" + time + ".bin";

        try
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
            foreach (Team team in system.teams)
            {
                formatter.Serialize(stream, team.tickets);
            }
            stream.Close();
        }
        catch (Exception e)
        {
            if (e != null) { }

            Statics.LogDebug("Could not open " + saveFile);
        }

        saveFile = "Payout_" + time + ".txt";

        try
        {
            Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);

            string content = "Ticket (bought) Payout\r\n";
            int totalPayout = 0;
            int totalShips = 0;

            foreach (Team team in system.teams)
            {
                foreach (Ticket ticket in team.tickets)
                {
                    content += "  " + ticket.ticketNumber + "     (" + ticket.numberOfShips + ")     " + ticket.winnings + "\r\n";
                    totalPayout += ticket.winnings;
                    totalShips += ticket.numberOfShips;
                }
            }

            content += "\r\n\r\n Total bought " + totalShips;
            content += "\r\n Total payout " + totalPayout;

            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] buffer = utf8.GetBytes(content);
            stream.Write(buffer, 0, buffer.Length);

            stream.Close();
        }
        catch (Exception e)
        {
            if (e != null) { }

            Statics.LogDebug("Could not open " + saveFile);
        }


        // Reset game
        cam.transform.parent = transform;
        Destroy(system.gameObject);
        system = Instantiate(systemPrefab);
        system.controller = this;
        system.GetComponent<GPS>().cam = cam;
        StartPlaying();
    }

}
