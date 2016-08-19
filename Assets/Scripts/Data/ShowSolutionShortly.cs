using UnityEngine;
using UniRx;
using System.Collections;

public class ShowSolutionShortly : MonoBehaviour {

	public float WaitForTime = 1.5f;
	public AnimationCurve FadeCurve;

	// Use this for initialization
	void Start () {
		this.transform.localScale = Vector3.zero;

		MessageBroker.Default.Receive<ExpressionMade>().Subscribe(msg =>
		{
			if(msg.WasFail)
			{
				StartCoroutine(ShowSolution(msg.RequiredExpression, 0.5f));
			}
		}).AddTo(this.gameObject);
	}
	
	IEnumerator ShowSolution(Faces expr, float duration)
	{
		float AlphaTime = 0f;
		while(AlphaTime <= duration)
		{
			this.transform.localScale = new Vector3(-1, 1, 1) * FadeCurve.Evaluate(Mathf.Clamp01(AlphaTime / duration));
			yield return null;
			AlphaTime += Time.deltaTime;
		}

		yield return new WaitForSeconds(WaitForTime);

		while (AlphaTime >= 0)
		{
			this.transform.localScale = new Vector3(-1, 1, 1) * FadeCurve.Evaluate(Mathf.Clamp01(AlphaTime / duration));
			yield return null;
			AlphaTime -= Time.deltaTime;
		}
	}
}
