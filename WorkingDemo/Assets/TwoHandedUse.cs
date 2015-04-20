using UnityEngine;
using System.Collections;
using Leap;
using System;

public class TwoHandedUse : MonoBehaviour {

	public bool useTwoHanded = false; // Check in Unity to decide whether we want to use 2-handed
	public bool systemOn = false; // Let's us know if system is ready 
//	GameObject[] listOfHands;
	// Use this for initialization
	GameObject grabbingHand = null;
	public GameObject closestHand  = null;
	public GameObject grabbingObj = null;
	Transform grabbingObjParent = null;
	GameObject sphere;
	float prevDist = -1;
	void Awake () {
		sphere = this.transform.GetChild (0).gameObject;
		Color startColor = sphere.renderer.material.color;
		sphere.renderer.material.color = new Color (startColor.r, startColor.g, startColor.b, 0.4f);

	}
	void setupTwoHanded(){
		grabbingObj = grabbingHand.transform.GetChild (0).gameObject;
		grabbingObjParent = grabbingHand.GetComponent<ReferenceHand> ().childsParent;
		float dist = Vector3.Distance (grabbingObj.transform.position, closestHand.transform.position);
		sphere.transform.localScale = new Vector3 (2*dist, 2*dist, 2*dist);
		if (prevDist != -1) {
//			Debug.Log ("Scaling");
			float changeInScale = (dist - prevDist)/prevDist;
			Debug.Log ("diff dist: " + (dist-prevDist).ToString());
			//			Debug.Log ("dist: " + dist.ToString());
//			Debug.Log ("changeInScale: " + changeInScale.ToString());

			//dist > prevDist --> larger circle --> scale object larger
			Vector3 currentScale = grabbingObj.transform.localScale;
			float factor;
			factor = (1+changeInScale);
			factor = (float)Math.Pow(factor,5);
			Debug.Log ("factor: " + factor.ToString());
			grabbingObj.transform.localScale = new Vector3((factor)*currentScale.x,(factor)*currentScale.y,(factor)*currentScale.z);
		}
//		if (grabbingObj.transform.parent != sphere.transform) {

//			sphere.transform.parent = closestHand.transform;
//			grabbingObj.transform.parent = sphere.transform;
//		}
		sphere.transform.position = grabbingObj.transform.position;
		sphere.renderer.enabled = true;
		prevDist = dist;
//		closestHand.renderer.material.color = Color.blue;
		// change color of 2nd hand
		// if 2nd hand pinches
		// activate if 2nd hand pinches? use timer and everything

		//draw ring around hand1 with hand2 at radius

		//		Gizmos.color = Color.yellow;
//		Gizmos.DrawSphere(grabbingHand.transform.position, dist);

		// the movement of the grabbing hand will not act as normal, but is instead allowed to move freely(?)
			// as long it stays in view
		// as the cloestHand moves around the grabbingHand's obj, rotate the obj
		// as the cloestHand moves closer/further to the obj, the obj scales accordingly 
			// it may not be the object but rather a ghost of the obj
		// Do we want to keep the changes in scale and rotation? Or is it for observation pruposes only

	}

	void cancelTwoHanded(){

	}

	// Update is called once per frame
	void Update () {
//			Debug.Log("systemOn: " + systemOn.ToString());
			if (systemOn) {
				if (grabbingHand != null && closestHand != null && grabbingHand.transform.childCount != 0){
					setupTwoHanded ();
				} 
				else{
					systemOn = false;
				}	
			}
			else {
				GameObject[] listOfHands = GameObject.FindGameObjectsWithTag ("Hand");
				sphere.renderer.enabled = false;
				grabbingHand = null;
				closestHand = null;
				sphere.transform.parent = this.transform;
//				if(grabbingObj != null){
//					grabbingObj.transform.parent = grabbingObjParent;
//				}
				prevDist = -1;
				grabbingObjParent = null;
				systemOn = false;
				int grabbingHandIndex = -1;
				for (int i = 0; i<listOfHands.Length; i++) {
						if (listOfHands [i].transform.childCount != 0) {
								grabbingHand = listOfHands [i];
								grabbingHandIndex = i;
						}
				}
				if (grabbingHandIndex != -1 && grabbingHand != null && listOfHands.Length >= 2) {
						// Now that one hand is sucessfully grabbing an object and another to use
						// we can setup the two-handed procedure
						float minDistance;
						int closestHandIndex;
						if (grabbingHandIndex == 0) {
								minDistance = Vector3.Distance (grabbingHand.transform.position, listOfHands [1].transform.position);
								closestHandIndex = 1;
						} else {
								minDistance = Vector3.Distance (grabbingHand.transform.position, listOfHands [0].transform.position);
								closestHandIndex = 0;
						}
						for (int i = 0; i<listOfHands.Length; i++) {
								if (i != grabbingHandIndex) {
										float distance = minDistance = Vector3.Distance (grabbingHand.transform.position, listOfHands [i].transform.position);
										if (distance < minDistance) {
												closestHandIndex = i;
												minDistance = distance;
										}
								}

						}
						// maybe use a threshhold for closestHand?
						closestHand = listOfHands [closestHandIndex];

				}
			}		
	}
}
