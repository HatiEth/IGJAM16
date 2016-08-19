using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ControllerInput : MonoBehaviour {

    public GameObject[] buttons;
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExecuteEvents.Execute<IPointerDownHandler>(buttons[0], new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[0], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            ExecuteEvents.Execute<IPointerDownHandler>(buttons[1], new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[1], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            ExecuteEvents.Execute<IPointerDownHandler>(buttons[2], new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[2], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            ExecuteEvents.Execute<IPointerDownHandler>(buttons[3], new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute<IPointerClickHandler>(buttons[3], new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        }
}
