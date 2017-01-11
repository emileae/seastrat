using UnityEngine;
using System.Collections;

public class TreeSpot : MonoBehaviour {

	public Waypoint waypoint;

	// occupied by NPC
	public bool occupied = false;

//	// fish prefabs
//	public Transform fish;
//
//	// fish
//	public int cost = 1;
//	private bool purchased = false;
//	public List<GameObject> storedFish = new List<GameObject>();
//	public int numFish = 0;
//	private float fishZPosition = -3.0f;

	// activation
	public NPC npc;
	public bool reachedMax = false;// used by NPC to know when to stop

	public bool fishingRodTree = true;
	public int fishingRodsProduced = 0;
	public int maxFishingRods = 1;
	// TODO
	// include a reset function for trees to regenerate
	public float fishingRodTime = 5.0f;
	private IEnumerator fishingRodCoroutine;
	public Transform rod;
	public float rodXOffset = 0.2f;
	public float rodYOffset = 1.0f;

	private IEnumerator produceRod;

	public bool huntingSpearTree = false;
	public int huntingSpearsProduced = 0;
	public bool boatBuildingTree = false;
	public int boatsProduced = 0;

	void Start(){
		// reference
		waypoint = gameObject.GetComponent<Waypoint>();
	}

	public void ActivateSpot(NPC npcScript){
		npc = npcScript;
		if (!reachedMax) {
			if (fishingRodTree) {
//				InvokeRepeating ("ProduceRod", 0f, fishingRodTime);
				produceRod = ProduceRod ();
				StartCoroutine (produceRod);
			}
		}
	}

	private IEnumerator ProduceRod(){
		yield return new WaitForSeconds(fishingRodTime);

		Transform newRod = GameObject.Instantiate(rod, new Vector3(transform.position.x - (rodXOffset * fishingRodsProduced), transform.position.y + rodYOffset, 0), Quaternion.identity) as Transform;

		Waypoint newRodWaypoint = newRod.gameObject.GetComponent<Waypoint> ();
		newRodWaypoint.island = waypoint.island;
		newRodWaypoint.platform = waypoint.platform;
		newRodWaypoint.platformLevel = waypoint.platformLevel;

		fishingRodsProduced += 1;

//		Debug.Log ("fishingRodsProduced: " + fishingRodsProduced);
//		Debug.Log ("maxFishingRods: " + maxFishingRods);

		if (fishingRodsProduced >= maxFishingRods) {
//			Debug.Log ("Tell NPC to idle??????");
			reachedMax = true;
//			Debug.Log ("Put tree to sleep for some time to regenerate");
			npc.isActive = false;
			occupied = false;
			npc.Idle ();
		}
//		fishingRodsProduced += 1;

		// setup fishing rod and call an NPC to fetch it
		newRodWaypoint.CallNPC ();

	}

//	public

}
