using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrabTechniqueDecider : MonoBehaviour {

	public bool gapingGrab = false;
	public bool pinchGrab = false;
	public bool toggleGrab = false;

	public List<GameObject> GrabbableObjects;
	int currentIndex = 0;

	// The decider for which grasping to use, based on in-editor toggle
	void Update () {
		bool[] techniqueList = {gapingGrab, pinchGrab, toggleGrab}; 
		for(int i = 0; i<techniqueList.Length; i++){
			if(techniqueList[i] == true && currentIndex != i){
				techniqueList[currentIndex] = false;
				currentIndex = i;
				break;
			}
			else if(techniqueList[i] == false && currentIndex == i){
				techniqueList[i] = true;
				break;
			}
		}
		gapingGrab  = techniqueList [0];
		pinchGrab 	= techniqueList [1];
		toggleGrab  = techniqueList [2]; 

	}
}
