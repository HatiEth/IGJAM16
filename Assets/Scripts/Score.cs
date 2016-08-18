using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {

    public Text scoreText;
    public Text timeText;

    public void UpdateScore(float score, float time)
        {
            scoreText.text = "" + score;
            timeText.text = time + " seconds"; 
        }
}
