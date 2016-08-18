using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FaceGameState : MonoBehaviour {
	public Color[] FacialExpressionColors = new Color[System.Enum.GetNames(typeof(Faces)).Length];
	private CompositeDisposable _disposeables = new CompositeDisposable();
	private CompositeDisposable _coreloop = new CompositeDisposable();

	public Transform PersonStartPosition;
	public Transform PersonMidPosition;
	public Transform PersonExitPosition;

	public GameObject PersonPrefab;
	[Tooltip("Total number of other persons on party")]
	public int NumberOfPersons = 7;
	

	public Person[] PartyPeople;


	public Person CurrentPerson;

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
	public Subject<Person> PersonQuestions;

	ReactiveCommand SelectedFacialExpression = new ReactiveCommand();
	private void Awake()
	{
		m_pPartyTimerS = new ReactiveProperty<float>(PartyTimeSeconds);
		
		PartyPeople = new Person[NumberOfPersons];
		for(int i=0;i<NumberOfPersons;++i)
		{
			var PartyPersonGO = GameObject.Instantiate(PersonPrefab, PersonStartPosition.position, Quaternion.identity) as GameObject;
			PartyPeople[i] = PartyPersonGO.GetComponent<Person>();
		}
	}

	Subject<Person> PersonStream = new Subject<Person>();

	private IObservable<Person> GeneratePersonObservable(Person p)
	{
		return Observable.FromCoroutine<bool>((observer, cancelToken) => CurrentPerson.MoveTo(this.PersonMidPosition.position, 1.5f, observer, cancelToken))
			.Do((_) => MessageBroker.Default.Publish(new PersonReady { Person = p }))
			.SelectMany(MessageBroker.Default.Receive<PlayerChoosedExpression>())
			.First()
			.Do((_) => p.HasMet = true)
			.Do(expr =>
			{
				Debug.Log(expr);
				if (CurrentPerson.RequiredFaceExpression == expr.FacialExpression)
				{
					// @TODO: Score here
				}
			})
			.Select(_ => p)
			// @TODO: Start Question observable
			.SelectMany(Observable.FromCoroutine<bool>((observer, cancelToken) => CurrentPerson.MoveTo(this.PersonExitPosition.position, 1.5f, observer, cancelToken)))
			.Select(_ => p)
			;
	}

	private void Start()
	{
		SelectNextPerson();

		var CoreLoop = PersonStream
			.SelectMany(person => GeneratePersonObservable(person))
			.Select(person => person.transform.position = PersonStartPosition.transform.position)
			.Do(_ => SelectNextPerson())
			.Do(_ => PersonStream.OnNext(CurrentPerson))
			;

		var coreloop_cancel = CoreLoop.Subscribe();
		coreloop_cancel.AddTo(this.gameObject);

		PersonStream.OnNext(CurrentPerson);

		// If Time <= 0 - we complete the person stream
		// m_pPartyTimerS.Where(remainingTime => remainingTime < 0).Subscribe(_ => PersonStream.OnCompleted());
	}

	public void SelectNextPerson()
	{
		CurrentPerson = PartyPeople[Random.Range(0, NumberOfPersons - 1)];
		CurrentPerson.GenerateQuestions();
	}

	public void OnDestroy()
	{
		_disposeables.Dispose();
		_coreloop.Dispose();
	}

	public void SelectYourExpression(Faces expression)
	{
		MessageBroker.Default.Publish(new PlayerChoosedExpression { FacialExpression = expression });
	}

	void Update()
	{
		PartyTimerS -= Time.deltaTime;
	}
}
