using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine.UI;

public class TrackHand {

		public GameObject parent = new GameObject ();	
		public GameObject referenceHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Transform[] grabbableList;
		List<Transform> grabList = null;
		List<Transform> grabListParent = null;
		float maxConnectingDistance = 0.33f;
		float timeNow = -1;
		float lastPinch = 0.0f;
		static float currentFrame = -1;
		Transform thumb;
		Transform indexFinger;
		public float pinchCloseLimit;
		public float pinchOpenLimit;
		public float grabTimerLength;
		Vector3 prevPos = Vector3.zero;

		public TrackHand(GameObject rigidHand, Transform thumbObj, Transform indexFingerObj){
			referenceHand.transform.parent = parent.transform;
			referenceHand.tag = "Hand";
			referenceHand.GetComponent<Renderer>().material.color = Color.grey;
			referenceHand.GetComponent<Collider>().enabled = false;
			referenceHand.AddComponent<ReferenceHand>().rigidHand = rigidHand;
			Shader shader = Shader.Find ("Specular");
			referenceHand.GetComponent<MeshRenderer>().material = new Material(shader);
			thumb = thumbObj;
			indexFinger = indexFingerObj;
			pinchCloseLimit = float.Parse(GameObject.Find ("CloseStrValue").GetComponent<Text> ().text.ToString ());
			pinchOpenLimit  = float.Parse(GameObject.Find ("OpenStrValue").GetComponent<Text> ().text.ToString ());
			grabTimerLength = float.Parse(GameObject.Find ("TimerLenValue").GetComponent<Text> ().text.ToString ());
	}

		// Determines which grabbing function to use based on pre-selected values (ie the boolean values in
	    // the grabTechnique gameobject)
		public void trackHand(float pinch){
			GameObject grabTechnique = GameObject.Find("GrabTechnique");
			bool useToggleGrab = grabTechnique.GetComponent<GrabTechniqueDecider>().toggleGrab;
			bool usePinchGrab = grabTechnique.GetComponent<GrabTechniqueDecider>().pinchGrab;
			bool useGapingGrab = grabTechnique.GetComponent<GrabTechniqueDecider>().gapingGrab;
			GameObject useTwoHanded = GameObject.Find ("TwoHandedUse");
			bool useGapingGrabForTwoHanded;
			if (useTwoHanded.GetComponent<TwoHandedUse> ().useTwoHanded == true && referenceHand == useTwoHanded.GetComponent<TwoHandedUse> ().closestHand) {
					useGapingGrabForTwoHanded = true;
			} else {
					useGapingGrabForTwoHanded = false;
			}
			if (useGapingGrabForTwoHanded) {
				twoHandedGapingPinch();
				}
				else {
					if (useToggleGrab) {
							holdToggle (pinch);
					} else if (usePinchGrab) {
							pinchHold (pinch);
					} else if (useGapingGrab) {
							gapingPinch ();
					}
				}

		}	

		// Eliminates the link between grabbed object and grabbing hand and
	    // returns the grabbed object to its orignial parent 
		public void unparentObject(){
			prevPos = Vector3.zero;
			Transform tempObj;
			for(int i = 0; i<parent.transform.childCount;i++){
				tempObj = parent.transform.GetChild(i);
				for(int j = 0; j<grabList.Count; j++){
					if(tempObj == grabList[j]){
						if(tempObj == referenceHand.GetComponent<ReferenceHand>().pairedObj){
						}
						tempObj.parent = grabListParent[j];
						referenceHand.GetComponent<ReferenceHand>().childsParent = null;
						break;
					}
				}
			}
		}

	// Does the calculation for the "gaping grab" (pinch and hold) technique for secondary hand when the primary hand 
	// already has attached to an object
	public void twoHandedGapingPinch(){
		float pinch = getPinchStr();
		bool systemOn = GameObject.Find ("TwoHandedUse").GetComponent<TwoHandedUse> ().systemOn;
		//if hand open, disconnect
		if(pinch > pinchCloseLimit){
			// reset timer
			if(systemOn == false || pinch<pinchOpenLimit){
				timeNow = -1;
				referenceHand.GetComponent<ReferenceHand>().isCounting = false;
				if(pinch<pinchOpenLimit && systemOn == true){
				}
			}
			else{
				if(timeNow == -1){
					timeNow = Time.realtimeSinceStartup;
					referenceHand.GetComponent<ReferenceHand>().isCounting = true;
				}
				else{
					float timeElasped = Time.realtimeSinceStartup - timeNow;
					if(timeElasped >= grabTimerLength){
						timeNow = -1;
						referenceHand.GetComponent<ReferenceHand>().isCounting = false;
						GameObject.Find ("TwoHandedUse").GetComponent<TwoHandedUse> ().systemOn = false;
					}
				}
			}
		}
		//if hand closed, start timer
		else{
			if(systemOn == false){
				if(timeNow == -1){
					if(lastPinch > pinchCloseLimit){
						timeNow = Time.realtimeSinceStartup;
						referenceHand.GetComponent<ReferenceHand>().isCounting = true;
					}
				}
				else{
					//if timer > 1 sec, connect if within range
					float timeElasped = Time.realtimeSinceStartup - timeNow;
					if(timeElasped >= grabTimerLength){
						timeNow = -1;
						referenceHand.GetComponent<ReferenceHand>().isCounting = false;
						GameObject.Find ("TwoHandedUse").GetComponent<TwoHandedUse> ().systemOn = true;
					}
				}
			}
		}
		lastPinch = pinch;
	}




