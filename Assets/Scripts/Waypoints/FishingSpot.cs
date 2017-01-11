using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingSpot : MonoBehaviour {

	// blackboard
	public BlackBoard blackboard;

	public Waypoint waypoint;
	private Island islandScript;

	// payments
	public int costToOpen = 1;
	public int currentFishPaid = 0;

	// npc control
	public bool isOpen = true;
//	public bool openingFS = false;

	// fish
	public bool closedByBrokenRod = false;
	public int startingFish = 0;
	public int numFish = 0;
	private int totalFish = 0;
	private float fishZPosition = -0.7f;
	private List<GameObject> storedFish = new List<GameObject>();
	public int maxFishStoreable = 4;

	// fish prefabs
	public Transform fish;

	// Use this for initialization
	void Start () {
		// references
		waypoint = gameObject.GetComponent<Waypoint>();
		islandScript = waypoint.island.GetComponent<Island> ();

		if (startingFish > 0) {
			for (var i = 0; i < startingFish; i++) {
				AddFish ();
			}
			waypoint.purchased = true;
		}

		if (!isOpen){
			waypoint.RemoveFromBlackBoardList ();
		}

		if (blackboard == null) {
			blackboard = GameObject.Find ("BlackBoard").GetComponent<BlackBoard> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
//		if (openingFS) {
//			OpenFishingSpot ();
//		}
		if (closedByBrokenRod && numFish < maxFishStoreable) {
			closedByBrokenRod = false;
			// if a broken rod closed the fishing spot then player later comes to remove excess fish, then re-open
			OpenFishingSpot ();
		}
	}

	public void OpenFishingSpot(){
//		openingFS = false;
		isOpen = true;
		waypoint.AddToBlackBoardList ();
	}
	public void CloseFishingSpot(){
		isOpen = false;
		waypoint.RemoveFromBlackBoardList ();
	}

	public bool AddFish(){

		// TODO
		// when modelling is done, set the fish height in advance to save on a GETCOMPONENT call for fish bounds
		if (numFish < maxFishStoreable) {
			float fishPositionX = transform.position.x;
			float fishPositionY = transform.position.y - (numFish * blackboard.fishSizeY);//(numFish * fish.GetComponent<Renderer> ().bounds.size.y);
			Debug.Log ("FISH RENDERER BOUNDS: . . . . :::" + blackboard.fishSizeY);
			float fishPositionZ = fishZPosition;
			Transform newFish = GameObject.Instantiate (fish, new Vector3 (fishPositionX, fishPositionY, fishPositionZ), Quaternion.identity) as Transform;
			storedFish.Add (newFish.gameObject);
			numFish += 1;
			totalFish += 1;
			return true;
		} else {
			return false;
		}
	}

	public bool RemoveFish(){
		if (storedFish.Count > 0) {
			int idx = storedFish.Count - 1;
			Destroy (storedFish [idx]);
			storedFish.RemoveAt (idx);
			islandScript.fishStored -= 1;
			numFish -= 1;
			return false;// this return false when NPC gets food and true when they need to die
		}else if (storedFish.Count <= 0){
			// TODO
			// kill npc if there is no fish
//			Debug.Log ("Should kill off NPC");
			return true;
		}

		return false;
	}

}
