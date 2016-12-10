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
		Hunting
	}

	[Header("Patrol")]
	public Transform patrol;
	public float patrolDistance = 4f;
	public bool patrolForward = true;
	public int patrolFirstPoint = 0;

	[Header("Tracking")]
	public float trackingAngle = 80f;
	public float trackingDistance = 20f;
	public float headRotation = 180f;
	public Transform trackingTarget;
	public Transform trackingHead;

	[Header("Sounds")]
	public AudioClip audioLostTracking;
	public AudioClip audioTracking;

	NavMeshAgent navAgent;
	new AudioSource audio;
	State state;
	int patrolPosition;
	Vector3 lastTargetPosition;

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
				if((trackingTarget.position-lastTargetPosition).sqrMagnitude > 2f)
				{
					CheckVisible();
				}
				if((lastTargetPosition-transform.position).sqrMagnitude < 4f)
				{
					if((trackingTarget.position-transform.position).sqrMagnitude < 2f || CheckVisible())
					{
						lastTargetPosition = trackingTarget.position;
					}
					else
					{
						state = State.Patrolling;
						audio.Stop();
						audio.loop = false;
						audio.clip = audioLostTracking;
						audio.Play();
						navAgent.SetDestination(patrol.GetChild(patrolPosition).position);
						Debug.Log(name + ": Lost Track");
						return;
					}
				}
				Vector3 dir = lastTargetPosition - transform.position;
				dir.y = 0;
				if(dir.x == 0 && dir.y == 0)
				{
					return;
				}
				trackingHead.rotation = Quaternion.RotateTowards(trackingHead.rotation, Quaternion.LookRotation(dir, Vector3.up), headRotation*Time.deltaTime);
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
						navAgent.SetDestination(lastTargetPosition);
						if(state != State.Hunting)
						{
							audio.Stop();
							audio.loop = true;
							audio.clip = audioTracking;
							audio.Play();
							state = State.Hunting;
						}
						return true;
					}
				}
			}
		}
		return false;
	}

}