	// Does calculation for the "gaping pinch" (pinch and hold) and is the primary and best pinch available 
	public void gapingPinch(){
		float pinch = getPinchStr();
		Transform obj = getWithinRangeObject ();
		bool connected = (parent.transform.childCount == 1) ? false : true; 
		//if hand open, disconnect
		if(pinch > pinchCloseLimit){
			// reset timer
			if(connected == false || pinch<pinchOpenLimit){
			   timeNow = -1;
			   referenceHand.GetComponent<ReferenceHand>().isCounting = false;
				if(pinch<pinchOpenLimit && connected == true){
					referenceHand.GetComponent<Renderer>().material.color = Color.green;
					parent.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = Color.green;
				}
				else if(connected == false){
					referenceHand.GetComponent<ReferenceHand>().pairedObj = null;
				}
			}
			else{
				if(timeNow == -1){
					timeNow = Time.realtimeSinceStartup;
					referenceHand.GetComponent<ReferenceHand>().isCounting = true;
				}
				else{
					float timeElasped = Time.realtimeSinceStartup - timeNow;
					if(timeElasped >= grabTimerLength){
						unparentObject();
						timeNow = -1;
						referenceHand.GetComponent<ReferenceHand>().isCounting = false;
						referenceHand.GetComponent<ReferenceHand>().pairedObj = null;

					}
					else{
						float percentageComplete =(timeElasped/grabTimerLength);
						referenceHand.GetComponent<ReferenceHand>().pairedObj.gameObject.GetComponent<Renderer>().material.color = new Color(percentageComplete, 1-percentageComplete, 0);
						referenceHand.GetComponent<Renderer>().material.color = new Color(percentageComplete, 1-percentageComplete, 0);

					}
				}
			}
		}
		else if(obj == null && connected == false){
			timeNow = -1;
			referenceHand.GetComponent<ReferenceHand>().isCounting = false;
			referenceHand.GetComponent<ReferenceHand>().pairedObj = null;

		}
		//if hand closed, start timer
		else{
			if(connected == false){
				if(timeNow == -1){
					if(lastPinch > pinchCloseLimit){
						timeNow = Time.realtimeSinceStartup;
						referenceHand.GetComponent<ReferenceHand>().isCounting = true;
						referenceHand.GetComponent<ReferenceHand>().pairedObj = obj;
					}
				}
				else{
					//if timer > 1 sec, connect if within range
					float timeElasped = Time.realtimeSinceStartup - timeNow;
					if(timeElasped >= grabTimerLength){
						timeNow = -1;
						referenceHand.GetComponent<ReferenceHand>().isCounting = false;
						referenceHand.GetComponent<ReferenceHand>().childsParent = obj.parent;
						obj.parent = parent.transform;
						obj.gameObject.GetComponent<Renderer>().material.color = Color.green;
						referenceHand.GetComponent<Renderer>().material.color = Color.green;
					}
					else{
						float percentageComplete =(timeElasped/grabTimerLength);
						obj.gameObject.GetComponent<Renderer>().material.color = new Color((1-percentageComplete), percentageComplete, 0);
						referenceHand.GetComponent<Renderer>().material.color = new Color((1-percentageComplete), percentageComplete, 0);
					}
				}
			}
		}
		lastPinch = pinch;
	}

	// Calculates the pinch strength instead of using the Leap Motion API's.
	// Updates on-screen text with the updated pinch value
	public float getPinchStr(){
			float pinchStr = Vector3.Distance (thumb.position, indexFinger.position);
			Text text 	= GameObject.Find ("PinchStrengthValue").GetComponent<Text>();
			text.text   = pinchStr.ToString("F3"); 
			return pinchStr;
	}

