using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingHUD : MonoBehaviour
{

    public Text avgScore;
    public Text simulations;

    public void OutputScore(float score)
    {
        avgScore.text = Mathf.Round(score).ToString();
    }


    public void OutputSimulations(int current, int max)
    {
        simulations.text = current + " / " + max;
    }
}
