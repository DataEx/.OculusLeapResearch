﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;
using UnityEngine.UI;
using System.Collections.Generic;

// The model for our rigid hand made out of various polyhedra.
public class RigidHand : SkeletalHand {

	public float filtering = 0.5f;
	
	public TrackHand tracker;
	public Transform fadingPalm = null;

	
	void Start() {
	// Creates reference object on Leap hand generation
	fadingPalm = null;
	Transform rigidHand = this.gameObject.transform;	 
	
	Transform indexFinger = null;
	Transform thumb = null;
	Transform tempObj = rigidHand;
	for(int i = 0; i< rigidHand.childCount; i++){
		if(rigidHand.GetChild(i).name == "thumb"){
			tempObj = rigidHand.GetChild(i);
			for(int j = 0; j<tempObj.childCount; j++){
				if(tempObj.GetChild(j).name == "bone3"){
					thumb = tempObj.GetChild(j);
				}
			}
		}
		else if(rigidHand.GetChild(i).name == "index"){
			tempObj = rigidHand.GetChild(i);
			for(int j = 0; j<tempObj.childCount; j++){
				if(tempObj.GetChild(j).name == "bone3"){
					indexFinger = tempObj.GetChild(j);
				}
			}
		}

		
	}
	
	

	Hand hand = GetLeapHand();

	tracker = new TrackHand (this.gameObject, thumb, indexFinger);
	

	Transform  cube1 = GameObject.Find ("Cube1").transform;
	Transform  cube2 = GameObject.Find ("Cube2").transform;
	Transform  cube3 = GameObject.Find ("Cube3").transform;

	tracker.addGrabbableObject(cube1);
	tracker.addGrabbableObject(cube2);
	tracker.addGrabbableObject(cube3);


	palm.rigidbody.maxAngularVelocity = Mathf.Infinity;
    IgnoreCollisionsWithSelf();


	tracker.setName ((hand.Id).ToString ());
	tracker.setSize(new Vector3 (0.08f, 0.08f, 0.08f));
	palm.collider.enabled = false; // is this necessary?
  }


	void Update(){
		Hand hand = GetLeapHand();
		float frameID = hand.Frame.Id;
		float pinch = hand.PinchStrength;

		tracker.trackHand(pinch);
		tracker.updateFrame (frameID);
		if (fadingPalm == null) {
			tracker.updatePosition (palm);
		}
		else{
			tracker.updatePosition (fadingPalm.gameObject);

		}
		//print (hand.Frame.Id);	
		//print (Vector3.Distance (palm.transform.position, tracker.referenceHand.transform.position));

	//	Text text 	= GameObject.Find ("PinchStrengthText").GetComponent<Text>();
	//	text.text = "Pinch Str: " + pinch.ToString (); 

	}


  public override void InitHand() {
    base.InitHand();
  }

  public override void UpdateHand() {
    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null)
        fingers[f].UpdateFinger();
    }

    if (palm != null) {
      // Set palm velocity.
      Vector3 target_position = GetPalmCenter();
      palm.rigidbody.velocity = (target_position - palm.transform.position) *
                                (1 - filtering) / Time.deltaTime;

      // Set palm angular velocity.
      Quaternion target_rotation = GetPalmRotation();
      Quaternion delta_rotation = target_rotation *
                                  Quaternion.Inverse(palm.transform.rotation);
      float angle = 0.0f;
      Vector3 axis = Vector3.zero;
      delta_rotation.ToAngleAxis(out angle, out axis);

      if (angle >= 180) {
        angle = 360 - angle;
        axis = -axis;
      }
      if (angle != 0) {
        float delta_radians = (1 - filtering) * angle * Mathf.Deg2Rad;
        palm.rigidbody.angularVelocity = delta_radians * axis / Time.deltaTime;
      }
    }
  }
}
