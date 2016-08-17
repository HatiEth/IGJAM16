using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Clock : MonoBehaviour {

    public GameObject clockRotator;
    public GameObject clockFill;

	public void UpdateClock (float percentRemaining) {
        clockFill.GetComponent<Image>().fillAmount = percentRemaining;
        clockRotator.transform.eulerAngles = new Vector3(0, 0, percentRemaining * -360.0f);
	}
}
