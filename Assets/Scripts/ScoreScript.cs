using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public int score = 0;
    private int prevScore = 0;
    public Text scoreNumber;

    void Start()
    {
        
    }

    void Update()
    {
        if (prevScore != score)
        {
            scoreNumber.text = score.ToString();
        }
        prevScore = score;
    }
}
