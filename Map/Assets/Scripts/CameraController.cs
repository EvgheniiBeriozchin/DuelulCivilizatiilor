using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	public float panSpeed = 20f;
	public float panBorderThickness = 10f;
	public Vector2 panLimit;
	public float maxY = 30f;
	public float minY = 8f;
	public float scrollSpeed = 20f;
	bool isCameraActive = true;
	// Update is called once per frame
	void Update () {
		if (isCameraActive){
		Vector3 pos = transform.position;
		if (Input.GetKey ("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
		{
			pos.z += panSpeed * Time.deltaTime;
		}
		if (Input.GetKey ("a") || Input.mousePosition.x <= panBorderThickness) 
		{
			pos.x -= panSpeed * Time.deltaTime;
		}
		if (Input.GetKey ("s") || Input.mousePosition.y <= panBorderThickness) 
		{
			pos.z -= panSpeed * Time.deltaTime;
		}
		if (Input.GetKey ("d") || Input.mousePosition.x >= Screen.width - panBorderThickness) 
		{
			pos.x += panSpeed * Time.deltaTime;
		}

		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		pos.y -= scroll * scrollSpeed * 10f * Time.deltaTime;
			pos.x = Mathf.Clamp (pos.x, -panLimit.x, panLimit.x);
		pos.y = Mathf.Clamp (pos.y, minY, maxY);
			pos.z = Mathf.Clamp (pos.z, -panLimit.y, panLimit.y);
		transform.position = pos;
	}
	}
	public void OnCameraLock(){
		if (isCameraActive)
			isCameraActive = false;
		else
			isCameraActive = true;
	}
}
