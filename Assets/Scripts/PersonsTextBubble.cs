using UnityEngine;
using System.Collections;

public class PersonsTextBubble : MonoBehaviour {
	private Textwriter AssociatedTextWriter;

	void Start()
	{
		AssociatedTextWriter = GetComponentInChildren<Textwriter>();
	}

	public void StartText(string TargetText)
	{

		AssociatedTextWriter.StartText(TargetText);
	}


}
