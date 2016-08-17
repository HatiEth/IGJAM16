using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class SelectExpressionButton : MonoBehaviour {

	public Faces Expression = Faces.Anger;

	private FaceGameState m_FaceGame;

	// Use this for initialization
	void Start () {
		m_FaceGame = FindObjectOfType<FaceGameState>();

		GetComponent<Image>().color = m_FaceGame.FacialExpressionColors[(int)Expression];

		GetComponent<Button>().OnClickAsObservable().Subscribe((_) =>
		{
			m_FaceGame.SelectYourExpression(Expression);
		}).AddTo(this.gameObject);
	}
	
	
}
