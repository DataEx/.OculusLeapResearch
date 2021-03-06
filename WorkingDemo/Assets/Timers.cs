﻿using UnityEngine;
using System.Collections;

public class Timers {
	
	public GameObject pointer = null;
	public GameObject target = null;
	public float timerLength;
	public float scale;
	public GameObject targetCopy = null;
	public float startTime;
//	public GameObject headerObj = GameObject.CreatePrimitive (PrimitiveType.Cube);
//	public GameObject footerObj = GameObject.CreatePrimitive (PrimitiveType.Cube);
	public GameObject[] counterObjs;
	public bool[] isVisible; 
	public Transform parent = null;
	public bool makeVisible = false;

	int percentageComplete = 0;
	//public GameObject outlineObj; 
	public Timers(GameObject targetObj, float timerLen, float scalingNumber){
		counterObjs = new GameObject[10];
		target = targetObj;
		targetCopy = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		targetCopy.renderer.enabled = false;		
		targetCopy.collider.enabled = false;
		timerLength = timerLen;
		scale = scalingNumber;
		float xCoor = target.transform.position.z  + scale;
		for (int i = 0; i<counterObjs.Length; i++) {
			counterObjs[i] = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			counterObjs[i].collider.enabled = false;
			counterObjs[i].transform.eulerAngles	= new Vector3(90,90,0);
			counterObjs[i].transform.position	 	= new Vector3 ((-5+i)*0.25f, xCoor,0);
			counterObjs[i].transform.localScale 	= new Vector3 (counterObjs[i].transform.localScale.x*0.3f, counterObjs[i].transform.localScale.y*0.1f,counterObjs[i].transform.localScale.z*0.3f);
			counterObjs[i].transform.parent = targetCopy.transform;
			counterObjs[i].renderer.material.color  = Color.grey;
		}
		pointer = GameObject.Find ("CameraLeft");
	}
	
	public void startTimer(){
		if(makeVisible == false){
			makeVisible = true;
			startTime = Time.realtimeSinceStartup;
			percentageComplete = 0;
			for(int i = 0; i<counterObjs.Length; i++){
				counterObjs[i].renderer.material.color = Color.grey;
				counterObjs[i].renderer.enabled = true;
			}
		}
	}
	
	
	public void stopTimer(){
		makeVisible = false;
		if (counterObjs[counterObjs.Length-1].renderer.material.color == Color.green) {
			float currentTime = Time.realtimeSinceStartup - startTime;
			if(currentTime >= timerLength + 0.5f){
				for(int i = 0; i<counterObjs.Length; i++){
					counterObjs[i].renderer.material.color = Color.grey;
					counterObjs[i].renderer.enabled = false;
				}
			}
		}
		else{
			for(int i = 0; i<counterObjs.Length; i++){
				counterObjs[i].renderer.material.color = Color.grey;
				counterObjs[i].renderer.enabled = false;
			}
		}
	}
	
	public bool timerHasStarted(){
		return makeVisible;//headerObj.renderer.enabled;
	}
	// Update is called once per frame
	public void updateObj () {
		targetCopy.transform.position = target.transform.position;
		targetCopy.transform.localScale = target.transform.localScale;
		targetCopy.transform.rotation = target.transform.rotation;
		if (pointer != null) {
			targetCopy.transform.LookAt(pointer.transform);
		}
		if (timerHasStarted() == true) {
			float currentTime = Time.realtimeSinceStartup - startTime;
			if (currentTime < timerLength) {
				float percentageDone = currentTime / (timerLength) * 10.0f;
				
				int roundedPerc = (int)percentageDone;
				if(roundedPerc >= percentageComplete){
					percentageComplete = roundedPerc;
					counterObjs[counterObjs.Length - 1 - percentageComplete].renderer.material.color = Color.green;
					if(percentageComplete == counterObjs.Length -1){
						for(int i = 0; i<counterObjs.Length; i++){
							counterObjs[i].renderer.material.color = Color.green;
						}
					}
				}
			}
		}
	}
}
