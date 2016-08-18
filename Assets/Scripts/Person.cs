using UniRx;
using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {
	public Sprite[] FacialExpressions = new Sprite[System.Enum.GetNames(typeof(Faces)).Length];
	[Tooltip("Minimum questions a person may ask")]
	public int MinQuestionCount = 0;
	[Tooltip("Maximum amount of questions the person will ask")]
	public int MaxQuestionCount = 1;

	public SpriteRenderer HeadSlot;
	public SpriteRenderer HairSlot;
	public SpriteRenderer FacialSlot;
	public SpriteRenderer BodySlot;

	public Sprite[] HeadTypes;
	public Sprite[] HairTypes;
	public Sprite[] FacialTypes;
	public Sprite[] BodyTypes;

	[ReadOnly]
	public int QuestionCount = 0;

	[ReadOnly]
	public Faces RequiredFaceExpression;
	[ReadOnly]
	public string Name;
	[ReadOnly]
	public bool HasMet;

	PersonsTextBubble TextBubble;

	public AnimationCurve PersonWalkAnimationProgress;

	void Start()
	{
		int idxOfFaceExpression = (Random.Range(0, System.Enum.GetNames(typeof(Faces)).Length));
		RequiredFaceExpression = (Faces)(idxOfFaceExpression);
		GetComponent<SpriteRenderer>().sprite = FacialExpressions[idxOfFaceExpression];

		Name = GenerateName();

		HairSlot.sprite = HairTypes[Random.Range(0, HairTypes.Length - 1)];
		HeadSlot.sprite = HeadTypes[Random.Range(0, HeadTypes.Length - 1)];
		FacialSlot.sprite = FacialTypes[Random.Range(0, FacialTypes.Length - 1)];
		BodySlot.sprite = BodyTypes[Random.Range(0, BodyTypes.Length - 1)];

		TextBubble = FindObjectOfType<PersonsTextBubble>();
	}

	public void InitiateMoveTo(Vector3 target, float travelTime, Subject<Person> onArrival)
	{
		// MainThreadDispatcher.StartUpdateMicroCoroutine(MoveTo(target, travelTime, onArrival));

		//@TODO: Add wobble
	}



	public IEnumerator MoveTo(Vector3 target, float time, IObserver<bool> observer, CancellationToken cancellationToken)
	{
		Vector3 startPosition = transform.position;

		float AlphaTime = 0f;
		while (AlphaTime <= time)
		{
			transform.position = startPosition + (target - startPosition) * PersonWalkAnimationProgress.Evaluate(Mathf.Clamp01(AlphaTime / time));

			yield return null;
			AlphaTime += Time.deltaTime;
		}
		observer.OnNext(true);
		observer.OnCompleted();
	}

	public IEnumerator AwaitExpression()
	{
		yield return null;
	}

	public void InitiateAskQuestion(Subject<Person> OnQuestionEnd)
	{
		MainThreadDispatcher.StartUpdateMicroCoroutine(AskQuestion(OnQuestionEnd));
	}

	public IEnumerator AskQuestion(Subject<Person> OnQuestionEnd)
	{
		TextBubble.StartText("asdkjasndjasbduasjknfhasbklfa ahdbasjdbasuzdkbas abhsdnasdjnabshj bjaskdnasalnsdkasl najsdbasdbka hsfbsajf sad");

		yield return null;

		OnQuestionEnd.OnNext(this);
	}

	//public IObservable<bool> DoQuestions()
	//{
	//}

	public void GenerateQuestions()
	{
		QuestionCount = Random.Range(MinQuestionCount, MaxQuestionCount);
	}

	private string GenerateName()
	{
		var path = Application.streamingAssetsPath + "/names.txt";



		return ("");
	}
}
