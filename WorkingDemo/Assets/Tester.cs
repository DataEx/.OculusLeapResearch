using UnityEngine;
using System.Collections;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//GameObject obj = new GameObject ();
		this.gameObject.AddComponent<ObjectTimer>();
		this.gameObject.GetComponent<ObjectTimer>().target = GameObject.Find("Sphere").transform;
		//		ObjectTimer a = new ObjectTimer ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
