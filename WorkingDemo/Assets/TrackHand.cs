using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine.UI;

public class TrackHand {

		public GameObject referenceHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Transform[] grabbableList;
		List<Transform> grabList = null; // = new List<Transform[]>();
		List<Transform> grabListParent = null;
		float maxConnectingDistance = 0.33f;
		float timeNow = -1;
		float lastPinch = 0.0f; // the pinch of the previous frame
		static float currentFrame = -1;
		Transform thumb;
		Transform indexFinger;
		public float pinchCloseLimit;
		public float pinchOpenLimit;
		public float grabTimerLength;
	//		Hand leapHand;

		public TrackHand(GameObject rigidHand, Transform thumbObj, Transform indexFingerObj){
			referenceHand.tag = "Hand";
			referenceHand.renderer.material.color = Color.grey;
			referenceHand.collider.enabled = false;
			referenceHand.AddComponent<ReferenceHand>().rigidHand = rigidHand;
			thumb = thumbObj;
			indexFinger = indexFingerObj;
			pinchCloseLimit = float.Parse(GameObject.Find ("CloseStrValue").GetComponent<Text> ().text.ToString ());
			pinchOpenLimit  = float.Parse(GameObject.Find ("OpenStrValue").GetComponent<Text> ().text.ToString ());
			grabTimerLength = float.Parse(GameObject.Find ("TimerLenValue").GetComponent<Text> ().text.ToString ());
			//Debug.Log (grabTimerLength);
	}

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

		public void unparentObject(){
			Transform tempObj;
			for(int i = 0; i<referenceHand.transform.childCount;i++){
				tempObj = referenceHand.transform.GetChild(i);
				for(int j = 0; j<grabList.Count; j++){
					if(tempObj == grabList[j]){
						tempObj.parent = grabListParent[j];
						referenceHand.GetComponent<ReferenceHand>().childsParent = null;
						break;
					}
				}
			}
		}

	public void startPinch(){

	}

	public void twoHandedGapingPinch(){
		float pinch = getPinchStr();
		//Transform obj = getWithinRangeObject ();
		bool systemOn = GameObject.Find ("TwoHandedUse").GetComponent<TwoHandedUse> ().systemOn;
		//if hand open, disconnect
		if(pinch > pinchCloseLimit){
			// reset timer
			if(systemOn == false || pinch<pinchOpenLimit){
				timeNow = -1;
				referenceHand.GetComponent<ReferenceHand>().isCounting = false;
				if(pinch<pinchOpenLimit && systemOn == true){
//					referenceHand.renderer.material.color = Color.green;
//					referenceHand.transform.GetChild(0).gameObject.renderer.material.color = Color.green;
				}
				else if(systemOn == false){
//					referenceHand.GetComponent<ReferenceHand>().pairedObj = null;
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
//						unparentObject();
						timeNow = -1;
						referenceHand.GetComponent<ReferenceHand>().isCounting = false;
						GameObject.Find ("TwoHandedUse").GetComponent<TwoHandedUse> ().systemOn = false;
//						referenceHand.GetComponent<ReferenceHand>().pairedObj = null;
						
					}
					else{
//						float percentageComplete =(timeElasped/grabTimerLength);
						// Green(0,1,0,1)
						// Red  (1,0,0,0)
//						referenceHand.GetComponent<ReferenceHand>().pairedObj.gameObject.renderer.material.color = new Color(percentageComplete, 1-percentageComplete, 0);
//						referenceHand.renderer.material.color = new Color(percentageComplete, 1-percentageComplete, 0);
						
					}
				}
			}
		}
		//else if(obj == null){
		//	timeNow = -1;
//			referenceHand.GetComponent<ReferenceHand>().isCounting = false;
//			referenceHand.GetComponent<ReferenceHand>().pairedObj = null;
			
