using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayPersonExpression : MonoBehaviour {

	private FaceGameState m_FaceGameState;

	// Use this for initialization
	void Start () {
		m_FaceGameState = FindObjectOfType<FaceGameState>();
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Text>().text = "" + m_FaceGameState.CurrentPerson.RequiredFaceExpression;
	}
}
