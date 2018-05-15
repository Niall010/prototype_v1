using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

public class ControllerInput : MonoBehaviour
{
    //Should only ever be one, but just in case
    protected List<VRInteractableItem> heldObjects;
    //Controller References
    protected SteamVR_TrackedObject trackedObj;
    //Should only ever be one, but just in case one gets stuck
    protected Dictionary<EVRButtonId, List<VRInteractableItem>> pressDownObjects;

    protected List<EVRButtonId> buttonsTracked;


    public SteamVR_Controller.Device device
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }

    public delegate void TouchpadPress();
    public static event TouchpadPress OnTouchpadPress;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        //Instantiate lists
        pressDownObjects = new Dictionary<EVRButtonId, List<VRInteractableItem>>();

        //List the buttons you send inputs to the controller for
        buttonsTracked = new List<EVRButtonId>()
    {
        EVRButtonId.k_EButton_SteamVR_Trigger,
        EVRButtonId.k_EButton_Grip
    };
    }

    void OnTriggerStay(Collider collider)
    {
        //If collider has a rigid body to report to
        if (collider.attachedRigidbody != null)
        {
            //If rigidbody's object has interactable item scripts, iterate through them
            VRInteractableItem[] interactables = collider.attachedRigidbody.GetComponents<VRInteractableItem>();
            for (int i = 0; i < interactables.Length; i++)
            {
                VRInteractableItem interactable = interactables[i];
                for (int b = 0; b < buttonsTracked.Count; b++)
                {
                    //If a tracked button is pressed
                    EVRButtonId button = buttonsTracked[b];
                    if (device.GetPressDown(button))
                    {
                        //If we haven't already sent the button press message to this interactable
                        //Safeguard against objects that have multiple colliders for one interactable script
                        if (!pressDownObjects.ContainsKey(button) || !pressDownObjects[button].Contains(interactable))
                        {
                            //Send button press through to interactable script
                            interactable.ButtonPressDown(button, this);

                            //Add interactable script to a dictionary flagging it to recieve notice
                            //when that same button is released
                            if (!pressDownObjects.ContainsKey(button))
                                pressDownObjects.Add(button, new List<VRInteractableItem>());

                            pressDownObjects[button].Add(interactable);
                        }
                    }
                }
            }
        }
    }


    void Update()
    {
        //Check through all desired buttons to see if any have been released
        EVRButtonId[] pressKeys = pressDownObjects.Keys.ToArray();
        for (int i = 0; i < pressKeys.Length; i++)
        {
            //If tracked button is released
            if (device.GetPressUp(pressKeys[i]))
            {
                //Get all tracked objects in that button's "pressed" list
                List<VRInteractableItem> releaseObjects = pressDownObjects[pressKeys[i]];
                for (int j = 0; j < releaseObjects.Count; j++)
                {
                    //Send button release through to interactable script
                    releaseObjects[j].ButtonPressUp(pressKeys[i], this);
                }

                //Clear 
                pressDownObjects[pressKeys[i]].Clear();
            }
        }
    }

}
