using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ControllerInput : MonoBehaviour {

    public GameObject[] buttons;
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            ExecuteEvents.Execute<IPointerEnterHandler>(buttons[0], new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[0], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute<IPointerExitHandler>(buttons[0], new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            Debug.Log("AButton");
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[1], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            Debug.Log("BButton");
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[2], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            Debug.Log("XButton");
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[3], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            Debug.Log("YButton");
        }
        }
}
