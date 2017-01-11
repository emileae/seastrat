using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackBoard : MonoBehaviour {

	// lists of items
	public List<GameObject> activatedFishingSpots = new List<GameObject>();
	public List<GameObject> activatedTrees = new List<GameObject>();
	public List<GameObject> availableFishingRods = new List<GameObject>();
	public List<GameObject> activatedWalls = new List<GameObject>();

	// NPC
	public List<GameObject> availableNPCs = new List<GameObject>();
	public List<GameObject> hunterNPCs = new List<GameObject>();

	// Seamonster approach
	public bool seamonsterOnLeft = false;
	public bool seamonsterOnRight = false;
	public GameObject seamonster = null;

	// weather
	public bool windBlowing = false;
	public int windXDirection = 1;
	public float windSpeed = 0.1f;

	// item dimensions
	public float fishSizeY = 0.6f;

	public void AddGameObjectToList(List<GameObject> ListName, GameObject go){
		if (!ListName.Contains (go)) {
			ListName.Add (go);
		}
	}
	public void RemoveGameObjectToList(List<GameObject> ListName, GameObject go){
		ListName.Remove (go);
	}

	public GameObject[] GetListAsArray(List<GameObject> ListName){
		return ListName.ToArray ();
	}

	public bool ListContainsGameObject(List<GameObject> ListName, GameObject go){
		if (ListName.Contains (go)) {
			return true;
		} else {
			return false;
		}
	}

	public void CallHunterNPCs(){
		Debug.Log ("Calling Hunters to Fight");
		GameObject[] hunters = hunterNPCs.ToArray ();
		for (int i = 0; i < hunters.Length; i++) {
			NPC npc = hunters [i].GetComponent<NPC> ();
			npc.Hunt ();
		}
	}

	public void HunterIdle(){
		Debug.Log ("Stop Hunters from idling");
		GameObject[] hunters = hunterNPCs.ToArray ();
		for (int i = 0; i < hunters.Length; i++) {
			NPC npc = hunters [i].GetComponent<NPC> ();
			npc.HunterIdle ();
		}
	}


	
	// Update is called once per frame
	void Update () {
	
	}
}
