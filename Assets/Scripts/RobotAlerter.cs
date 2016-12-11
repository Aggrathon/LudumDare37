using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAlerter : MonoBehaviour {

	public LayerMask enemyLayer;
	public float alertRadius = 20f;

	static float lastTime;
	Collider[] buffer;

	void OnCollisionEnter(Collision col)
	{
		if (Time.time - lastTime < 2f)
			return;

		if (buffer == null)
			buffer = new Collider[20];
		RobotAI.Alert(transform.position);
		int count = Physics.OverlapSphereNonAlloc(transform.position, alertRadius, buffer, enemyLayer);
		for (int i = 0; i < count; i++)
		{
			buffer[i].GetComponent<RobotAI>().ForceCheckArea(transform.position);
		}
		lastTime = Time.time;
	}
}
