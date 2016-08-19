using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Thoughtbubble : MonoBehaviour {

    public string[] Thoughts;
    public Text Thought_Text;

	// Use this for initialization
	void Start ()
    {
        MessageBroker.Default.Receive<PlayerChoosedExpression>().Subscribe(msg =>
        {
            if (Random.Range(0,9) == 1 && Thoughts.Length > 0 )
            {
                
                Thought_Text.text = Thoughts[Random.Range(0, Thoughts.Length - 1)];
                GetComponent<Animator>().Play(0);
            }
           
        }
        
        );
    }
}
