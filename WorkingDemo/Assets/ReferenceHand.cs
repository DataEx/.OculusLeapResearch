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
///	public Tranform attachedObj; 
	// Use this for initialization
	void Awake () {
		float timerLength = float.Parse(GameObject.Find ("TimerLenValue").GetComponent<Text> ().text.ToString ());
		timer = new Timers (this.gameObject,timerLength,0.6f);
	}
	
	// Update is called once per frame
	void Update () {
		if (isCounting == true) {
//			print ("START");
			timer.startTimer();
		}
		else{
//			print ("STOP");
			timer.stopTimer();
		}
		timer.updateObj ();
		if (rigidHand == null) {
			if(this.gameObject.transform.childCount>0){
				this.gameObject.transform.GetChild(0).parent = childsParent;
			}
			Destroy(this.gameObject);
			Destroy(timer.targetCopy);
		}

	}

	void onDestroy(){
		//Destroy (this.gameObject.GetComponent<ObjectTimer> ());
	}
}