		//}
		//if hand closed, start timer
		else{
			if(systemOn == false){
				if(timeNow == -1){
					if(lastPinch > pinchCloseLimit){
						timeNow = Time.realtimeSinceStartup;
						referenceHand.GetComponent<ReferenceHand>().isCounting = true;
//						referenceHand.GetComponent<ReferenceHand>().pairedObj = obj;
					}
				}
				else{
					//if timer > 1 sec, connect if within range
					float timeElasped = Time.realtimeSinceStartup - timeNow;
					if(timeElasped >= grabTimerLength){
						timeNow = -1;
						referenceHand.GetComponent<ReferenceHand>().isCounting = false;
						GameObject.Find ("TwoHandedUse").GetComponent<TwoHandedUse> ().systemOn = true;
//						referenceHand.GetComponent<ReferenceHand>().childsParent = obj.parent;
//						obj.parent = referenceHand.transform;
//						obj.gameObject.renderer.material.color = Color.green;
//						referenceHand.renderer.material.color = Color.green;
					}
					else{
//						float percentageComplete =(timeElasped/grabTimerLength);
						// Green(0,1,0,1)
						// Red  (1,0,0,0)
//						obj.gameObject.renderer.material.color = new Color((1-percentageComplete), percentageComplete, 0);
//						referenceHand.renderer.material.color = new Color((1-percentageComplete), percentageComplete, 0);
						//						Debug.Log(referenceHand.renderer.material.color);
					}
				}
			}
		}
		lastPinch = pinch;
	}





	public void gapingPinch(){
		float pinch = getPinchStr();
		Transform obj = getWithinRangeObject ();
		bool connected = (referenceHand.transform.childCount == 0) ? false : true; 
		//if hand open, disconnect
		if(pinch > pinchCloseLimit){
			// reset timer
			if(connected == false || pinch<pinchOpenLimit){
			   timeNow = -1;
			   referenceHand.GetComponent<ReferenceHand>().isCounting = false;
				if(pinch<pinchOpenLimit && connected == true){
					referenceHand.renderer.material.color = Color.green;
					referenceHand.transform.GetChild(0).gameObject.renderer.material.color = Color.green;
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
						// Green(0,1,0,1)
						// Red  (1,0,0,0)
						referenceHand.GetComponent<ReferenceHand>().pairedObj.gameObject.renderer.material.color = new Color(percentageComplete, 1-percentageComplete, 0);
						referenceHand.renderer.material.color = new Color(percentageComplete, 1-percentageComplete, 0);

					}
				}
			}
		}
		else if(obj == null){
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
						obj.parent = referenceHand.transform;
						obj.gameObject.renderer.material.color = Color.green;
						referenceHand.renderer.material.color = Color.green;
					}
					else{
						float percentageComplete =(timeElasped/grabTimerLength);
						// Green(0,1,0,1)
						// Red  (1,0,0,0)
						obj.gameObject.renderer.material.color = new Color((1-percentageComplete), percentageComplete, 0);
						referenceHand.renderer.material.color = new Color((1-percentageComplete), percentageComplete, 0);
//						Debug.Log(referenceHand.renderer.material.color);
					}
				}
			}
		}
		lastPinch = pinch;
	}

		public void parentObj(Transform obj){
			timeNow = -1;
			referenceHand.GetComponent<ReferenceHand>().childsParent = obj.parent;
			obj.parent = referenceHand.transform;

		}

		public float getPinchStr(){
			float pinchStr = Vector3.Distance (thumb.position, indexFinger.position);
			Text text 	= GameObject.Find ("PinchStrengthValue").GetComponent<Text>();
			text.text   = pinchStr.ToString("F3"); 
			return pinchStr;
		}

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



		public void reparentObjects(){
			for(int i = 0; i<grabList.Count; i++){
				if(grabList[i].parent != grabListParent[i] || grabList[i].parent.tag != "Hand"){
					grabList[i].parent = grabListParent[i];
				}
			}
		}

		public void update(GameObject palm, float frameID){
			updateFrame (frameID);
			updatePosition(palm);
			reparentObjects();
		}


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
		}	

		public void setSize(Vector3 dimensions){
			referenceHand.transform.localScale = (new Vector3 (dimensions.x, dimensions.y, dimensions.z));
		}

		public void updatePosition(GameObject palm){
			referenceHand.transform.position = palm.transform.position;
			referenceHand.transform.rotation = palm.transform.rotation;
		//Debug.Log (palm.name);
	}

		public Transform getWithinRangeObject(){
			if (grabList != null) {
				float[] distFromCube = new float[grabList.Count];
				for(int i = 0; i<distFromCube.Length; i++){
					distFromCube[i] = Vector3.Distance(referenceHand.transform.position, grabList[i].position);
				}
				// Find the closest object to the referenceHand
				Transform closestCube;
				int closestIndex = 0;
				for(int i = 0; i<distFromCube.Length; i++){
					if(distFromCube[i] < distFromCube[closestIndex]){
						closestIndex = i;
					}
				}
				closestCube = grabList[closestIndex];
				if(distFromCube [closestIndex] <= maxConnectingDistance){
					return closestCube;
				}
				else{
					return null;
				}
			}
			else{
				return null;
			}
		}
		
	public void updateFrame(float frameID){
		if (currentFrame != frameID) {
			currentFrame = frameID;
			colorObjs();
		}

	}
	public void colorObjs(){

		List<Transform> occupiedGrabbableObjs = new List<Transform> ();
		GameObject[] allHands = GameObject.FindGameObjectsWithTag ("Hand");
		GameObject twoHandedUse = GameObject.Find ("TwoHandedUse");
		foreach (GameObject hand in allHands) {
			if(hand.transform.childCount == 0 && hand.GetComponent<ReferenceHand>().isCounting == false){
				hand.renderer.material.color = Color.gray;
			}
		    if(hand == twoHandedUse.GetComponent<TwoHandedUse>().closestHand){
				Debug.Log(hand.name);
				hand.renderer.material.color = Color.blue;
			}
			if(hand.GetComponent<ReferenceHand>().pairedObj != null){
				occupiedGrabbableObjs.Add(hand.GetComponent<ReferenceHand>().pairedObj);
			}
		}

		List<GameObject> hands = new List<GameObject> ();
		for (int i = 0; i<allHands.Length; i++) {
			if(allHands[i].transform.childCount == 0 && allHands[i].GetComponent<ReferenceHand>().isCounting == false && allHands[i] != twoHandedUse.GetComponent<TwoHandedUse>().closestHand){
				hands.Add(allHands[i]);
			}
		}
		List<Transform> colorableList = new List<Transform> ();
		for(int i=0;i<grabList.Count;i++){
			if(grabList[i].parent.tag != "Hand" && occupiedGrabbableObjs.Contains (grabList[i])==false){
				grabList[i].gameObject.renderer.material.color = Color.grey;
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
				objColor = closestObj.gameObject.renderer.material.color;
				if(objColor == Color.grey){
					hands[i].renderer.material.color = colors[colorIndex];
					closestObj.gameObject.renderer.material.color = colors[colorIndex];
					colorIndex = (colorIndex+1)%colors.Length;
					// color hand and obj a color
				}
				else{
					hands[i].renderer.material.color = objColor;
				}
			}

		}



	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

}



// each frame, check the cloest objs to the hands and color them