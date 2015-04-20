/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;

public class PressAnyKeyToReset : MonoBehaviour {
	KeyCode[] allowedHits = {KeyCode.Slash, KeyCode.Comma, KeyCode.Period};
	void OnGUI() {          
		//Input.GetKeyDown(KeyCode.Comma)
//		bool reset = true;
//		for (int i = 0; i<allowedHits.Length; i++) {
//			if(Input.GetKeyDown(allowedHits[i])){
//				reset = false;
//			}
//		}
//		if (reset == true) {
//			Application.LoadLevel (Application.loadedLevel);
//		}
		if (Event.current.type == EventType.KeyDown){
			bool cont = true;
			for(int i = 0; i<allowedHits.Length; i++){
					if(Input.GetKey(allowedHits[i]) == true){
						cont = false;
					}
			} 
			if (cont == true) {
				Application.LoadLevel (Application.loadedLevel);
			}
		} 
	}	 
}
