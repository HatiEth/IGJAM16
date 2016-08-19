using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HintBubble : MonoBehaviour {



    // Use this for initialization
    void Start()
    {
        MessageBroker.Default.Receive<ExpressionMade>().Subscribe(inf =>
        {
            if (inf.WasMiss || inf.HitPerson == null || inf.PlayerExpression != inf.HitPerson.RequiredFaceExpression)
            {

            }
        }
            );
     }

}
