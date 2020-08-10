using System.Collections.Generic;
using UnityEngine;
using coloradoJam;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;

public class BuyerUI : MonoBehaviour
{
    public Tickets bets = new Tickets();

    public Text entries;
    public Dropdown team;
    public Text ships;
    public Button validate;

    // Start is called before the first frame update
    void Start()
    {
        ResetEntries();
        //Ticket ticket;
        //for (int i = 0; i < 100; i++)
        //{
        //    ticket = new Ticket();
        //    int team = Random.Range(0, 4);
        //    switch (team)
        //    {
        //        case 0:
        //            ticket.team = Teams.Blue;
        //            break;
        //        case 1:
        //            ticket.team = Teams.Green;
        //            break;
        //        case 2:
        //            ticket.team = Teams.Red;
        //            break;
        //        case 3:
        //            ticket.team = Teams.Yellow;
        //            break;

        //    }
        //    ticket.numberOfShips = Random.Range(1, 100);
        //    ticket.ticketNumber = i;
        //    bets.list.Add(ticket);
        //}

        //SaveBets();
    }


    public void SaveBets()
    {
        try
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).ToString() + "/Tickets.bin";

            if (!File.Exists(path)) // New game started
            {
                Ticket last = bets.list[bets.list.Count - 1];
                bets.list.Clear();
                last.ticketNumber = 0;
                bets.list.Add(last);

                ResetEntries();
                AddEntry(last);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, bets);
            stream.Close();
        }
        catch (System.Exception e)
        {
            if (e != null) { }

            Statics.LogDebug("Could not write Tickets.bin");
        }
    }


    public void ValidateButton()
    {
        Ticket ticket = new Ticket(); 

        switch (team.value)
        {
            case 0:
                ticket.team = Teams.Blue;
                break;
            case 1:
                ticket.team = Teams.Green;
                break;
            case 2:
                ticket.team = Teams.Red;
                break;
            case 3:
                ticket.team = Teams.Yellow;
                break;

        }

        int numberOfShips;
        if (Int32.TryParse(ships.text, out numberOfShips))
        {
            ticket.numberOfShips = numberOfShips;
            ticket.ticketNumber = bets.list.Count;
            bets.list.Add(ticket);

            AddEntry(ticket);
            SaveBets();

        }
    }


    public void ResetEntries()
    {
        entries.text = "Tickets\n";
    }

    public void AddEntry(Ticket ticket)
    {
        entries.text += ticket.ticketNumber + " : " + ticket.numberOfShips + "$ (" + ticket.team.ToString() + ")\n";
    }
}
