using UnityEngine;
using System.Collections;

public class ObjectTimer  : MonoBehaviour{

	public GameObject headerObj = GameObject.CreatePrimitive (PrimitiveType.Cube);
	public GameObject footerObj = GameObject.CreatePrimitive (PrimitiveType.Cube);
	public GameObject[] counterObjs;
	public bool[] isVisible; 
	public float startTime = Time.realtimeSinceStartup;
	public float endTime = 1;
	public int currentPerc = 0;
	public Transform target;
	public GameObject parent; // all objects are child of this
	public float scale = 1;
	public bool reset = false;

	
	void Awake(){
		headerObj.name = "Header";
		footerObj.name = "Footer";
		headerObj.collider.enabled = false;
		footerObj.collider.enabled = false;

		parent = headerObj;
		headerObj.transform.localScale = new Vector3(scale*2.5f,scale*2.5f,scale*0.25f);		
		footerObj.transform.localScale = new Vector3(scale*2.5f,scale*2.5f,scale*0.25f);
		isVisible = new bool[10]; //automatically all are set to false
		counterObjs = new GameObject[10];
		for(int i = 0; i<counterObjs.Length; i++){
			counterObjs[i] = GameObject.CreatePrimitive (PrimitiveType.Cube);
			counterObjs[i].name = ((i+1)*10).ToString() + "% Complete";
			counterObjs[i].transform.localScale = new Vector3(scale*1,scale*1,scale*0.5f);		
			counterObjs[i].renderer.enabled = false;
			counterObjs[i].collider.enabled = false;
			counterObjs[i].transform.parent = parent.transform;
		}
		footerObj.transform.parent = parent.transform;

	}

	// Use this for initialization
	void Start () {
		//Debug.Log (target);
		float xCoor = target.transform.position.z  +  10;
		headerObj.transform.position = new Vector3 (0, 0, xCoor);
		footerObj.transform.position = new Vector3 (0, 0, xCoor + 10);
		for(int i = 0; i<counterObjs.Length; i++){
			counterObjs[i].transform.position = new Vector3 (0, 0, xCoor + i);
		}
		parent.transform.LookAt (target, parent.transform.position);
		parent.transform.parent = target.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (reset == false) {
			currentPerc = 0;
			headerObj.renderer.enabled = false;
			footerObj.renderer.enabled = false;
		}
		else{
			if(currentPerc == 0){
				headerObj.renderer.enabled = true;
				footerObj.renderer.enabled = true;
				startTime = Time.realtimeSinceStartup;
				float xCoor = target.transform.position.z  +  10;
				headerObj.transform.position = new Vector3 (0, 0, xCoor);
				footerObj.transform.position = new Vector3 (0, 0, xCoor + 10);
				for(int i = 0; i<counterObjs.Length; i++){
					counterObjs[i].transform.position = new Vector3 (0, 0, xCoor + i);
					counterObjs[i].renderer.enabled = false;
				}
				parent.transform.LookAt (target, parent.transform.position);
				parent.transform.parent = target.transform;

			}
			float currentTime = Time.realtimeSinceStartup - startTime;
			if (currentTime < startTime + endTime) {
				float percentageDone = currentTime / (startTime + endTime) * 10.0f;
				int roundedPerc = (int)percentageDone;
				if(roundedPerc > currentPerc){
					currentPerc = roundedPerc;
					counterObjs[counterObjs.Length - currentPerc].renderer.enabled = true;
				}
			}
		}
		//}
	}

	void onDestroy(){
		Destroy (parent);
	}
}