	// Does calculation for outdated pinch technique
		public void pinchHold(float pinch){
			Transform obj = getWithinRangeObject ();
			bool connected = (referenceHand.transform.childCount == 0) ? false : true; 
			//if hand open, disconnect
			if(pinch < 0.7f){
				// reset timer
				timeNow = -1;
				if(connected){
					unparentObject();				
				}
			}
			else if(obj == null){
				timeNow = -1;
			}
			//if hand closed, start timer
			else{
				if(connected == false){
					if(timeNow == -1){
						if(lastPinch < 0.7f){
							timeNow = Time.realtimeSinceStartup;
						}
					}
					else{
						//if timer > 1 sec, connect if within range
						float timeElasped = Time.realtimeSinceStartup - timeNow;
						if(timeElasped >= 1.0){
							timeNow = -1;
							referenceHand.GetComponent<ReferenceHand>().childsParent = obj.parent;
							obj.parent = referenceHand.transform;

						
					}

					}
				}
			}
			lastPinch = pinch;
		}
		
	// Does calculation for outdated pinch technique
	public void holdToggle(float pinch){
			Transform obj = getWithinRangeObject ();
			// if hand open, reset
			if(pinch < 0.7f || obj == null){
				timeNow = -1;
			}
			// if hand close, start timer 
			else{
				if(timeNow == -1){
					if(lastPinch < 0.7f){
						timeNow = Time.realtimeSinceStartup;
					}
				}
				else{
					float timeElasped = Time.realtimeSinceStartup - timeNow;
					// if timer lasts 1 sec, check whether hand is parented or not
					if(timeElasped >= 1.0){
						timeNow = -1;
						// If not attached to an object, parent it to the hand if an object is close enough
						if(referenceHand.transform.childCount == 0){
							referenceHand.GetComponent<ReferenceHand>().childsParent = obj.parent;
							obj.parent = referenceHand.transform;
						}
						// If attached to an object, deparent it
						else{
							unparentObject();
						}
					}
				}					
			}
			lastPinch = pinch;
		}


		// Takes in data from the physical hand and updates tracking infcrmation 
		public void update(GameObject palm, float frameID){
			updateFrame (frameID);
			updatePosition(palm);
		}

		// Adds a gameobject to the llist of objects which can be considered grabbable by the application
		public void addGrabbableObject(Transform obj){
			if (grabList == null) {
				grabList = new List<Transform>();
				grabListParent = new List<Transform>();
			}
			grabList.Add(obj);
			grabListParent.Add(obj.parent);
		}

		public void setName(string name){
			referenceHand.name = name;
			parent.name = name + "_parent";
		}	

		// Returns True if a hand attached an object will apply tranlational effects to the object, else False
		// It will return False if not attached to an object or if currently applying rotational or scaling effects
		public bool moveObject(){
			GameObject twoHanded = GameObject.Find("TwoHandedUse");
			bool useTwoHanded = twoHanded.GetComponent<TwoHandedUse>().useTwoHanded;
			bool hasClosestHand = false;
			bool hasGrabbingObj = false;
			bool systemOn = false;
			if(useTwoHanded){
				hasClosestHand  = twoHanded.GetComponent<TwoHandedUse>().closestHand != null;
				hasGrabbingObj  = twoHanded.GetComponent<TwoHandedUse>().grabbingObj != null;
				systemOn  = twoHanded.GetComponent<TwoHandedUse>().systemOn;
			}
			if ((useTwoHanded && hasClosestHand && hasGrabbingObj && systemOn) == false) {
				return true;
			}
			else{
				return false;
			}
		}
	
		public void setSize(Vector3 dimensions){
			referenceHand.transform.localScale = (new Vector3 (dimensions.x, dimensions.y, dimensions.z));
		}

		// Applies positional updates to the referenceHand and to it attached object (if currently applying translational
		// transformation)
		public void updatePosition(GameObject palm){
			Vector3 palmPos = palm.transform.position;
			referenceHand.transform.position = palmPos;
			referenceHand.transform.rotation = palm.transform.rotation;
			
			
			if(parent.transform.childCount == 2 && prevPos != Vector3.zero && moveObject()){
				Vector3 changeInPos = new Vector3(palmPos.x - prevPos.x,palmPos.y - prevPos.y,palmPos.z - prevPos.z); 
				Vector3 objOldPos = referenceHand.GetComponent<ReferenceHand>().pairedObj.transform.position;
				referenceHand.GetComponent<ReferenceHand>().pairedObj.transform.position = new Vector3(objOldPos.x + changeInPos.x, objOldPos.y + changeInPos.y, objOldPos.z + changeInPos.z);
			}
			prevPos = referenceHand.transform.position;
	}

