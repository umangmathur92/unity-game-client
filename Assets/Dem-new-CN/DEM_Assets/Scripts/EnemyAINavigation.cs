using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// make this a component of the Enemy Animal AI prefab

public class EnemyAINavigation : MonoBehaviour {

	// Set the transform to the tree of life prefab in Unity Inspector
	public Transform locationTreeOfLife;
	public TreeOfLifeBehavior treeBehavior;

	private EnemyBehavior behavior;
	private NavMeshAgent agent;
	float distance;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		agent.destination = locationTreeOfLife.position;
		behavior = this.gameObject.GetComponent<EnemyBehavior>();
	}
		




	// to be done in every frame
	void Update() {
		
		// distance from enemy to the tree of life
		distance = Vector3.Distance(agent.transform.position, locationTreeOfLife.position);

		// check if enemy has reached the tree of life
		if (distance <= 3.0) 
		{
			agent.isStopped = true;

			if (treeBehavior != null) {
				treeBehavior.ReactToHit ();
			}

			if (behavior != null) {
				EnemyController.numberOfEnemies--;
				behavior.ReactToHit ();
			}
		}

	}




}
