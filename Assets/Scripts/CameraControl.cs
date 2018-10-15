using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	float rotationX = 0F;
	float rotationY = 0F;
	Quaternion originalRotation;
	public float mvoeSpeed = 0.5f;

	void Update() {
		if (Input.GetMouseButton(1)) {
			if (axes == RotationAxes.MouseXAndY) {
				//Gets rotational input from the mouse
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;

				//Get the rotation you will be at next as a Quaternion
				Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
				Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);

				//Rotate
				transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			} else if (axes == RotationAxes.MouseX) {
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
				transform.localRotation = originalRotation * xQuaternion;
			} else {
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
				transform.localRotation = originalRotation * yQuaternion;
			}
		}
		if (Input.GetMouseButton(2)) {
			this.transform.position -= transform.right * Input.GetAxis("Mouse X") * 2;
			this.transform.position -= transform.up * Input.GetAxis("Mouse Y") * 2;
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
			this.transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * 10;
		}
		var mvoeSpeed = this.mvoeSpeed;
		if (Input.GetKey(KeyCode.LeftShift)) {
			mvoeSpeed /= 10;
		}
		if (Input.GetKey(KeyCode.W)) {
			this.transform.position += transform.forward * Time.timeScale * mvoeSpeed;
		}
		if (Input.GetKey(KeyCode.A)) {
			this.transform.position -= transform.right * Time.timeScale * mvoeSpeed;
		}
		if (Input.GetKey(KeyCode.S)) {
			this.transform.position -= transform.forward * Time.timeScale * mvoeSpeed;
		}
		if (Input.GetKey(KeyCode.D)) {
			this.transform.position += transform.right * Time.timeScale * mvoeSpeed;
		}
		if (Input.GetKey(KeyCode.Q)) {
			this.transform.position += Vector3.up * Time.timeScale * mvoeSpeed;
		}
		if (Input.GetKey(KeyCode.E)) {
			this.transform.position += Vector3.down * Time.timeScale * mvoeSpeed;
		}
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

    }

	void Start() {
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb)
			rb.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
}
