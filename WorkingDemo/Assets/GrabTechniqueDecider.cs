using UnityEngine;
using System.Collections;

public class GrabTechniqueDecider : MonoBehaviour {

	public bool gapingGrab = false;
	public bool pinchGrab = false;
	public bool toggleGrab = false;
	int currentIndex = 0;

	void Start () {
	}
	
	// Update is called once per frame
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
