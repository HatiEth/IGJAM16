using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class FaceGameState : MonoBehaviour {

	private CompositeDisposable _disposeables = new CompositeDisposable();

	[HideInInspector]
	public int PersonExpectedExpression;
	[HideInInspector]
	public int YourCurrentExpression;

	[Tooltip("Total time of party left in seconds")]
	/* 
	 * Total time of party left in seconds 
	 */
	public float PartyTimeSeconds;

	public ReactiveProperty<float> m_pPartyTimerS;
	public float PartyTimerS
	{
		get { return m_pPartyTimerS.Value; }
		set { m_pPartyTimerS.Value = value; }
	}

	public Subject<bool> m_MatchedExpression;

	private void Awake()
	{
		m_pPartyTimerS = new ReactiveProperty<float>(PartyTimeSeconds);
		m_MatchedExpression = new Subject<bool>();
	}


	private void Start()
	{
		
		GenerateExpectedExpression();
		

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

	void Update()
	{
		PartyTimerS -= Time.deltaTime;
	}
}
