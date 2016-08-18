using UnityEngine;
using System.Collections;

public class PersonsTextBubble : MonoBehaviour {
	private Textwriter AssociatedTextWriter;

	void Start()
	{
		AssociatedTextWriter = GetComponentInChildren<Textwriter>();
	}

	public IEnumerator StartText(string TargetText)
	{

		yield return AssociatedTextWriter.WriteText(TargetText);
	}


}
