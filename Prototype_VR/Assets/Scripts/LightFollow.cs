using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public Transform objectToFollow;
	
	void Update ()
    {
        // If this object's position isn't equal to the object to follow's position
        // Set this object's position to same position of object to follow
		if (gameObject.transform.position != objectToFollow.position)
        {
            gameObject.transform.position = objectToFollow.position;
        }
	}
}
