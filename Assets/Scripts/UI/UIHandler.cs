using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using coloradoJam;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;

public class UIHandler : MonoBehaviour
{
    public SystemController controller;

    public Button playButton;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //
    // BUTTONS
    //

    public void PlayButton()
    {

        controller.StartPlaying();
    }



}
