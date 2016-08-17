using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class FaceGameState : MonoBehaviour {

	private CompositeDisposable _disposeables = new CompositeDisposable();

	public int PersonExpectedExpression;
	public int YourCurrentExpression;

	public Subject<bool> m_MatchedExpression;

	private void Start()
	{
		GenerateExpectedExpression();
		m_MatchedExpression = new Subject<bool>();

		m_MatchedExpression.Subscribe((gotItRight) =>
		{
			GenerateExpectedExpression();
		}).AddTo(_disposeables);

	}

	public void OnDestroy()
	{
		_disposeables.Dispose();
	}

	public bool CalculateFacialExpressions()
	{
		return (PersonExpectedExpression == YourCurrentExpression);
	}

	public void SelectYourExpression(int expressionId)
	{
		YourCurrentExpression = expressionId;
		m_MatchedExpression.OnNext(CalculateFacialExpressions());
	}

	public void GenerateExpectedExpression()
	{
		PersonExpectedExpression = Random.Range(0, 7);
	}
}
