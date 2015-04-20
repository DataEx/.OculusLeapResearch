using UnityEngine;
using System.Collections;

public class CenterCamera : MonoBehaviour {
	GameObject leftCamera  = null;
	GameObject rightCamera = null;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (leftCamera == null) {
			leftCamera = GameObject.Find("CameraLeft");
		}
		if (rightCamera == null) {
			rightCamera = GameObject.Find("CameraRight");
		}
		if (leftCamera != null && rightCamera != null) {
			this.gameObject.transform.position = new Vector3(
				leftCamera.transform.position.x + rightCamera.transform.position.x,
				leftCamera.transform.position.y + rightCamera.transform.position.y,
				leftCamera.transform.position.z + rightCamera.transform.position.z
				);
		}

	}
}
