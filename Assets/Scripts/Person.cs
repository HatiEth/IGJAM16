using UniRx;
using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {
	public Sprite[] FacialExpressions = new Sprite[System.Enum.GetNames(typeof(Faces)).Length];
	public Font font;
	[Tooltip("Minimum questions a person may ask")]
	public int MinQuestionCount = 0;
	[Tooltip("Maximum amount of questions the person will ask")]
	public int MaxQuestionCount = 1;

	[ReadOnly]
	public int QuestionCount = 0;

	[ReadOnly]
	public Faces RequiredFaceExpression;
	[ReadOnly]
	public string Name;
	[ReadOnly]
	public bool HasMet;

	PersonsTextBubble TextBubble;

	void Start()
	{
		TextBubble = FindObjectOfType<PersonsTextBubble>();
	}

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

	public void InitiateAskQuestion(Subject<Person> OnQuestionEnd)
	{
		MainThreadDispatcher.StartUpdateMicroCoroutine(AskQuestion(OnQuestionEnd));
	}

	IEnumerator AskQuestion(Subject<Person> OnQuestionEnd)
	{
		TextBubble.StartText("asdkjasndjasbduasjknfhasbklfa ahdbasjdbasuzdkbas abhsdnasdjnabshj bjaskdnasalnsdkasl najsdbasdbka hsfbsajf sad");

		yield return null;

		OnQuestionEnd.OnNext(this);

	}

	public void GeneratePersonMood()
	{
		int idxOfFaceExpression = (Random.Range(0, System.Enum.GetNames(typeof(Faces)).Length));
		RequiredFaceExpression = (Faces)(idxOfFaceExpression);
		GetComponent<SpriteRenderer>().sprite = FacialExpressions[idxOfFaceExpression];
		QuestionCount = Random.Range(MinQuestionCount, MaxQuestionCount);
	}
}
