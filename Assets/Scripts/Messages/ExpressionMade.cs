using UnityEngine;
using System.Collections;

public class ExpressionMade {
	public Faces PlayerExpression = Faces.Anger;
	public Person HitPerson = null;
	public int CurrentCombo = 1;
	public bool WasPerfect = false;
	public bool WasFail = false;
	public Faces RequiredExpression;

	public bool WasMiss
	{
		get { return (HitPerson == null); }
	}
}
