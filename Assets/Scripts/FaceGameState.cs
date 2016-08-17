using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;

public class FaceGameState : MonoBehaviour {

	private CompositeDisposable _disposeables = new CompositeDisposable();

	public Transform PersonStartPosition;
	public Transform PersonMidPosition;
	public Transform PersonExitPosition;

	public GameObject PersonPrefab;
	[Tooltip("Total number of other persons on party")]
	public int NumberOfPersons = 7;
	public Person[] PartyPeople;

	public Person CurrentPerson;

	[HideInInspector]
	public Faces YourCurrentExpression;

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
	public Subject<Person> PersonArrived;
	public Subject<Person> PersonExited;

	private void Awake()
	{
		m_pPartyTimerS = new ReactiveProperty<float>(PartyTimeSeconds);

		PersonArrived = new Subject<Person>();
		PersonExited = new Subject<Person>();

		m_MatchedExpression = new Subject<bool>();
		PartyPeople = new Person[NumberOfPersons];
		for(int i=0;i<NumberOfPersons;++i)
		{
			var PartyPersonGO = GameObject.Instantiate(PersonPrefab, PersonStartPosition.position, Quaternion.identity) as GameObject;
			PartyPeople[i] = PartyPersonGO.GetComponent<Person>();
		}

		PersonArrived.Subscribe((person) =>
		{
			// @TOOD: Initiate Person Logic

			person.InitiateMoveTo(PersonExitPosition.position, 1.5f, PersonExited);
		}).AddTo(this.gameObject);

		PersonExited.Subscribe((person) =>
		{
			person.transform.position = PersonStartPosition.position;

			SelectNextPerson();
		}).AddTo(this.gameObject);

	}

	private void Start()
	{

		m_MatchedExpression.Subscribe((gotItRight) =>
		{
			GenerateExpectedExpression();
		}).AddTo(_disposeables);

		SelectNextPerson();
	}

	public void SelectNextPerson()
	{
		CurrentPerson = PartyPeople[Random.Range(0, NumberOfPersons - 1)];
		GenerateExpectedExpression();

		CurrentPerson.InitiateMoveTo(PersonMidPosition.position, 2.5f, PersonArrived);
	}

	public void OnDestroy()
	{
		_disposeables.Dispose();
	}

	public bool CalculateFacialExpressions()
	{
		return (CurrentPerson.RequiredFaceExpression == YourCurrentExpression);
	}

	public void SelectYourExpression(Faces expression)
	{
		YourCurrentExpression = expression;
		m_MatchedExpression.OnNext(CalculateFacialExpressions());
	}

	public void GenerateExpectedExpression()
	{
		CurrentPerson.RequiredFaceExpression = (Faces)(Random.Range(0, System.Enum.GetNames(typeof(Faces)).Length));
	}

	void Update()
	{
		PartyTimerS -= Time.deltaTime;
	}
}
