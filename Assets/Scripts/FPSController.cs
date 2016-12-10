using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FPSController : MonoBehaviour {

	CharacterController cc;
	
	void Start () {
		cc = GetComponent<CharacterController>();
		OnApplicationFocus(true);
	}
	
	void Update ()
	{
		transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), 0);
		cc.SimpleMove(transform.rotation*new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))*3f);
		Camera.main.transform.rotation *= Quaternion.Euler(-Input.GetAxis("Mouse Y"), 0, 0);
	}

	void OnApplicationFocus(bool hasFocus)
	{
		Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = hasFocus;
	}

	void OnDestroy()
	{
		OnApplicationFocus(false);
	}
}
