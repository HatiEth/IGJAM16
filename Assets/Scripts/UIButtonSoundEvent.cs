 using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.EventSystems;
 using System.Collections;
 
 public class UIButtonSoundEvent : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public void OnPointerEnter(PointerEventData ped)
    {
      //Soundmanager.instance.RandomizeSfx(hoverSound);
    }

    public void OnPointerDown(PointerEventData ped)
    {
        //Soundmanager.instance.RandomizeSfx(clickSound);
    }
}
