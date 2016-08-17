using UniRx;
using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {
	public Sprite[] FacialExpressions = new Sprite[System.Enum.GetNames(typeof(Faces)).Length];

	public Faces RequiredFaceExpression;
	public string Name;
	public bool HasMet;


	public void InitiateMoveTo(Vector3 target, float travelTime, Subject<Person> onArrival)
	{
		MainThreadDispatcher.StartUpdateMicroCoroutine(MoveTo(target, travelTime, onArrival));

		//@TODO: Add wobble
	}

	IEnumerator MoveTo(Vector3 target, float time, Subject<Person> onArrival)
	{
		Vector3 startPosition = transform.position;

		float AlphaTime = 0f;
		while (AlphaTime <= time)
		{
			transform.position = Vector3.Lerp(startPosition, target, Mathf.Clamp01(AlphaTime / time));

			yield return null;
			AlphaTime += Time.deltaTime;
		}

		onArrival.OnNext(this);
	}

	public void GenerateExpectedExpression()
	{
		int idxOfFaceExpression = (Random.Range(0, System.Enum.GetNames(typeof(Faces)).Length));
		RequiredFaceExpression = (Faces)(idxOfFaceExpression);
		GetComponent<SpriteRenderer>().sprite = FacialExpressions[idxOfFaceExpression];
	}
}
