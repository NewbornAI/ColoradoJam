using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DysonSwarm;

public class SystemController : MonoBehaviour
{

    public SolarSystem system;
    public UIHandler UI;



    // Start is called before the first frame update
    void Start()
    {
        StartPlaying();
    }

    public void StartPlaying()
    {
        system.StartPlaying();
    }




















}
