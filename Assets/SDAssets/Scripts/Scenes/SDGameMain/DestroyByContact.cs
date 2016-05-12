﻿/* 
 * File Name: DestroyByContact.cs
 * Description: Destroys attached objects if they collide with the player.
 */

using UnityEngine;
using System.Collections;

namespace SD {
    public class DestroyByContact : MonoBehaviour {

        private GameController gameController;
        private const int newScoreValue = 10; // Score to be recieved by eating prey

        // Use this for initialization
        void Start () {
            gameController = GameController.getInstance ();
        }
        
        // Update is called once per frame
        void Update () {
        
        }

        // Destroys the attached object upon a collison with the player
        void OnTriggerEnter(Collider other) {
            if (other.tag == "Player" || other.tag == "Predator") {
                int npcFishId = gameObject.GetComponentInParent<NPCFish>().id;
                int npcFishSpeciesId = gameObject.GetComponentInParent<NPCFish>().speciesId;
                Debug.Log ("Consumed prey with ID: " + npcFishId);
                if (SDMain.networkManager != null) {
                    GameManager.getInstance ().DestroyNPCFish (npcFishId, npcFishSpeciesId);
                }
                gameController.destroyPrey (npcFishId);
                gameController.AddUnscoredPoint (newScoreValue);
            }
        }
    }
}
