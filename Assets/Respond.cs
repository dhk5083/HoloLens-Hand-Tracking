using UnityEngine;
using System.Collections;

public class Respond : MonoBehaviour {

	bool placing = false;
	
	// Called by GazeGestureManager when the user performs a Select gesture
	void OnSelect()
	{
		// On each Select gesture, toggle whether the user is in placing mode.
		placing = !placing;
		if (placing) {
			print ("Move enabled!");
		} else {
			print ("Move disabled!");
		}

	}
	// Update is called once per frame
	void Update()
	{
		// If the user is in placing mode,
		// update the placement to match the user's gaze.

	}
}
