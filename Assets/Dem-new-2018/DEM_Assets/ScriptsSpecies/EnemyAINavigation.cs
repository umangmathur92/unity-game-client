﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// make this a component of the Enemy Animal AI prefab

public class EnemyAINavigation : MonoBehaviour {

	// Set the transform to the tree of life prefab in Unity Inspector
	public Transform locationTreeOfLife;
	private EnemyBehavior behavior;
	private NavMeshAgent agent;
	private float distance;
	private GameObject treeOfLife;
	private bool treeOfLifeHit;

	void Awake () {
		agent = GetComponent<NavMeshAgent>();
		agent.destination = locationTreeOfLife.position;
		treeOfLifeHit = false;
		behavior = this.gameObject.GetComponent<EnemyBehavior>();
		treeOfLife = GameObject.Find("TreeOfLife");
	}

	// Use this for initialization
	void Start () {
		
	}
		


	// to be done in every frame
	void Update() {
		// distance from enemy to the tree of life
		distance = Vector3.Distance(agent.transform.position, locationTreeOfLife.position);

		// check if enemy has reached the tree of life
		if (distance <= 6.0 && !treeOfLifeHit) 
		{
			agent.isStopped = true;

			if (treeOfLife != null) {
				treeOfLifeHit = true;
				treeOfLife.GetComponent<TreeOfLifeBehavior> ().reactToHit ();
			}

			if (behavior != null) {
				EnemyController.numberOfEnemies--;
				behavior.ReactToHit ();
			}
		}

	}




}
