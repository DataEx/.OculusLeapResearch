using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorCubes : MonoBehaviour {

	float maxConnectingDistance = 0.33f;
	Color[] colors = new Color[] {Color.red, Color.magenta, Color.cyan};
	List<GameObject[]> handCubePairs = new List<GameObject[]>();

	List<GameObject> colorObjects = null;

	public void addGrabbableObject(GameObject obj){
		if (colorObjects == null) {
			colorObjects = new List<GameObject>();
		}
		colorObjects.Add(obj);
	}


	void updatePairs(){
		handCubePairs.RemoveAll (pairs => Vector3.Distance (pairs [0].transform.position, pairs [1].transform.position) > maxConnectingDistance);
	}




	void Update () {

//		updatePairs ();

		int colorIndex = 0;
		GameObject[] hands = GameObject.FindGameObjectsWithTag ("Hand");
		Transform[] cubeList = new Transform[this.gameObject.transform.childCount];
		for(int i=0;i<cubeList.Length;i++){
			cubeList[i] = this.gameObject.transform.GetChild(i);
		}

		for(int i=0;i<cubeList.Length;i++){
			cubeList[i].gameObject.renderer.material.color = Color.grey;
		}


		float[] distFromCube = new float[cubeList.Length];
		Transform closestCube;
		int closestIndex;

		for(int handIndex = 0; handIndex <hands.Length; handIndex++){
			if(hands[handIndex].transform.childCount > 0){
				continue;
			}
			closestIndex = 0;

			for(int i = 0; i<distFromCube.Length; i++){
				distFromCube[i] = Vector3.Distance(hands[handIndex].transform.position, cubeList[i].transform.position);
			}
			for(int i = 0; i<distFromCube.Length; i++){
				if(distFromCube[i] < distFromCube[closestIndex]){
					closestIndex = i;
				}
			}
			// Find the closest object to the hand
			closestCube = cubeList[closestIndex];
			print(closestCube);
			// If the object is close enough (below a threshold, color it)
			if (distFromCube [closestIndex] <= maxConnectingDistance) {
				if(closestCube.gameObject.renderer.material.color == Color.grey){
					closestCube.gameObject.renderer.material.color = colors[colorIndex];
					hands[handIndex].renderer.material.color = colors[colorIndex];
//					handCubePairs.Add(new GameObject[] {closestCube.gameObject,hands[handIndex]});
					print(colors[colorIndex].ToString());
					colorIndex = (colorIndex+1)%colors.Length;
				}
				else{
					hands[handIndex].renderer.material.color = colors[colorIndex] = closestCube.gameObject.renderer.material.color;
				}
			}
			else{
				closestCube.gameObject.renderer.material.color = Color.grey;
				hands[handIndex].renderer.material.color = Color.grey;

			}


		}

	}
}