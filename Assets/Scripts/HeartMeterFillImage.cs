using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;

public class HeartMeterFillImage : MonoBehaviour {

	public void Start()
	{
		MessageBroker.Default.Receive<HeartChanged>().Subscribe(msg =>
		{
			GetComponent<Image>().fillAmount = msg.CurrentHeart;
		}).AddTo(this.gameObject);
	}
}
