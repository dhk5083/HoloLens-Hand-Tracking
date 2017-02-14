using UnityEngine;
using System.Collections;

public class DistanceCalc : MonoBehaviour {
	public GameObject obj;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		//print("Cube Pos: " + transform.position + " | Plane Pos: " + obj.transform.position);
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		Debug.DrawRay(transform.position, fwd, Color.green, 2, true);

		RaycastHit hit;
		if (Physics.Raycast(transform.position, fwd, out hit, 10))
		{
			//print("Distance to gameObject: " + hit.distance);
		}
	}
	
}
