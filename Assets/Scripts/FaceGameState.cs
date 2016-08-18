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

	public float HeartMeterStart;
	public ReactiveProperty<float> m_pHeartMeter;
	public float HeartMeter { get { return m_pHeartMeter.Value; } set { m_pHeartMeter.Value = value; } }

	public GameObject PersonPrefab;
	[Tooltip("Total number of other persons on party")]
	public int NumberOfPersons = 7;

	private AudioSource m_ASource;
	public AudioClip SFX_Scored;
	public AudioClip SFX_Failed;
	public AudioClip SFX_Selected;


	[ReadOnly]
	public Faces[] AssociatedFaces = new Faces[System.Enum.GetNames(typeof(BodyTypes)).Length];
	[ReadOnly]
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

	private void Awake()
	{
		m_ASource = GetComponent<AudioSource>();
		m_pPartyTimerS = new ReactiveProperty<float>(PartyTimeSeconds);
		
		PartyPeople = new Person[NumberOfPersons];
		for(int i=0;i<NumberOfPersons;++i)
		{
			var PartyPersonGO = GameObject.Instantiate(PersonPrefab, PersonStartPosition.position, Quaternion.identity) as GameObject;
			PartyPeople[i] = PartyPersonGO.GetComponent<Person>();
			//@TODO Proper randomize
			PartyPeople[i].BodySlot.sprite = PartyPeople[i].BodyTypes[i % PartyPeople[i].BodyTypes.Length];
		}

		MessageBroker.Default.Receive<PlayerChoosedExpression>().Subscribe(msg =>
		{
			m_ASource.PlayOneShot(SFX_Selected);
		}).AddTo(this.gameObject);
	}

	Subject<Person> PersonStream = new Subject<Person>();

	private IObservable<Person> GeneratePersonObservable(Person p)
	{
		return Observable.FromCoroutine<bool>((observer, cancelToken) => CurrentPerson.MoveTo(this.PersonExitPosition.position, 2.5f, observer, cancelToken))
			.Do((_) => MessageBroker.Default.Publish(new PersonReady { Person = p }))
			.SelectMany(MessageBroker.Default.Receive<PlayerChoosedExpression>())	// wait for player
			.First()
			.Do((_) => p.HasMet = true)
			.Do(expr =>
			{
				if (CurrentPerson.RequiredFaceExpression == expr.FacialExpression)
				{
					// @TODO: Score here, Calculate Sweetspot
					m_ASource.PlayOneShot(SFX_Scored);
				}
				else
				{
					ScreenShakeService.Instance.Amplify(0.3f);
					m_ASource.PlayOneShot(SFX_Failed);
				}
			})
			.Select(_ => p)
			// @TODO: Start Question observable
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
