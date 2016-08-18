using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayYourExpression : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MessageBroker.Default.Receive<PlayerChoosedExpression>().Subscribe(msg =>
		{
			GetComponent<Text>().text = "" + msg.FacialExpression;
		}).AddTo(this.gameObject);
	}
}
