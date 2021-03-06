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
	public Vector2 BoxCastSize = Vector2.one;

	public float HeartMeterStart = 1f;
	public ReactiveProperty<float> m_pHeartMeter;
	public float HeartMeter { get { return m_pHeartMeter.Value; } set { m_pHeartMeter.Value = value; } }

	[Range(0f, 1f)]
	public float HeartMeterDrainPerSecond = .25f;

	[Range(0f, 1f)]
	public float HeartMeterForPerfect = 0.4f;
	[Range(0f, 1f)]
	public float HeartMeterForGood = 0.3f;

	[Range(0f, 1f)]
	public float DrainAddedPerIncrease = 0.02f;

	[Range(0f, 1f)]
	public float MAX_HeartMeterDrain = 1f;


	public GameObject PersonPrefab;
	[Tooltip("Total number of other persons on party")]
	public int NumberOfPersons = 7;


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

	public float IncreaseDifficultyAfterSeconds = 30f;

	public Subject<bool> m_MatchedExpression;
	public Subject<Person> PersonArrived;
	public Subject<Person> PersonExited;
	public Subject<Person> PersonQuestions;

	public int CurrentMultiplier = 1;
	public int ScorePerPerson = 10;

	[Range(0f, 1f)]
	public float GoodSection = .2f;

	[Range(0f, 1f)]
	public float PerfectSection = .2f;

	private int EasyMode = 3;

	private ReactiveProperty<int> m_pScore = new ReactiveProperty<int>(0);
	public int Score { get { return m_pScore.Value; } set { m_pScore.Value = value; } }

	private void GeneratedAssociatedFaces()
	{
		var EnumValues = System.Enum.GetValues(typeof(Faces));
		List<Faces> EnumValuesInt = new List<Faces>(EnumValues.OfType<Faces>());

		for (int i = 0; i < AssociatedFaces.Length; ++i)
		{
			if (EnumValuesInt.Count == 0)
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

		m_pHeartMeter = new ReactiveProperty<float>(HeartMeterStart);

		m_pScore.Subscribe(score =>
		{
			MessageBroker.Default.Publish(new ScoreChanged { CurrentScore = score });
		}).AddTo(this.gameObject);

		GeneratedAssociatedFaces();


		PartyPeople = new List<Person>(NumberOfPersons);
		for (int i = 0; i < NumberOfPersons; ++i)
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
		while (PartyPeople.Count > StayingPeopleCounter)
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

		Score = 0;
		StayingPeopleCounter = NumberOfPersons - 1;
		var MoveStream = PersonStream
			.SelectMany((p) => MovePersonToExit(p))
			.Delay(System.TimeSpan.FromSeconds(0.25))
			.Do(p => PartyPeople.Add(p))
			.Do(p => p.transform.position = PersonStartPosition.position)
			.Do(p => p.WasFaced = false)
			.Subscribe()
			;

		var delaySelect = Observable
			.FromCoroutine<Person>(observer => TakeUntilWalkwayFull(0.25f, 0.75f, observer))
			.Do(p => MessageBroker.Default.Publish(new PersonReady { Person = p }))
			.RepeatUntilDestroy(this);

		delaySelect.Subscribe(x => PersonStream.OnNext(x)); // Add selected person to PersonStream

		m_pHeartMeter.Subscribe(heart =>
		{
			MessageBroker.Default.Publish(new HeartChanged { CurrentHeart = heart });
			if(heart <= 0)
			{
				SceneManager.LoadScene("GameOverScene");
			}
		}).AddTo(this.gameObject);


		// If Time <= 0 - we complete the person stream
		// m_pPartyTimerS.Where(remainingTime => remainingTime < 0).Subscribe(_ => PersonStream.OnCompleted());

		Observable.Timer(System.TimeSpan.FromSeconds(IncreaseDifficultyAfterSeconds))
			.RepeatUntilDestroy(this)
			.Subscribe(_ =>
			{
				if(EasyMode>0)
				{
					++StayingPeopleCounter;
					EasyMode--;
				}

				var PartyPersonGO = GameObject.Instantiate(PersonPrefab, PersonStartPosition.position, Quaternion.identity) as GameObject;
				var p = PartyPersonGO.GetComponent<Person>();
				PartyPeople.Add(p);
				//@TODO Proper randomize
				int bodyIdx = Random.Range(0, System.Enum.GetValues(typeof(BodyTypes)).Length - 1);
				p.GenerateByTypes(AssociatedFaces[bodyIdx], (BodyTypes)(bodyIdx));

				HeartMeterDrainPerSecond = Mathf.Min(HeartMeterDrainPerSecond + DrainAddedPerIncrease, MAX_HeartMeterDrain);
			}).AddTo(this.gameObject);

		MessageBroker.Default.Receive<ExpressionMade>().Subscribe(msg =>
		{
			CurrentMultiplier = msg.CurrentCombo;
			if(!msg.WasMiss)
			{
				HeartMeter = Mathf.Clamp01(HeartMeter + (msg.WasPerfect ? HeartMeterForPerfect : HeartMeterForGood));
			}
		}).AddTo(this.gameObject);
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


		int hits = Physics2D.BoxCastNonAlloc(BoxCastOrigin.position, BoxCastSize, 0f, Vector2.zero, BoxCastResults);
		if (hits >= 0)
		{
			float Distance = float.MaxValue;
			int idx = -1;
			for (int i = 0; i < hits; ++i)
			{
				var person = BoxCastResults[i].transform.GetComponent<Person>();
				if (person 
					&& (BoxCastOrigin.position.x - person.transform.position.x) > 0 
					&& (BoxCastOrigin.position.x - person.transform.position.x) < Distance // right most distance
					&& !person.WasFaced
				)
				{
					Distance = (BoxCastOrigin.position.x - person.transform.position.x);
					idx = i;
				}
			}

			if (idx != -1)
			{
				var person = BoxCastResults[idx].transform.GetComponent<Person>();
				bool wasInside = Mathf.Abs(BoxCastOrigin.position.x - person.transform.position.x) < (BoxCastSize.x * GoodSection);

				person.WasFaced = true;

				if (person.RequiredFaceExpression == expression && wasInside)
				{
					m_ASource.PlayOneShot(SFX_Perfect);
					Score += ScorePerPerson * CurrentMultiplier;
					

					bool wasPerfect = Mathf.Abs(BoxCastOrigin.position.x - person.transform.position.x) < (BoxCastSize.x * PerfectSection);

					MessageBroker.Default.Publish(new ExpressionMade
					{
						HitPerson = person,
						CurrentCombo = CurrentMultiplier+1,
						WasPerfect = wasPerfect,
						PlayerExpression = expression
					});
				}
				else
				{
					m_ASource.PlayOneShot(SFX_Fail);
					MessageBroker.Default.Publish(new ExpressionMade
					{
						PlayerExpression = expression,
						WasFail = true,
						RequiredExpression = person.RequiredFaceExpression
					});
				}
			}
			else
			{
				m_ASource.PlayOneShot(SFX_Fail);
				MessageBroker.Default.Publish(new ExpressionMade
				{
					PlayerExpression = expression,
				});
			}
		}
		else	// there is no person
		{
			m_ASource.PlayOneShot(SFX_Fail);
			MessageBroker.Default.Publish(new ExpressionMade
			{
				PlayerExpression = expression
			});
		}
	}

	void Update()
	{
		PartyTimerS += Time.deltaTime;
		HeartMeter -= HeartMeterDrainPerSecond * Time.deltaTime;

	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(BoxCastOrigin.position, BoxCastSize);
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(BoxCastOrigin.position, Vector2.Scale(BoxCastSize, new Vector2(GoodSection, 1f)));
		Gizmos.color = Color.green;
		Gizmos.DrawCube(BoxCastOrigin.position, Vector2.Scale(BoxCastSize, new Vector2(PerfectSection, 1f)));
	}
}
