using UnityEngine;
using System.Collections;
public class lookAtViewer : MonoBehaviour {
	public GameObject target = null;
	//Transform screen = null;
	// Use this for initialization
	void Awake(){
		target = GameObject.Find ("CameraLeft");
//		Vector3 ls = this.transform.localScale;
//		Vector3 lp = this.transform.localPosition;
//		Quaternion lr = this.transform.localRotation;

//		this.transform.parent = target.transform;
//		
//		this.transform.localScale = ls;
//		this.transform.localRotation = lr;
//		GameObject screen = GameObject.Find ("OVRGuiObjectMain(Clone)");
//		this.transform.parent = screen.transform.parent;
//
//		float ipdOffsetDirection = 1.0f;
//		Transform guiParent = this.transform.parent;
//		if (guiParent != null)
//		{
//			OVRCamera ovrCamera = guiParent.GetComponent<OVRCamera>();
//			if (ovrCamera != null && ovrCamera.RightEye)
//				ipdOffsetDirection = -1.0f;
//		}
//		
//		float ipd = 0.0f;
//		CameraController.GetIPD(ref ipd);
//		lp.x += ipd * 0.5f * ipdOffsetDirection;
//		GUIRenderObject.transform.localPosition = lp;

	}

	void Update () {
		for(int i = 0; i<target.transform.childCount; i++){
			if(target.transform.GetChild(i).name == "OVRGuiObjectMain(Clone)"){
			//	screen
			}
		}
	//	float dist = Vector3.Distance (this.transform.position, target.transform.position); 
	//	print (dist);
//		Vector3 parentPos = this.transform.parent.position;
//		this.transform.position = new Vector3 (parentPos.x + 3, parentPos.y, parentPos.x);
//		this.transform.LookAt(this.transform.parent);
		//}
//		transform.LookAt (target);
	}
}
