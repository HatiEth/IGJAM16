using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class FlashFeedback : MonoBehaviour {
    
    [Header("Particles")]
    public ParticleSystem ScoreFX_PerfectGO;
    public ParticleSystem ScoreFX_GoodGO;
    public ParticleSystem ScoreFX_BadGO;

    [Header("Colors")]
    public Color Col_Start;
    public Color Col_Perfect = Color.green;
    public Color Col_Good = Color.yellow;
    public Color Col_Bad = Color.red;

    public AnimationCurve FlashCurve;

    public Image ImgToFlash;

    private Coroutine cor;

    // Use this for initialization
    void Start () {

        MessageBroker.Default.Receive<ExpressionMade>().Subscribe(inf =>
        {
            if (inf.WasMiss || inf.HitPerson == null)
            {
                ScoreFX_BadGO.Play();

                if (cor != null) StopCoroutine(cor);

                cor = StartCoroutine(Fade(Col_Start, Col_Bad, ImgToFlash));
            }
            else
            if (inf.WasPerfect)
            {
                ScoreFX_PerfectGO.Play();
                if (cor != null) StopCoroutine(cor);

                cor = StartCoroutine(Fade(Col_Start, Col_Perfect, ImgToFlash));
            }
            else
            {
                ScoreFX_GoodGO.Play();
                if (cor != null) StopCoroutine(cor);

                cor = StartCoroutine(Fade(Col_Start, Col_Good, ImgToFlash));
            }

        }
        );

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MessageBroker.Default.Publish<ExpressionMade>(new ExpressionMade { HitPerson = new Person(), WasPerfect = true });
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            MessageBroker.Default.Publish<ExpressionMade>(new ExpressionMade { HitPerson = null });
        }


        if (Input.GetKeyDown(KeyCode.J))
        {
            MessageBroker.Default.Publish<ExpressionMade>(new ExpressionMade { HitPerson = new Person() });
        }
    }

    private IEnumerator Fade(Color FromCol, Color ToColor, Image img)
    {
        float time = 0;

        while (time < 1f)
        {
            float Alpha = FlashCurve.Evaluate(time);
            img.color = Color.Lerp(FromCol, ToColor, Alpha);
            yield return null;
            time += Time.deltaTime;
        }
    }

}
