using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RobotAlerter : MonoBehaviour {

	public LayerMask enemyLayer;
	public float alertRadius = 20f;
	public AudioClip pickupSound;
	public AudioClip collisionSound;

	static float lastTime;
	Collider[] buffer;

	void OnCollisionEnter(Collision col)
	{
		if (Time.time - lastTime < 2f)
			return;

		if(collisionSound != null)
		{
			GetComponent<AudioSource>().PlayOneShot(collisionSound);
		}

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
	
	void OnTriggerEnter(Collider col)
	{
		if (col.transform.CompareTag("Player"))
		{
			GameState.instance.player.AddThrowable();
			GetComponent<AudioSource>().PlayOneShot(pickupSound);
			enabled = false;
			Destroy(gameObject, pickupSound.length);
		}
	}
}
