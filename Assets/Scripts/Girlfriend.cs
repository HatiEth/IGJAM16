using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Girlfriend : MonoBehaviour {

	public Sprite[] FacialExpressions = new Sprite[System.Enum.GetNames(typeof(Faces)).Length];

	void Start()
	{
		Text textBox = GetComponent<Text>();

		MessageBroker.Default.Receive<PersonReady>().Subscribe(msg =>
		{
			if(!msg.Person.HasMet)
			{
				textBox.text = "be " + msg.Person.RequiredFaceExpression;

				// GetComponent<SpriteRenderer>().sprite = FacialExpressions[(int)msg.Person.RequiredFaceExpression];
			}

		}).AddTo(this.gameObject);
	}
}
