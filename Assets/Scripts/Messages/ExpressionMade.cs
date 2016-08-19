using UnityEngine;
using System.Collections;

public class ExpressionMade {
	public Faces PlayerExpression;
	public Person HitPerson;
	public int CurrentCombo;
	public bool WasPerfect;

	public bool WasMiss
	{
		get { return (HitPerson == null); }
	}
}
