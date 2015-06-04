using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine.UI;

public class ReferenceHand : MonoBehaviour {

	public GameObject rigidHand;
	public Transform childsParent = null;
	public Transform pairedObj = null;
	public bool isCounting = false;
	public Timers timer;
	public Transform closestObj = null;

	void Awake () {
		float timerLength = float.Parse(GameObject.Find ("TimerLenValue").GetComponent<Text> ().text.ToString ());
		timer = new Timers (this.gameObject,timerLength,0.6f);
	}

	// Uses data from the grasping functions in TrackHand.cs to determine whether to use the timer  
	void Update () {
		if (isCounting == true) {
			timer.startTimer();
		}
		else{
			timer.stopTimer();
		}
		timer.updateObj ();
		if (rigidHand == null) {
			if(this.gameObject.transform.parent.childCount>1){
				this.gameObject.transform.parent.GetChild(1).parent = childsParent;
			}
			Destroy(this.transform.parent.gameObject);
			Destroy(timer.targetCopy);
		}

	}
}