		// Returns the closest grabbable object to the hand within a threshhold
		public Transform getWithinRangeObject(){
			if (grabList != null) {
				List<Transform> canGrabList = new List<Transform>();
				for(int i = 0; i<grabList.Count; i++){
					if(grabList[i].parent == grabListParent[i]){
						canGrabList.Add(grabList[i]);
					}
				}
				if(canGrabList.Count == 0){return null;}
				float[] distFromCube = new float[canGrabList.Count];
				for(int i = 0; i<distFromCube.Length; i++){
					distFromCube[i] = Vector3.Distance(referenceHand.transform.position, canGrabList[i].position);
				}
				// Find the closest object to the referenceHand
				Transform closestCube;
				int closestIndex = 0;
				for(int i = 0; i<distFromCube.Length; i++){
					if(distFromCube[i] < distFromCube[closestIndex]){
						closestIndex = i;
					}
				}
				closestCube = canGrabList[closestIndex];
				if(distFromCube [closestIndex] <= maxConnectingDistance){
					referenceHand.GetComponent<ReferenceHand>().closestObj = closestCube;
					return closestCube;
				}
				else{
					referenceHand.GetComponent<ReferenceHand>().closestObj = null;
					return null;
				}
			}
			else{
					referenceHand.GetComponent<ReferenceHand>().closestObj = null;
				return null;
			}
		}

	// calls the colorObjs() once per frame (not once per hand per frame)
	public void updateFrame(float frameID){
		if (currentFrame != frameID) {
			currentFrame = frameID;
			colorObjs();
		}

	}

	// Updates the colors of hands and grabbable objects based on their position and activity 
	public void colorObjs(){

		List<Transform> occupiedGrabbableObjs = new List<Transform> ();
		GameObject[] allHands = GameObject.FindGameObjectsWithTag ("Hand");
		GameObject twoHandedUse = GameObject.Find ("TwoHandedUse");
		foreach (GameObject hand in allHands) {
			if(hand.transform.parent.childCount == 1 && hand.GetComponent<ReferenceHand>().isCounting == false){
				hand.GetComponent<Renderer>().material.color = Color.gray;
			}
		    if(hand == twoHandedUse.GetComponent<TwoHandedUse>().closestHand){
				hand.GetComponent<Renderer>().material.color = Color.blue;
			}
			if(hand.GetComponent<ReferenceHand>().pairedObj != null){
				occupiedGrabbableObjs.Add(hand.GetComponent<ReferenceHand>().pairedObj);
			}
		}

		List<GameObject> hands = new List<GameObject> ();
		for (int i = 0; i<allHands.Length; i++) {
			if(allHands[i].transform.parent.childCount == 1 && allHands[i].GetComponent<ReferenceHand>().isCounting == false && allHands[i] != twoHandedUse.GetComponent<TwoHandedUse>().closestHand){
				hands.Add(allHands[i]);
			}
		}
		List<Transform> colorableList = new List<Transform> ();
		for(int i=0;i<grabList.Count;i++){
			if(grabList[i].parent.tag != "Hand" && occupiedGrabbableObjs.Contains (grabList[i])==false){
				grabList[i].gameObject.GetComponent<Renderer>().material.color = Color.grey;
				colorableList.Add (grabList[i]);
			}
		}

		// for each hand, check to see which is the closest cube
		// if that cube is close enough, color it and the hand
		// if cube is already colored, color the hand the same color
		float[] distFromHand = new float[colorableList.Count];
		int closestIndex;
		Transform closestObj;
		Color objColor;
		int colorIndex = 0;
		Color[] colors = {Color.red, Color.blue, Color.cyan, Color.magenta};
		for (int i = 0; i<hands.Count; i++) {
			for(int j = 0; j<colorableList.Count; j++){
				distFromHand[j] = Vector3.Distance(hands[i].transform.position, colorableList[j].position);
			}
			closestIndex = 0;
			for(int j = 0; j<distFromHand.Length; j++){
				if(distFromHand[j] < distFromHand[closestIndex]){
					closestIndex = j;
				}
			}
			closestObj = colorableList[closestIndex];
			if (distFromHand [closestIndex] <= maxConnectingDistance) {
				objColor = closestObj.gameObject.GetComponent<Renderer>().material.color;
				if(objColor == Color.grey){
					hands[i].GetComponent<Renderer>().material.color = colors[colorIndex];
					closestObj.gameObject.GetComponent<Renderer>().material.color = colors[colorIndex];
					colorIndex = (colorIndex+1)%colors.Length;
					// color hand and obj a color
				}
				else{
					hands[i].GetComponent<Renderer>().material.color = objColor;
				}
			}

		}



	}
	
}