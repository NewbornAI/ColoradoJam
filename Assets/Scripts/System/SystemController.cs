using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DysonSwarm;
using System;

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

    }


    public void GameOver()
    {
        // Score screen

        // Save winnings in file

        // Reset game
        cam.transform.parent = transform;
        Destroy(system.gameObject);
        system = Instantiate(systemPrefab);
        system.controller = this;
        system.GetComponent<GPS>().cam = cam;
        StartPlaying();
    }

}
