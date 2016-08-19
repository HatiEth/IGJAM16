using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;

public class DbgGamestateTimer : MonoBehaviour {

	private FaceGameState m_FaceGame;

	// Use this for initialization
	void Start () {
		m_FaceGame = FindObjectOfType<FaceGameState>();
	
		m_FaceGame.m_pPartyTimerS.Subscribe((time) =>
		{
			GetComponent<Text>().text = "" + Mathf.CeilToInt(time) + " Seconds";
		}).AddTo(this.gameObject);
	}
	
}
