using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Girlfriend : MonoBehaviour {

	public Image GirlfriendFacialSpriteRenderer;
	public Sprite[] FacialExpressions = new Sprite[System.Enum.GetNames(typeof(Faces)).Length];

	public Image[] Togglables;

	void Start()
	{
		foreach(var toggle in Togglables)
		{
			toggle.enabled = false;
		}

		MessageBroker.Default.Receive<PersonReady>().Subscribe(msg =>
		{
			if (!msg.Person.HasMet)
			{
				foreach (var toggle in Togglables)
				{
					toggle.enabled = true;
				}
				GirlfriendFacialSpriteRenderer.sprite = FacialExpressions[(int)msg.Person.RequiredFaceExpression];
				
			}
			else
			{
				foreach (var toggle in Togglables)
				{
					toggle.enabled = false;
				}
			}

		}).AddTo(this.gameObject);
	}
}
