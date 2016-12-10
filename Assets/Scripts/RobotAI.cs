using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class RobotAI : MonoBehaviour {

	enum State
	{
		Patrolling,
		Hunting,
		Firing
	}

	[Header("Patrol")]
	public Transform patrol;
	public float patrolDistance = 4f;
	public bool patrolForward = true;
	public int patrolFirstPoint = 0;
	int patrolPosition;

	[Header("Tracking")]
	public float trackingAngle = 80f;
	public float trackingDistance = 30f;
	public float headRotation = 180f;
	public Transform trackingTarget;
	public Transform trackingHead;
	Vector3 lastTargetPosition;

	[Header("Firing")]
	public float fireCooldown = 5f;
	public float fireRange = 10f;
	float lastFire;

	[Header("Sounds")]
	public AudioClip audioLostTracking;
	public AudioClip audioTracking;
	public AudioClip audioChargeUp;
	public AudioClip audioExplosion;
	new AudioSource audio;

	NavMeshAgent navAgent;
	State state;

	void Start()
	{
		navAgent = GetComponent<NavMeshAgent>();
		audio = GetComponent<AudioSource>();
		patrolPosition = patrolFirstPoint % patrol.childCount;
		navAgent.SetDestination(patrol.GetChild(patrolPosition).position);
		state = State.Patrolling;
	}

	void Update()
	{
		switch (state)
		{
			case State.Patrolling:
				if (CheckVisible())
				{
					HuntState();
					return;
				}
				if (Vector3.Distance(transform.position, patrol.GetChild(patrolPosition).position) < patrolDistance)
				{
					if (patrolForward)
						patrolPosition = (patrolPosition + 1) % patrol.childCount;
					else
						patrolPosition = (patrolPosition + patrol.childCount - 1) % patrol.childCount;
					navAgent.SetDestination(patrol.GetChild(patrolPosition).position);
				}
				trackingHead.localRotation = Quaternion.RotateTowards(trackingHead.localRotation, new Quaternion(0,0,0,1),headRotation*Time.deltaTime);
				break;

			case State.Hunting:
				if((trackingTarget.position-lastTargetPosition).sqrMagnitude > 2f && CheckVisible())
				{
					navAgent.SetDestination(lastTargetPosition);
				}
				Vector3 dir = lastTargetPosition - transform.position;
				dir.y = 0;
				float magn = dir.sqrMagnitude;
				if (Time.time-lastFire > fireCooldown && magn < fireRange*fireRange && CheckVisible())
				{
					FireState();
				}
				else if(magn < 4f)
				{
					if((trackingTarget.position-transform.position).sqrMagnitude < 4f)
					{
						lastTargetPosition = trackingTarget.position;
						navAgent.SetDestination(lastTargetPosition);
					}
					else
					{
						PatrolState();
						return;
					}
				}
				if(dir.x == 0 && dir.y == 0)
				{
					return;
				}
				trackingHead.rotation = Quaternion.RotateTowards(trackingHead.rotation, Quaternion.LookRotation(dir, Vector3.up), headRotation*Time.deltaTime);
				break;

			case State.Firing:
				if(Time.time-lastFire<audioChargeUp.length)
				{
					CheckVisible();
					dir = lastTargetPosition - transform.position;
					dir.y = 0;
					trackingHead.rotation = Quaternion.RotateTowards(trackingHead.rotation, Quaternion.LookRotation(dir, Vector3.up), headRotation * Time.deltaTime);
				}
				else
				{
					audio.clip = audioExplosion;
					audio.Play();
					lastFire = Time.time+audioExplosion.length+0.1f;
					StartCoroutine(AfterFire());


				}
				break;
		}

	}

	bool CheckVisible()
	{
		Vector3 direction = trackingTarget.position - trackingHead.position;
		if (direction.sqrMagnitude < trackingDistance * trackingDistance)
		{
			if (Vector3.Angle(direction, trackingHead.forward) < trackingAngle)
			{
				RaycastHit hit;
				if (Physics.Raycast(trackingHead.position+trackingHead.forward, direction, out hit, trackingDistance))
				{
					if (hit.collider.CompareTag("Player"))
					{
						lastTargetPosition = hit.transform.position;
						return true;
					}
				}
			}
		}
		return false;
	}

	void FireState()
	{
		if (state != State.Firing)
		{
			navAgent.Stop();
			audio.Stop();
			audio.loop = false;
			audio.clip = audioChargeUp;
			audio.Play();
			state = State.Firing;
			lastFire = Time.time;
		}
	}

	void HuntState()
	{
		if (state != State.Hunting)
		{
			audio.Stop();
			audio.loop = true;
			audio.clip = audioTracking;
			audio.Play();
			state = State.Hunting;
		}
		navAgent.SetDestination(lastTargetPosition);
	}

	void PatrolState()
	{
		if (state != State.Patrolling)
		{
			audio.Stop();
			audio.loop = false;
			audio.clip = audioLostTracking;
			audio.Play();
			state = State.Patrolling;
			navAgent.SetDestination(patrol.GetChild(patrolPosition).position);
		}
	}

	IEnumerator AfterFire()
	{
		yield return new WaitForSeconds(audioExplosion.length);
		navAgent.Resume();
		if (CheckVisible())
			HuntState();
		else
			PatrolState();
	}
}
