using UnityEngine;
using System.Collections;

public class TestFace : MonoBehaviour {

    public AnimationCurve TestLerp;
    public Color BaseColor;
    public Color Goal;

	// Use this for initialization
	void Start () {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float AlphaTime = 0f;

        while (AlphaTime < 2f)
        {
            float Alpha = TestLerp.Evaluate(AlphaTime);
            GetComponent<SpriteRenderer>().color = Color.Lerp(BaseColor, Goal, Alpha);
            yield return null;
            AlphaTime += Time.deltaTime;
        }
    }

	// Update is called once per frame
	void Update () {
	    
	}
}
