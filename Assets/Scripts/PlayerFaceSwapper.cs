using UniRx;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerFaceSwapper : MonoBehaviour {

    public List<Sprite> PlayerFaces = new List<Sprite>();
    public SpriteRenderer FaceSpriteRenderer;

    void Start()
    {
        MessageBroker.Default.Receive<PlayerChoosedExpression>().Subscribe(msg =>
        {
            FaceSpriteRenderer.sprite = PlayerFaces[(int)msg.FacialExpression];
        }
        
        );
    }
}
