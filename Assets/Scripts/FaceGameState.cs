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

	private AudioSource m_ASource;
	public AudioClip SFX_Perfect;
	public AudioClip SFX_Fail;

	public Transform BoxCastOrigin;

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
		m_ASource = GetComponent<AudioSource>();
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

	private IEnumerator TakeUntilWalkwayFull(float minWaitTime, float maxWaitTime, IObserver<Person> observer)
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

	RaycastHit2D[] BoxCastResults = new RaycastHit2D[32];

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
			.FromCoroutine<Person>(observer => TakeUntilWalkwayFull(0.75f, 1.5f, observer))
			.Do(p => MessageBroker.Default.Publish(new PersonReady { Person = p }))
			.RepeatUntilDestroy(this);

		delaySelect.Subscribe(x => PersonStream.OnNext(x)); // Add selected person to PersonStream



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


		int hits = Physics2D.BoxCastNonAlloc(BoxCastOrigin.position, new Vector2(1.0f, 1.0f), 0f, Vector2.zero, BoxCastResults);
		if (hits >= 0)
		{
			float Distance = float.MaxValue;
			int idx = -1;
			for (int i=0;i<hits;++i)
			{
				if(BoxCastResults[i].transform.GetComponent<Person>() && BoxCastResults[i].distance < Distance)
				{
					Distance = BoxCastResults[i].distance;
					idx = i;
				}
			}
			
			if(idx != -1)
			{
				var person = BoxCastResults[idx].transform.GetComponent<Person>();
				if(person.RequiredFaceExpression == expression)
				{
					m_ASource.PlayOneShot(SFX_Perfect);
				}
				else
				{
					m_ASource.PlayOneShot(SFX_Fail);
				}
			}
			else
			{
				m_ASource.PlayOneShot(SFX_Fail);
			}
		}
		else
		{
			m_ASource.PlayOneShot(SFX_Fail);
		}
	}

	void Update()
	{
		PartyTimerS += Time.deltaTime;
	}
}
