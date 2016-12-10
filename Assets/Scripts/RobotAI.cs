using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotAI : MonoBehaviour {

	public Transform patrol;
	public float patrolDistance = 4f;
	public bool patrolForward = true;
	public int patrolFirstPoint = 0;

	NavMeshAgent navAgent;
	int patrolPosition;

	void Start()
	{
		navAgent = GetComponent<NavMeshAgent>();
		patrolPosition = patrolFirstPoint % patrol.childCount;
		navAgent.SetDestination(patrol.GetChild(patrolPosition).position);
	}

	void Update()
	{
		if(Vector3.Distance(transform.position,patrol.GetChild(patrolPosition).position) < patrolDistance)
		{
			if(patrolForward)
				patrolPosition = (patrolPosition + 1) % patrol.childCount;
			else
				patrolPosition = (patrolPosition + patrol.childCount - 1) % patrol.childCount;
			navAgent.SetDestination(patrol.GetChild(patrolPosition).position);
		}
	}



}
