using UnityEngine;
using UniRx;
using System.Collections;

public class HeartMeterFillImage : MonoBehaviour {

	public void Start()
	{
		MessageBroker.Default.Receive<HeartChanged>().Subscribe(msg =>
		{

		}).AddTo(this.gameObject);
	}
}
