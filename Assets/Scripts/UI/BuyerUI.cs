using System.Collections.Generic;
using UnityEngine;
using coloradoJam;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class BuyerUI : MonoBehaviour
{
    public Tickets bets = new Tickets();

    // Start is called before the first frame update
    void Start()
    {
        Ticket ticket;
        for (int i = 0; i < 10; i++)
        {
            ticket = new Ticket();
            int team = Random.Range(0, 4);
            switch (team)
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
            ticket.numberOfShips = Random.Range(1, 10);
            ticket.ticketNumber = i;
            bets.list.Add(ticket);
        }

        SaveBets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveBets()
    {
        try
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Tickets.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, bets);
            stream.Close();
        }
        catch (System.Exception e)
        {
            if (e != null) { }

            Statics.LogDebug("Could not write Tickets.bin");
        }
    }
}
