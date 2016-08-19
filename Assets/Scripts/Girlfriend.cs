using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Girlfriend : MonoBehaviour {

	public RectTransform UI_TransformRoot;
	public Image GirlfriendFacialImage;
	public Sprite[] FacialExpressions = new Sprite[System.Enum.GetNames(typeof(Faces)).Length];
	public Image GirlFriendBodyTypeImage;
	public Sprite[] BodyTypeSprites = new Sprite[System.Enum.GetNames(typeof(BodyTypes)).Length];

	public AnimationCurve FadingCurve;
	public float FadeDuration = 0.2f;

	private HashSet<Faces> ExpressionTypes = new HashSet<Faces>();

	bool HasMetType(Person p)
	{
		return ExpressionTypes.Contains(p.RequiredFaceExpression);
	}

	void Start()
	{
		UI_TransformRoot.gameObject.SetActive(false);

		MessageBroker.Default.Receive<PersonReady>().Subscribe(msg =>
		{
			if (!HasMetType(msg.Person))
			{
				ExpressionTypes.Add(msg.Person.RequiredFaceExpression);
				UI_TransformRoot.gameObject.SetActive(true);
				StartCoroutine(FadeIn(FadeDuration));
				GirlfriendFacialImage.sprite = FacialExpressions[(int)msg.Person.RequiredFaceExpression];
				GirlFriendBodyTypeImage.sprite = BodyTypeSprites[(int)msg.Person.CurrentBodyType];
			}
		}).AddTo(this.gameObject);
	}

	private IEnumerator FadeIn(float duration)
	{
		float AlphaTime = 0f;

		while (AlphaTime <= duration)
		{
			UI_TransformRoot.transform.localScale = Vector3.one * FadingCurve.Evaluate(Mathf.Clamp01(AlphaTime / duration));

			yield return null;
			AlphaTime += Time.deltaTime;
		}

		yield return new WaitForSeconds(1.5f);
		yield return FadeOut(duration * 0.5f);
	}

	private IEnumerator FadeOut(float duration)
	{
		float AlphaTime = duration;

		while (AlphaTime >= 0f)
		{
			UI_TransformRoot.transform.localScale = Vector3.one * FadingCurve.Evaluate(Mathf.Clamp01(AlphaTime / duration));

			yield return null;
			AlphaTime -= Time.deltaTime;
		}
		UI_TransformRoot.gameObject.SetActive(false);
	}
}
