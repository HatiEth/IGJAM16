using UniRx;
using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {
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
	public Sprite[] BodyTypeSprites;


	public ReactiveProperty<bool> m_pWasFaced = new ReactiveProperty<bool>(false);
	public bool WasFaced { get { return m_pWasFaced.Value; } set { m_pWasFaced.Value = value; } }
	public Color WasFacedColor = new Color(30f / 255, 30f / 255, 30f / 255);


	[ReadOnly]
	public int QuestionCount = 0;

	[ReadOnly]
	public Faces RequiredFaceExpression;
	public BodyTypes CurrentBodyType;

	[ReadOnly]
	public bool HasMet;

	PersonsTextBubble TextBubble;

	public AnimationCurve PersonWalkAnimationProgress;

	public void GenerateByTypes(Faces F, BodyTypes B)
	{
		RequiredFaceExpression = F;
		CurrentBodyType = B;
		BodySlot.sprite = BodyTypeSprites[(int)B];
	}

	IEnumerator LerpColorSlots(Color from, Color to, float duration)
	{
		float AlphaTime = 0f;
		while(AlphaTime < duration)
		{
			var c = Color.Lerp(from, to, Mathf.Clamp01(AlphaTime / duration));

			HairSlot.color = c;
			HeadSlot.color = c;
			FacialSlot.color = c;
			BodySlot.color = c;

			yield return null;
			AlphaTime += Time.deltaTime;
		}
	}

	void Start()
	{
		HairSlot.sprite = HairTypes[Random.Range(0, HairTypes.Length - 1)];
		HeadSlot.sprite = HeadTypes[Random.Range(0, HeadTypes.Length - 1)];
		FacialSlot.sprite = FacialTypes[Random.Range(0, FacialTypes.Length - 1)];

		TextBubble = FindObjectOfType<PersonsTextBubble>();

		m_pWasFaced.Subscribe((wasFaced) =>
		{
			if(wasFaced)
			{
				MainThreadDispatcher.StartUpdateMicroCoroutine(LerpColorSlots(Color.white, WasFacedColor, 0.3f));
			}
			else
			{
				HairSlot.color = Color.white;
				HeadSlot.color = Color.white;
				FacialSlot.color = Color.white;
				BodySlot.color = Color.white;
			}
		}).AddTo(this.gameObject);
	}

	public IEnumerator MoveTo(Vector3 target, float time, IObserver<Person> observer)
	{
		Vector3 startPosition = transform.position;

		float AlphaTime = 0f;
		while (AlphaTime <= time)
		{
			transform.position = (startPosition + (target - startPosition) * PersonWalkAnimationProgress.Evaluate(Mathf.Clamp01(AlphaTime / time)));

			yield return null;
			AlphaTime += Time.deltaTime;
		}
		observer.OnNext(this);
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

	public void GenerateQuestions()
	{
		QuestionCount = Random.Range(MinQuestionCount, MaxQuestionCount);
	}

}
