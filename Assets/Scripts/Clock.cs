using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;

public class Clock : MonoBehaviour
{
    public GameObject clockRotator;
    public GameObject clockFill;
    public GameObject clockTimer;
    private FaceGameState m_FaceGame;

    // Use this for initialization
    void Start()
    {
        m_FaceGame = FindObjectOfType<FaceGameState>();

        m_FaceGame.m_pPartyTimerS.Subscribe((time) =>
        {
            clockTimer.GetComponent<Text>().text = "" + time;
            clockFill.GetComponent<Image>().fillAmount = (time / 180);
            clockRotator.transform.eulerAngles = new Vector3(0, 0, (time / 180) * 360.0f);
        }).AddTo(this.gameObject);
    }
}