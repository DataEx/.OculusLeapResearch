using UnityEngine;
using System.Collections;

public class ColorMe : MonoBehaviour {

	float maxConnectingDistance = 0.33f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		GameObject[] hands = GameObject.FindGameObjectsWithTag ("Hand");
		GameObject   cube  = this.gameObject;

		if (hands.Length > 0) {
			float minDistance = Vector3.Distance(cube.transform.position, hands[0].transform.position);
			for(int i = 0; i<hands.Length; i++){
				float distance = Vector3.Distance(cube.transform.position, hands[i].transform.position);
				if(distance < minDistance){
					minDistance = distance;
				} 
			}
			if(minDistance <=  maxConnectingDistance){
				cube.gameObject.renderer.material.color = Color.red;
			}
			else{
				cube.gameObject.renderer.material.color = Color.grey;
			}
		}
		else{
			cube.gameObject.renderer.material.color = Color.grey;
		}	
	}
}
