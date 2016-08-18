using UnityEngine;
using System.Collections;
using UniRx;

public class ScreenShakeService : MonoBehaviour {

    public Transform ObjectToShake;
    public static ScreenShakeService Instance;

    private float shake = 0f;
    public float DecreaseFactor = 1f;
    public float shakeFactor = 1f;


	void Awake()
	{
		Instance = this;
	}
	// Use this for initialization
	void Start () {
		Instance = this;
	}

	void Update()
	{
			if(shake > 0f)
			{
					ObjectToShake.localPosition = Random.insideUnitSphere * shake * shakeFactor;
					shake -= Time.deltaTime * DecreaseFactor * (1.0f + shake * shakeFactor);
			}
	}
	
	public void Amplify(float shake)
	{
			this.shake += shake;
	}

	void OnDisable()
	{
			ObjectToShake.localPosition = new Vector3();
	}
}
