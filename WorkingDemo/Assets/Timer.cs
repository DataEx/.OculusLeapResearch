using UnityEngine;
using System.Collections;

public class Timer {

	public GameObject target = null;
	public float timerLength;
	public float scale;
	public GameObject targetCopy = null;
	public float startTime;
	public GameObject headerObj = GameObject.CreatePrimitive (PrimitiveType.Cube);
	public GameObject footerObj = GameObject.CreatePrimitive (PrimitiveType.Cube);
	public GameObject[] counterObjs;
	public bool[] isVisible; 
	public Transform parent = null;
	public bool makeVisible = false;
	int percentageComplete = 0;
	public GameObject outlineObj;

	public Timer(GameObject targetObj, float timerLen, float scalingNumber){
		outlineObj = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
		//outlineObj.transform.localScale = new Vector3(scale*1,scale*1,scale*0.5f);		

		target = targetObj;
//		targetCopy = (GameObject)Instantiate (target);
		targetCopy = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		targetCopy.renderer.enabled = false;

		timerLength = timerLen;
		scale = scalingNumber;
		headerObj.name = "Header";
		footerObj.name = "Footer";
		parent = headerObj.transform;
		isVisible = new bool[10]; //automatically all are set to false
		counterObjs = new GameObject[10];
		float xCoor = target.transform.position.z  + scale;
		for(int i = 0; i<counterObjs.Length; i++){
			counterObjs[i] = GameObject.CreatePrimitive (PrimitiveType.Cube);
			counterObjs[i].name = ((i+1)*10).ToString() + "% Complete";
			counterObjs[i].transform.localScale = new Vector3(scale*1,scale*1,scale*0.5f);		
			counterObjs[i].transform.position = new Vector3 (0, xCoor+(i+1)/10.0f*3.0f*scale, 0);
			counterObjs[i].renderer.enabled = true;
			counterObjs[i].transform.LookAt(targetCopy.transform,counterObjs[i].transform.position);
			counterObjs[i].transform.parent = targetCopy.transform;
		}
		footerObj.transform.localScale = new Vector3(scale*2.5f,scale*2.5f,scale*0.25f);
		headerObj.transform.localScale = new Vector3(scale*2.5f,scale*2.5f,scale*0.25f);
		headerObj.transform.position = new Vector3 (0, xCoor, 0);
		footerObj.transform.position = new Vector3 (0, xCoor+(11)/10.0f*3.0f*scale, 0);
		footerObj.transform.parent = targetCopy.transform;
//		outlineObj.transform.localScale = new Vector3(scale*1,scale*1,scale*0.5f);		
//		outlineObj.transform.position = new Vector3 (0, xCoor, 0);
//		outlineObj.transform.Rotate (new Vector3(180,0,0));

		parent.transform.LookAt(targetCopy.transform, parent.position);
		footerObj.transform.LookAt(targetCopy.transform, footerObj.transform.position);
		parent.transform.parent = targetCopy.transform;
		parent.renderer.enabled = true;
		footerObj.renderer.enabled = true;

		outlineObj.transform.LookAt(targetCopy.transform,outlineObj.transform.position);
		outlineObj.transform.parent = targetCopy.transform;
	}

	public void startTimer(){
		if(makeVisible == false){
			makeVisible = true;
			headerObj.renderer.enabled = true;
			footerObj.renderer.enabled = true;
			startTime = Time.realtimeSinceStartup;
			percentageComplete = 0;
		}
	}


	public void stopTimer(){
		makeVisible = false;
		if (headerObj.renderer.material.color == Color.green) {
			float currentTime = Time.realtimeSinceStartup - startTime;
			if(currentTime >= timerLength + 0.5f){
				for(int i = 0; i<targetCopy.transform.childCount; i++){
					targetCopy.transform.GetChild(i).gameObject.renderer.material.color = Color.grey;
					targetCopy.transform.GetChild(i).gameObject.renderer.enabled = false;
				}
			}
		}
		else{
			for(int i = 0; i<targetCopy.transform.childCount; i++){
				targetCopy.transform.GetChild(i).gameObject.renderer.material.color = Color.grey;
				targetCopy.transform.GetChild(i).gameObject.renderer.enabled = false;
			}
		}
		outlineObj.renderer.enabled = true;

	}
	
	public bool timerHasStarted(){
		return headerObj.renderer.enabled;
	}
	// Update is called once per frame
	public void updateObj () {
		targetCopy.transform.position = target.transform.position;
		targetCopy.transform.localScale = target.transform.localScale;
		targetCopy.transform.rotation = target.transform.rotation;
		if (timerHasStarted() == true) {
			float currentTime = Time.realtimeSinceStartup - startTime;
			if (currentTime < timerLength) {
				float percentageDone = currentTime / (timerLength) * 10.0f;

				int roundedPerc = (int)percentageDone;
				if(roundedPerc > percentageComplete){
					percentageComplete = roundedPerc;
					Debug.Log(percentageDone);
					counterObjs[percentageComplete].renderer.enabled = true;
					if(percentageComplete == counterObjs.Length -1){
						for(int i = 0; i<targetCopy.transform.childCount; i++){
							targetCopy.transform.GetChild(i).gameObject.renderer.material.color = Color.green;
						}
					}
				}
			}
		}
	}
}
