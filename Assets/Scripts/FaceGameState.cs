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
	public Transform PersonExitPosition;
	public float PersonWalkDuration = 2.5f;

	public float HeartMeterStart;
	public ReactiveProperty<float> m_pHeartMeter;
	public float HeartMeter { get { return m_pHeartMeter.Value; } set { m_pHeartMeter.Value = value; } }

	public GameObject PersonPrefab;
	[Tooltip("Total number of other persons on party")]
	public int NumberOfPersons = 7;

	[ReadOnlyDuringRun]
	public Transform SweetSpot;

	[ReadOnly]
	public Faces[] AssociatedFaces = new Faces[System.Enum.GetNames(typeof(BodyTypes)).Length];
	[ReadOnly]
	public List<Person> PartyPeople;

	public Person CurrentPerson;

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


	private void GeneratedAssociatedFaces()
	{
		var EnumValues = System.Enum.GetValues(typeof(Faces));
		List<Faces> EnumValuesInt = new List<Faces>(EnumValues.OfType<Faces>());

		for(int i=0;i<AssociatedFaces.Length;++i)
		{
			if(EnumValuesInt.Count == 0)
			{
				EnumValuesInt = new List<Faces>(EnumValues.OfType<Faces>());
			}
			int randIdx = Random.Range(0, EnumValuesInt.Count - 1);
			AssociatedFaces[i] = EnumValuesInt.ElementAt(randIdx);
			EnumValuesInt.RemoveAt(randIdx);
		}
	}

	private void Awake()
	{
		m_pPartyTimerS = new ReactiveProperty<float>(0f);

		GeneratedAssociatedFaces();


		PartyPeople = new List<Person>(NumberOfPersons);
		for(int i=0;i<NumberOfPersons;++i)
		{
			var PartyPersonGO = GameObject.Instantiate(PersonPrefab, PersonStartPosition.position, Quaternion.identity) as GameObject;
			var p = PartyPersonGO.GetComponent<Person>();
			PartyPeople.Add(p);
			//@TODO Proper randomize
			p.GenerateByTypes(AssociatedFaces[i], (BodyTypes)(i));
		}
	}

	Subject<Person> PersonStream = new Subject<Person>();



	private IObservable<Person> GeneratePersonObservable(Person p)
	{
		return Observable.Timer(System.TimeSpan.FromSeconds(0.2)).AsUnitObservable()
			.Do((_) => MessageBroker.Default.Publish(new PersonReady { Person = p }))
			//.SelectMany((_) => Observable.FromCoroutine((observer) => CurrentPerson.MoveTo(this.PersonExitPosition.position, PersonWalkDuration)))
			.Select(_ => p)
			;
	}

	// Completes once the Person reached it's target
	private IObservable<Person> MovePersonToExit(Person p)
	{
		return (Observable.FromCoroutine<Person>((observer) => p.MoveTo(this.PersonExitPosition.position, PersonWalkDuration, observer)));
	}

	private int StayingPeopleCounter;

	private IEnumerator Unknown(float minWaitTime, float maxWaitTime, IObserver<Person> observer)
	{
		while(PartyPeople.Count > StayingPeopleCounter)
		{
			int PersonIdx = Random.Range(0, PartyPeople.Count - 1);
			var p = PartyPeople.ElementAt(PersonIdx);
			PartyPeople.RemoveAt(PersonIdx);
			yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
			observer.OnNext(p);
		}
		observer.OnCompleted();
	}

	private void Start()
	{
		StayingPeopleCounter = NumberOfPersons - 1;
		var MoveStream = PersonStream
			.SelectMany((p) => MovePersonToExit(p))
			.Delay(System.TimeSpan.FromSeconds(0.25))
			.Do(p => PartyPeople.Add(p))
			.Do(p => p.transform.position = PersonStartPosition.position)
			.Subscribe()
			;

		var delaySelect = Observable
			.FromCoroutine<Person>(observer => Unknown(0.75f, 1.5f, observer))
			.Do(p => MessageBroker.Default.Publish(new PersonReady { Person = p }))
			.RepeatUntilDestroy(this);

		delaySelect.Subscribe(x => PersonStream.OnNext(x)); // Add selected person to PersonStream


		var SweetSpotEnters = SweetSpot
			.OnTriggerEnter2DAsObservable()
			.SelectMany(collider => MessageBroker.Default.Receive<PlayerChoosedExpression>().Select(expr => new { collider, expr }))
			;
		SweetSpotEnters.Subscribe(x =>
		{
			if(x.collider.gameObject.GetComponent<Person>().RequiredFaceExpression == x.expr.FacialExpression)
			{

			}
		});
																					/*
		SelectNextPerson();
		PersonStream.OnNext(CurrentPerson);
		SelectNextPerson();
		PersonStream.OnNext(CurrentPerson);
		SelectNextPerson();
		PersonStream.OnNext(CurrentPerson);	 
																				*/
																					// SelectNextPerson();
																					/*
																					var CoreLoop = PersonStream
																						.SelectMany(person => GeneratePersonObservable(person))
																						.Select(person => person.transform.position = PersonStartPosition.transform.position)
																						.Do(_ => SelectNextPerson())
																						.Do(_ => PersonStream.OnNext(CurrentPerson))
																						;

																					var InputLoop = MessageBroker.Default.Receive<PlayerChoosedExpression>().AsObservable() // wait for player input
																						.Do(expr =>
																						{
																							if (CurrentPerson.RequiredFaceExpression == expr.FacialExpression)
																							{
																								Debug.Log("Hit Input");
																							}
																						})
																						// @TODO: Start Question observable
																						;

																					InputLoop.Subscribe();

																					var coreloop_cancel = CoreLoop.Subscribe();
																					coreloop_cancel.AddTo(this.gameObject);

																					PersonStream.OnNext(CurrentPerson);
																					*/

		// If Time <= 0 - we complete the person stream
		// m_pPartyTimerS.Where(remainingTime => remainingTime < 0).Subscribe(_ => PersonStream.OnCompleted());
	}

	public void SelectNextPerson()
	{
		int PersonIdx = Random.Range(0, PartyPeople.Count - 1);
		CurrentPerson = PartyPeople.ElementAt(PersonIdx);
		PartyPeople.RemoveAt(PersonIdx);

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
		PartyTimerS += Time.deltaTime;
	}
}
