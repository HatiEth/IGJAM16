using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {
	private Text scoreText;
	void Awake()
	{
		scoreText = GetComponent<Text>();

		MessageBroker.Default.Receive<ScoreChanged>().Subscribe(msg =>
		{
			scoreText.text = "" + msg.CurrentScore;
		}).AddTo(this.gameObject);
	}
}
