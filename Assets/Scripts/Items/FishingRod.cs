using UnityEngine;
using System.Collections;

public class FishingRod : MonoBehaviour {

	public GameObject npc;
	public bool followNPC = false;

	// fish prefabs
	public Transform fish;

	// fishing spot
	private bool isFishing = false;
	public GameObject fishingSpot;
	private FishingSpot fishingSpotScript;

	// fishing action
	private bool waitingForFish = false;
	private float spawnWaitTime = 10.0f;
	private int totalFishCaught = 0;
	public int maxFish = 8;
	private IEnumerator catchFish;

	// breaking the rod
	public float rodBreakWaitTime = 2.0f;
	private IEnumerator breakRod;

	// temp fishing animation
	private float maxRotation = 5;
	private float rotationSpeed = 2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (followNPC && npc != null) {
			transform.position = new Vector3 (npc.transform.position.x, npc.transform.position.y, 0);
		}
		if (isFishing) {
			float rodRotation = maxRotation * Mathf.Sin (Time.time * rotationSpeed) - 45;
			// bob up and down if manned and working
			transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rodRotation);
			if (!waitingForFish) {
				catchFish = SpawnFish();
				StartCoroutine (catchFish);
			}
			if (totalFishCaught >= maxFish){
				breakRod = BreakRod();
				StartCoroutine (breakRod);
			}
		}
	}

	public void StartFishing(){
		if (fishingSpot != null) {
//			Debug.Log ("Start Fishing");
			isFishing = true;
			fishingSpotScript = fishingSpot.GetComponent<FishingSpot> ();
		}
	}

	public void StopFishing(){
		isFishing = false;
		fishingSpot = null;
	}

	private IEnumerator SpawnFish(){
		waitingForFish = true;
		yield return new WaitForSeconds(spawnWaitTime);
//		Debug.Log (">>>>>>>>>Spawn A FISH<<<<<<<<<");
		bool addedFish = fishingSpotScript.AddFish ();
		totalFishCaught += 1;
//		if (addedFish) {
//			Debug.Log ("Caught a fish and stored it");
//		} else {
//			Debug.Log ("Caught a fish and threw it away");
//		}

		waitingForFish = false;
	}

	private IEnumerator BreakRod(){
		waitingForFish = true;
//		Debug.Log ("Break the ROD!!!");
		yield return new WaitForSeconds(rodBreakWaitTime);
		fishingSpotScript.OpenFishingSpot ();
		// if 1 rod breaks here then close fishing spot
		// if player later removes stored fish from spot it re-opens
		fishingSpotScript.CloseFishingSpot ();
		fishingSpotScript.closedByBrokenRod = true;
		npc.GetComponent<NPC>().BreakFishingRod ();
		Destroy (gameObject);
	}

}
