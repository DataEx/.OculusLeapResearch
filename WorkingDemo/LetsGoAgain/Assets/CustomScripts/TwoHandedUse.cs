using UnityEngine;
using System.Collections;
using Leap;
using System;
using System.Collections.Generic;

public class TwoHandedUse : MonoBehaviour {


	public bool useTwoHanded = false; // Check in Unity to decide whether we want to use 2-handed
	public bool systemOn = false; // Let's us know if system is ready 
	GameObject grabbingHand = null;
	public GameObject closestHand  = null;
	public GameObject grabbingObj = null;
	Transform grabbingObjParent = null;
	GameObject sphere;
	GameObject rotateHelper;
	float prevDist = -1;
	Vector3 prevHandToObjVec = Vector3.zero;
	Quaternion prevSphereRotation;
	Vector3 curHandToObjVec;

	void Awake () {
		sphere = this.transform.GetChild (0).gameObject;
		Color startColor = sphere.GetComponent<Renderer>().material.color;
		sphere.GetComponent<Renderer>().material.color = new Color (startColor.r, startColor.g, startColor.b, 0.4f);
		rotateHelper = new GameObject ();
		rotateHelper.name = "RotateHelper";
		rotateHelper.transform.parent = this.transform;
	}

	// Creates a viewable environment for scaling the grabbable object and does the calculations for it 
	void setupTwoHandedScaling(){
		float dist = Vector3.Distance (grabbingHand.transform.position, closestHand.transform.position);
		sphere.transform.localScale = new Vector3 (2*dist, 2*dist, 2*dist);
		if (prevDist != -1) {
			float changeInScale = (dist - prevDist)/prevDist;
			Vector3 currentScale = grabbingObj.transform.localScale;
			float factor;
			factor = (1+changeInScale);
			factor = (float)Math.Pow(factor,5);
			grabbingObj.transform.localScale = new Vector3((factor)*currentScale.x,(factor)*currentScale.y,(factor)*currentScale.z);
		}
		sphere.transform.position = grabbingObj.transform.position;
		sphere.GetComponent<Renderer>().enabled = true;
		prevDist = dist;
	}
	void resetVariables(){
		sphere.GetComponent<Renderer>().enabled = false;
		grabbingHand = null;
		closestHand = null;
		sphere.transform.parent = this.transform;
		prevDist = -1;
		grabbingObj = null;
		prevHandToObjVec = Vector3.zero;
		grabbingObjParent = null;
		systemOn = false;
	}

	// Does the calculations for the rotation of the scaling object
	void setupTwoHandedRotation(){
		rotateHelper.transform.position = grabbingObj.transform.position;
		// Draw line between the two objects
		if (prevHandToObjVec == Vector3.zero) {
			prevHandToObjVec = grabbingObj.transform.position - closestHand.transform.position;
			prevSphereRotation = rotateHelper.transform.rotation; 
		} 
		else {
			curHandToObjVec = grabbingObj.transform.position - closestHand.transform.position;
			grabbingObj.transform.rotation = Quaternion.FromToRotation(prevHandToObjVec, curHandToObjVec);
		}
	}

	// Updates variables determining if two handed transformations can be applied
	void Update () {
			if (systemOn) {
				if (grabbingHand != null && closestHand != null){
					if(grabbingHand.transform.parent.childCount == 1){
						setupTwoHandedScaling ();
					}
					else if(grabbingHand.transform.parent.childCount == 2){
						setupTwoHandedRotation ();
					}
				} 
				else{
					systemOn = false;
				}	
			}
			else {
				resetVariables();
				GameObject[] listOfHands = GameObject.FindGameObjectsWithTag ("Hand");
				int grabbingHandIndex = -1;
				for (int i = 0; i<listOfHands.Length; i++) {
					Transform closestObj = listOfHands[i].GetComponent<ReferenceHand>().closestObj;
					if(closestObj != null && listOfHands[i].transform.parent.childCount != 2){
						grabbingHand = listOfHands [i];
						grabbingHandIndex = i;
						grabbingObj = closestObj.gameObject;
						grabbingObjParent = closestObj.parent;

					}
					else if(listOfHands[i].transform.parent.childCount == 2){
						grabbingHand = listOfHands [i];
						grabbingHandIndex = i;
						grabbingObj = grabbingHand.GetComponent<ReferenceHand>().pairedObj.gameObject;
						grabbingObjParent = grabbingHand.GetComponent<ReferenceHand>().childsParent;
					}
					break;
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
								if (i != grabbingHandIndex && listOfHands[i].transform.parent.childCount != 2) {
										float distance = Vector3.Distance (grabbingHand.transform.position, listOfHands [i].transform.position);
										if (distance < minDistance) {
												closestHandIndex = i;
												minDistance = distance;
										}
								}

						}
						closestHand = listOfHands [closestHandIndex];
			}
			}
			// if systemOn is false and the rest of the hands are recognized, then do rotation
//			if (grabbingHand != null && closestHand != null && grabbingHand.transform.parent.childCount != 1) {
//				if(systemOn == false){
//					setupTwoHandedRotation ();
//				}
//			}
	}
}
	