using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {

	// black board
	public BlackBoard blackboard;

	// island
	public GameObject island;// ASSIGN IN EDITOR set in editor

	// platform
	public GameObject platform;// ASSIGN IN EDITOR set in editor
	public int platformLevel;// ASSIGN IN EDITOR set in editor

	// payments
		// fish prefabs
	public Transform fish;

		// waypoints are unavailable when they're either purchased or occupied
	public int cost = 1;
	public bool purchased = false;
	public bool occupied = false;
	public List<GameObject> storedFish = new List<GameObject>();
	public int numFish = 0;
	private float fishZPosition = -3.0f;

	private bool callingNPC = false;

	void Start(){
		InitWaypoint ();
	}
	void InitWaypoint(){
		// if instantiated during runtime then set the blackboard.... probably set a few other things as well
		if (blackboard == null) {
			blackboard = GameObject.Find ("BlackBoard").GetComponent<BlackBoard> ();
		}

		// determine the type of Waypoint && setup inital blackboard stuff
		// ???? do dynamic operations in individual scripts, purchased/occupied etc.
		if (gameObject.tag == "TreeSpot") {
			blackboard.AddGameObjectToList (blackboard.activatedTrees, gameObject);
		}else if(gameObject.tag == "FishingSpot"){
			blackboard.AddGameObjectToList (blackboard.activatedFishingSpots, gameObject);
		}else if (gameObject.tag == "FishingRod"){
			blackboard.AddGameObjectToList (blackboard.availableFishingRods, gameObject);
		}

	}

	public void RemoveFromBlackBoardList(){
		if (gameObject.tag == "TreeSpot") {
			blackboard.RemoveGameObjectToList (blackboard.activatedTrees, gameObject);
		}else if(gameObject.tag == "FishingSpot"){
			blackboard.RemoveGameObjectToList (blackboard.activatedFishingSpots, gameObject);
		}else if (gameObject.tag == "FishingRod"){
			blackboard.RemoveGameObjectToList (blackboard.availableFishingRods, gameObject);
		}
	}

	public void AddToBlackBoardList(){
		if (gameObject.tag == "TreeSpot") {
			blackboard.AddGameObjectToList (blackboard.activatedTrees, gameObject);
		}else if(gameObject.tag == "FishingSpot"){
			blackboard.AddGameObjectToList (blackboard.activatedFishingSpots, gameObject);
		}else if (gameObject.tag == "FishingRod"){
			blackboard.AddGameObjectToList (blackboard.availableFishingRods, gameObject);
		}else if (gameObject.tag == "Wall"){
			blackboard.AddGameObjectToList (blackboard.activatedWalls, gameObject);
		}
	}

	void Update(){
		if (callingNPC) {
			CallNPC ();
		}
	}

	// make payments by player
	public bool AddFish(){
		if (!purchased) {
			// TODO
			// store fish prefab dimensions in blackboard
			Bounds fishPrefabBounds = fish.GetComponent<Renderer> ().bounds;
			float fishPositionX = transform.position.x - (fishPrefabBounds.size.x);
			float fishPositionY = transform.position.y - (numFish * fishPrefabBounds.size.y);
			float fishPositionZ = fishZPosition;
			// TODO
			// in the case of paying to open a fishing spot this fish is not returned..., i.e. not added to storedFish List, maxFishStored etc.
			Transform newFish = GameObject.Instantiate (fish, new Vector3 (fishPositionX, fishPositionY, fishPositionZ), Quaternion.identity) as Transform;
			storedFish.Add (newFish.gameObject);
			numFish += 1;
			if (numFish >= cost) {
				purchased = true;

//				CallNPC ();
				if (gameObject.tag == "TreeSpot" || gameObject.tag == "FishingRod") {
					RemoveFromBlackBoardList ();// remove trees and fishing rods because they are calling NPCs and actually have NPCs
					CallNPC ();
				}else if(gameObject.tag == "FishingSpot"){
					AddToBlackBoardList ();// for fishing spot its being opened without necessarily having an NPC to it needs to be added to List
//					Debug.Log ("Only call NPCs with fishing rods");
					CallNPCWithRod ();
				}else if (gameObject.tag == "Wall"){
//					Debug.Log ("Paid for walllllllll");
					gameObject.GetComponent<Wall> ().GrowWall ();
					AddToBlackBoardList ();// purchased walls are added to blackboard list so that npcs can see if
				}
				return true;
			}
		}
		return false;
	}

	// cancel payments form player
	public void RemoveFish(){
		if (gameObject.tag == "TreeSpot" || gameObject.tag == "Wall" || gameObject.tag == "Boat") {
			if (storedFish.Count > 0) {
				int idx = storedFish.Count - 1;
				Destroy (storedFish [idx]);
				storedFish.RemoveAt (idx);
				numFish -= 1;
			}
		}
	}

	public void CallNPC(){
//		Debug.Log ("Call NPC");
//		Debug.Log ("waypointGameObject's platform: " + platformLevel);

		// TODO
		// move this out of this function and maintain a list of NPCs on island separately to avoid making this Find() call with each pickup
		GameObject[] NPCs = GameObject.FindGameObjectsWithTag ("NPC");

		bool npcCondition = true;

		GameObject closestNPC = null;
		float sqrLen = Mathf.Infinity;
		NPC npcScript = null;
		for (int i = 0; i < NPCs.Length; i++) {
			NPC potentialNPCScript = NPCs [i].GetComponent<NPC> ();
			if (!potentialNPCScript.isActive) {
				Vector3 offset = NPCs [i].transform.position - transform.position;
				float newSqrLen = offset.sqrMagnitude;
				if (newSqrLen < sqrLen) {
					closestNPC = NPCs [i];
					npcScript = potentialNPCScript;
				}
			}

		}
		if (npcScript != null && closestNPC != null) {
			npcScript.targetObject = gameObject;
			npcScript.moveToTarget = true;
			npcScript.isActive = true;
			callingNPC = false;
		}
	}

	public void CallNPCWithRod(){
		// TODO
		// move this out of this function and maintain a list of NPCs on island separately to avoid making this Find() call with each pickup
		GameObject[] NPCs = GameObject.FindGameObjectsWithTag ("NPC");

		GameObject closestNPC = null;
		float sqrLen = Mathf.Infinity;
		NPC npcScript = null;
		for (int i = 0; i < NPCs.Length; i++) {
			NPC potentialNPCScript = NPCs [i].GetComponent<NPC> ();
			if (!potentialNPCScript.isActive && potentialNPCScript.hasFishingRod) {
				Vector3 offset = NPCs [i].transform.position - transform.position;
				float newSqrLen = offset.sqrMagnitude;
				if (newSqrLen < sqrLen) {
					closestNPC = NPCs [i];
					npcScript = potentialNPCScript;
				}
			}

		}
		if (npcScript != null && closestNPC != null) {
			npcScript.targetObject = gameObject;
			npcScript.moveToTarget = true;
			npcScript.isActive = true;
			// set fishing spot since its normally set when NPC looks for fishingSpot
			npcScript.fishingSpot = gameObject;
			callingNPC = false;
			RemoveFromBlackBoardList ();
		}
	}

}
