using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FPSController : MonoBehaviour {

	CharacterController cc;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), 0);
		cc.SimpleMove(transform.rotation*new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))*3f);
		Camera.main.transform.rotation *= Quaternion.Euler(-Input.GetAxis("Mouse Y"), 0, 0);
	}
}
