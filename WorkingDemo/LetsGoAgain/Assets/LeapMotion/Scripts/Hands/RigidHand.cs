/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

// The model for our rigid hand made out of various polyhedra.
public class RigidHand : SkeletalHand {

	public TrackHand tracker;

  public float filtering = 0.5f;

  void Start() {
		Transform rigidHand = this.gameObject.transform;	 
		Transform indexFinger = null;
		Transform thumb = null;
		Transform tempObj = rigidHand;
		for (int i = 0; i< rigidHand.childCount; i++) {
			if (rigidHand.GetChild (i).name == "thumb") {
				tempObj = rigidHand.GetChild (i);
				for (int j = 0; j<tempObj.childCount; j++) {
					if (tempObj.GetChild (j).name == "bone3") {
						thumb = tempObj.GetChild (j);
					}
				}
			} else if (rigidHand.GetChild (i).name == "index") {
				tempObj = rigidHand.GetChild (i);
				for (int j = 0; j<tempObj.childCount; j++) {
					if (tempObj.GetChild (j).name == "bone3") {
						indexFinger = tempObj.GetChild (j);
					}
				}
			}
		}
		Hand hand = GetLeapHand();
		
		tracker = new TrackHand (this.gameObject, thumb, indexFinger);

		GameObject grabTechniqueObject = GameObject.Find ("GrabTechnique");
		List<GameObject> grabbableObjects = grabTechniqueObject.GetComponent<GrabTechniqueDecider> ().GrabbableObjects;
		for (int i = 0; i<grabbableObjects.Count; i++) {
			tracker.addGrabbableObject(grabbableObjects[i].transform);
		}

		tracker.setName ((hand.Id).ToString ());
		tracker.setSize(new Vector3 (0.08f, 0.08f, 0.08f));
		palm.GetComponent<Collider>().enabled = false;

		palm.GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
		Leap.Utils.IgnoreCollisions(gameObject, gameObject);

	}

	// Each frame, data from the Leap Motion API is sent over
	void Update(){
		Hand hand = GetLeapHand();
		float frameID = hand.Frame.Id;
		float pinch = hand.PinchStrength;
		
		tracker.trackHand(pinch);
		tracker.update (palm, frameID);
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
      palm.GetComponent<Rigidbody>().velocity = (target_position - palm.transform.position) *
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
        palm.GetComponent<Rigidbody>().angularVelocity = delta_radians * axis / Time.deltaTime;
      }
    }

    if (forearm != null) {
      // Set arm dimensions.
      CapsuleCollider capsule = forearm.GetComponent<CapsuleCollider> ();
      if (capsule != null) {
        // Initialization
        capsule.direction = 2;
        forearm.transform.localScale = new Vector3(1f, 1f, 1f);
        
        // Update
        capsule.radius = GetArmWidth () / 2f;
        capsule.height = GetArmLength () + GetArmWidth ();
      }

      // Set arm velocity.
      Vector3 target_position = GetArmCenter ();
      forearm.GetComponent<Rigidbody>().velocity = (target_position - forearm.transform.position) *
        (1 - filtering) / Time.deltaTime;

      // Set arm velocity.
      Quaternion target_rotation = GetArmRotation ();
      Quaternion delta_rotation = target_rotation *
        Quaternion.Inverse (forearm.transform.rotation);
      float angle = 0.0f;
      Vector3 axis = Vector3.zero;
      delta_rotation.ToAngleAxis (out angle, out axis);

      if (angle >= 180) {
        angle = 360 - angle;
        axis = -axis;
      }
      if (angle != 0) {
        float delta_radians = (1 - filtering) * angle * Mathf.Deg2Rad;
        forearm.GetComponent<Rigidbody>().angularVelocity = delta_radians * axis / Time.deltaTime;
      }
    }
  }
}
