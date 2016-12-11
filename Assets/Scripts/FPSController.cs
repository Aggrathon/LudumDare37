using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour {

	public float speed = 3.5f;

	CharacterController cc;
	
	void Start () {
		cc = GetComponent<CharacterController>();
	}
	
	void Update ()
	{
		float moveSpeed = speed;
		if (Input.GetButton("Sprint"))
			moveSpeed *= 3f;
		transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), 0);
		cc.SimpleMove(transform.rotation*new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized*moveSpeed);
		Camera.main.transform.rotation *= Quaternion.Euler(-Input.GetAxis("Mouse Y"), 0, 0);
	}
}
