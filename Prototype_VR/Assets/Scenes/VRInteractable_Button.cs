using UnityEngine;
using Valve.VR;

public class VRInteractable_Button : VRInteractableItem
{
    public EVRButtonId buttonToTrigger = EVRButtonId.k_EButton_SteamVR_Trigger;
    public Transform button;
    private Vector3 currentButtonDestination;
    public Vector3 buttonDownPos;
    private Vector3 buttonStartPos;
    public float buttonClickSpeed;

    public delegate void ButtonPress();
    public static event ButtonPress OnButtonPress;

    private void Start()
    {
        buttonStartPos = button.localPosition;
        currentButtonDestination = button.localPosition;
    }

    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        //If button is desired "trigger" button
        if (button == buttonToTrigger)
        {
            //Set button's destination position to the "down" position
            currentButtonDestination = buttonDownPos;

            //TriggerButtonPress();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "controller")
        {
            currentButtonDestination = buttonDownPos;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "controller")
        {
            currentButtonDestination = buttonStartPos;
        }
    }

    public override void ButtonPressUp(EVRButtonId button, ControllerInput controller)
    {
        //Set button's destination position to the "up" position
        if (button == buttonToTrigger)
        {
            currentButtonDestination = buttonStartPos;
        }
    }

    public void Update()
    {
        //Check to see if button is in the same position as its destination position
        if (button.localPosition != currentButtonDestination)
        {
            //If its not, lerp toward it at a predefined speed.
            //Remember to multiply movements in Update by Time.deltaTime, so that things don't move faster 
            //on computers with higher framerates
            Vector3 position = Vector3.MoveTowards(button.localPosition, currentButtonDestination, buttonClickSpeed * Time.deltaTime);
            button.localPosition = position;
        }
    }

    public void Awake()
    {
        VRInteractable_Button.OnButtonPress += MethodToTrigger;
    }

    protected void MethodToTrigger()
    {
        //This method will be called any time the button is pressed,
        //and the event is Invoked. Note that since the event is static,
        //this method will fire any time ANY button of that type is pressed.
    }

}