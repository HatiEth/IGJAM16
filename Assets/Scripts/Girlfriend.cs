using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Girlfriend : MonoBehaviour {

	[HideInInspector]
	public HashSet<System.Type> PersonsMet = new HashSet<System.Type>();

	public bool HasMet(System.Type PersonType)
	{
		return (PersonsMet.Contains(PersonType));
	}

	public void Meet(System.Type PersonType)
	{
		PersonsMet.Add(PersonType);
	}
}
