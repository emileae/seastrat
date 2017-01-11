using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	// black board
	public BlackBoard blackboard;

	// npc
	private BoxCollider collider;
	private float yPos;

	// platform
	public GameObject platform;
	public Bounds platformBounds;
	private Platform platformScript;
	private Edge edgeScript;

	// moving
	private Vector3 npcDirection;
	public float gravity = -2.0f;// lower gravity to fall slower
	public Vector3 startDirection;
	private Vector3 direction = Vector2.zero;
	private float speed;
	public float idleSpeed = 1.0f;
	public float moveToTargetSpeed = 2.0f;
	public float climbSpeed = 1.0f;
	private float velocityY;

	// ground
	public bool onGround = false;// this can be private

	// wall
	private Wall wall;
	private bool onWall = false;

	// ladder
	public bool ascendLadder;
	public bool descendLadder;
	public bool onLadder = false;
	private float ladderMaxXPos;
	private float ladderMinXPos;
	public bool inTopCollider = false;

	// activities
	public bool idling = true;

	  // hunter
	public bool isHunter = false;
	public bool hunting = false;
	private bool hunterStop = false;// stops hunter when reaching the edge, instead of changing direction or moving off platform
	public Transform harpoon = null;// harpoon prefab, set in editor
	private GameObject activeHarpoon;// the harpoon that is instantiated and thrown
	public float harpoonReloadTime = 3.0f;
	public bool throwingHarpoon = false;
	private IEnumerator throwHarpoon;

	  // fisherman
	public bool hasFishingRod = false;
	public GameObject fishingRod = null;// can be private
	public GameObject fishingSpot = null;// can be private

	  // targeting a waypoint
	public bool isActive = false;
	public bool moveToTarget = false;
	public GameObject targetObject = null;
	private GameObject activeObject = null;
	public int targetPlatform;

	void Start () {
		InitNPC ();
	}

	void Update () {

		if(idling && hasFishingRod && !isActive){
//			Debug.Log ("Check for empty fishing spots");
			CheckForAvailableFishingSpots ();
		}

		if (moveToTarget) {
			MoveToTarget ();
		}

		if (onLadder) {
			ClimbLadder();
		}

		if (isHunter && activeHarpoon) {
			HarpoonFollow ();
		}
		if (hunting && platformScript.level == targetPlatform) {
			HuntTrack ();
		}

		if (blackboard.windBlowing && !onWall) {
			direction += new Vector3 (blackboard.windXDirection, 0, 0);
			speed += blackboard.windSpeed;
			if (!onGround && !onLadder) {
				// fall down when not on platform
				direction += Vector3.up * gravity;
			}
		}

		transform.Translate (direction * Time.deltaTime * speed);
	}

	public void InitNPC(){
		
		collider = gameObject.GetComponent<BoxCollider> ();

		startDirection = new Vector3 (-1, 0, 0);
		SetPlatformBounds ();

		if (isHunter) {
			isActive = true;
			blackboard.AddGameObjectToList (blackboard.hunterNPCs, gameObject);
			CreateHarpoon ();
		}

		NPCYPos();
		transform.position = new Vector3 (transform.position.x, yPos, 0);
		speed = idleSpeed;
		direction = startDirection;

		if (!isActive) {
			Idle ();
		}
	}

	void NPCYPos(){
		// TODO make sure pivot is correctly set for NPC
		// here the pivot is at some weird trial and error position hence 1.5f
		yPos = platformBounds.max.y + (collider.size.y*0.5f);
	}

	public void Idle(){
		direction = startDirection;
		speed = idleSpeed;
		isActive = false;
	}

	void SetPlatformBounds(){
		Debug.Log ("Set Platform bounds.....");
		if (platform) {
			platformBounds = platform.GetComponent<BoxCollider> ().bounds;
			platformScript = platform.GetComponent<Platform> ();
		}
	}

	// ============================
	// Handling movement change direction and ladders
	// ============================

	void SetDirection(){
		if (targetObject != null) {
			// if on target platform then go towards target object
			if (platformScript.level == targetPlatform) {
				if (targetObject.transform.position.x > transform.position.x) {
					if (direction != Vector3.right) {
						direction = Vector3.right;
					}
				} else if (targetObject.transform.position.x < transform.position.x) {
					if (direction != Vector3.left) {
						direction = Vector3.left;
					}
				}
			} 
			// Not on target platform but still have a targetOBject
			else {
				float ladderXPos = platformScript.ladderObject.transform.position.x;
				if (ladderXPos > transform.position.x) {
					direction = Vector3.right;
				}else if (ladderXPos < transform.position.x){
					direction = Vector3.left;
				}
			}
		} else {
			direction = Vector3.right;
		}
	}

	void ClimbLadder(){
//		Debug.Log ("CLIMB THE LADDER");
		if (inTopCollider && !descendLadder) {
			// exiting ladder
			float currentYPos = transform.position.y - (collider.bounds.size.y * 0.5f);

			// if bottom of NPC is level with top of platform, then trigger horizontal movement
			if (currentYPos >= edgeScript.platformBounds.max.y) {
				ChangeDirection ();
				// every time an edge is triggered the ladder must be false
				onLadder = false;
				//				Debug.Log ("SET ASCENDLADDER to FALSE?!?!?!?!?!");
				ascendLadder = false;
				platform = edgeScript.platform;
				SetPlatformBounds ();
			}
		}else {
//			Debug.Log ("Enter Ladder and CLIMB!!!");
			// entering ladder
			if (transform.position.x >= ladderMinXPos && transform.position.x < ladderMaxXPos) {
//				Debug.Log ("Entering a Ladder To either ascend or descend");
				if (ascendLadder) {
//					Debug.Log ("Should go up now!!!!!");
					direction = Vector3.up;
				}else if (descendLadder){
					direction = Vector3.down;
				}
				speed = climbSpeed;
			}
		}

	}

	void ChangeDirection(){
		// if on the correct platform and hit the edge stop hunters
		if (hunting && targetPlatform == platformScript.level) {
			hunterStop = true;
		}

		// move NPCs back adn fourth as they hit the edge
		if (edgeScript) {
			if (edgeScript.left) {
				direction = Vector3.right;
			} else if (edgeScript.right) {
				direction = Vector3.left;
			}
		}
		speed = idleSpeed;
	}


	// ============================
	// Handling targeting
	// ============================


	void MoveToTarget(){

		if (speed <= 0) {
			speed = idleSpeed;
		}

		if (targetObject) {

			if (hunting) {
				// TODO
				// remove the very specific Seamonster script from this... make it more generic
				// shouldn't have to reference the seamonster script, what if there's a Landmonster or an Airmonster
				targetPlatform = targetObject.GetComponent<Seamonster>().attackingLevel;
			} else {
				targetPlatform = targetObject.GetComponent<Waypoint> ().platformLevel;
			}

			if (platformScript.level < targetPlatform) {
				ascendLadder = true;
				descendLadder = false;
			} else if (platformScript.level > targetPlatform) {
				ascendLadder = false;
				descendLadder = true;
			} else if (platformScript.level == targetPlatform) {
				ascendLadder = false;
				descendLadder = false;
				moveToTarget = false;// for now just get to target platform
			}
		}

	}

	void ReachTarget(){
		speed = 0f;
		HandleTarget ();
	}


	// ============================
	// Handling waypoint Functions
	// ============================
	public void Hunt(){
		hunting = true;
		targetObject = blackboard.seamonster;
		moveToTarget = true;
	}

	void CreateHarpoon(){
		activeHarpoon = GameObject.Instantiate (harpoon, new Vector3 (transform.position.x, transform.position.y + 1.0f, 0f), Quaternion.identity) as GameObject;
		Debug.Log ("Made a new harpoon!");
		// tell harpoon to follow NPC
		HarpoonFollow ();
		// set initial angle of harpoon
//		activeHarpoon.transform.eulerAngles = new Vector3(
//			activeHarpoon.transform.eulerAngles.x + 90,
//			activeHarpoon.transform.eulerAngles.y,
//			activeHarpoon.transform.eulerAngles.z
//		);
	}

	void HarpoonFollow(){
		if (!throwingHarpoon) {
			activeHarpoon.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, 0f);
		}
	}

	public void HunterIdle(){
		targetObject = null;
		moveToTarget = false;
		hunting = false;
	}

	void HuntTrack(){
		if (edgeScript) {
			Debug.Log ("edgeScript.left: " + edgeScript.left);
			// if NPC prematurely goes to an edge and the seamonster swims past
			if (edgeScript.left && targetObject.transform.position.x > transform.position.x) {
				hunterStop = false;
			}
			if (edgeScript.right && targetObject.transform.position.x < transform.position.x) {
				hunterStop = false;
			}
		}
		if (!hunterStop) {
			if (targetObject.transform.position.x > transform.position.x) {
				direction = Vector3.right;
			} else if (targetObject.transform.position.x < transform.position.x) {
				direction = Vector3.left;
			} else {
				direction = Vector3.zero;
			}
		} else {
			direction = Vector3.zero;// stop hunter when reaching teh edge
		}

		//Aiming

		Vector3 offset = targetObject.transform.position - transform.position;
		float sqrLen = offset.sqrMagnitude;
		// if monster is in range, then aim the harpoon and throw it
		if (sqrLen <= 60) {
			Debug.DrawLine (transform.position, targetObject.transform.position, Color.red);

			Debug.Log ("Throwing the harpoon????? " + throwingHarpoon);

			if (!activeHarpoon && !throwingHarpoon) {
				Debug.Log ("Created Another Harpoon");
				CreateHarpoon ();
//				HarpoonFollow ();
			}

			if (activeHarpoon && !throwingHarpoon) {
//				activeHarpoon.transform.rotation = Quaternion.LookRotation(-offset);
				activeHarpoon.transform.LookAt(targetObject.transform.position);
				throwHarpoon = ThrowHarpoon ();
				StartCoroutine (throwHarpoon);
			}
		}
	}

	private IEnumerator ThrowHarpoon(){
		throwingHarpoon = true;
		Debug.Log ("THROW HARPOON!!!!!!!");

		Vector3 offset = targetObject.transform.position - transform.position;
		Harpoon harpoonScript = activeHarpoon.GetComponent<Harpoon> ();
		harpoonScript.targetObject = targetObject;
		harpoonScript.throwHarpoon = true;
		yield return new WaitForSeconds(harpoonReloadTime);
		Debug.Log ("Lets reset the activeHarpoon");
		activeHarpoon = null;
		throwingHarpoon = false;
	}

	// ============================
	// Handling waypoint Functions
	// ============================

	void HandleTarget(){
//		Debug.Log ("Do something with the target");
		if (activeObject != null) {
			if (activeObject.tag == "TreeSpot") {
//				Debug.Log ("Make Fishing rods");
				ActivateTree ();
				// remove from blackboard list since its no longer available
				activeObject.GetComponent<Waypoint> ().RemoveFromBlackBoardList ();
			}
			else if (activeObject.tag == "FishingRod"){
//				Debug.Log ("Got Fishing Rod");
				GotFishingRod ();
			}
		}
	}

	void ActivateTree(){
		TreeSpot treeSpotScript = activeObject.GetComponent<TreeSpot> ();
		treeSpotScript.occupied = true;
		treeSpotScript.ActivateSpot (gameObject.GetComponent<NPC>());
		// remove from blackboard list since its no longer available
		activeObject.GetComponent<Waypoint> ().RemoveFromBlackBoardList ();
	}

	void GotFishingRod(){
		idling = true;
		hasFishingRod = true;
		// remove from blackboard list since its no longer available
		activeObject.GetComponent<Waypoint> ().RemoveFromBlackBoardList ();
		AttachFishingRodToNPC ();
		GoToFishingSpot ();
		fishingRod = activeObject;
	}

	void AttachFishingRodToNPC(){
		FishingRod activeObjectScript = activeObject.GetComponent<FishingRod>();
		activeObjectScript.followNPC = true;
		activeObjectScript.npc = gameObject;
	}

	void GoToFishingSpot(){

		GameObject[] FishingSpots = blackboard.GetListAsArray (blackboard.activatedFishingSpots);

		GameObject closestFS = null;
		float sqrLen = Mathf.Infinity;
		FishingSpot fsScript = null;
		for (int i = 0; i < FishingSpots.Length; i++) {
			FishingSpot potentialFSScript = FishingSpots [i].GetComponent<FishingSpot> ();
			Vector3 offset = FishingSpots [i].transform.position - transform.position;
			float newSqrLen = offset.sqrMagnitude;
			if (newSqrLen < sqrLen) {
				closestFS = FishingSpots [i];
				fsScript = potentialFSScript;
			}
		}
		if (fsScript != null && closestFS != null && FishingSpots.Length > 0) {

			targetObject = closestFS;// target object is the fishing spot
			moveToTarget = true;
			isActive = true;
			fsScript.isOpen = false;
//			Debug.Log ("Go To Fishing Spot");
			// TODO
			// might only want to remove this form the list once the NPC has arrived at the fishing spot
			// remove from blackboard list since its no longer available
			closestFS.GetComponent<Waypoint> ().RemoveFromBlackBoardList ();
			fishingSpot = closestFS;
		} else {
//			Debug.Log ("No available Fishing Spots... try again");
			Idle ();
		}
	}

	void CheckForAvailableFishingSpots(){
		GameObject[] fishingSpots = blackboard.GetListAsArray (blackboard.activatedFishingSpots);
		if (fishingSpots.Length > 0) {
			GoToFishingSpot ();
		}
	}

	public void BreakFishingRod(){
//		Debug.Log ("Fishing rod broken, so idle");
		hasFishingRod = false;
		fishingRod = null;
		fishingSpot = null;
		Idle ();
	}

	// ============================
	// Handling Triggers
	// ============================

	void OnTriggerEnter(Collider col){
		GameObject go = col.gameObject;

		// handle reaching the target object
		if (targetObject != null){
			if (targetObject == go) {
				activeObject = targetObject;
				targetObject = null;
				moveToTarget = false;
				ReachTarget ();
			}
		}

		// layer 8 == waypoint layer
		if (go.layer == 8){
			if (go.tag == "Wall") {
				// true if this wall is in activated list
				onWall = blackboard.ListContainsGameObject(blackboard.activatedWalls, go);
			}
		}

		// layer 10 == Edge layer
		if (go.layer == 10) {
			edgeScript = go.GetComponent<Edge> ();
			// standard edge of platform, no ladders involved
			// onLadder is only set if clibing up a ladder

			// dont change direction if commanded to descend a ladder and edge is a ladder edge

			// not on a ladder and not hitting an edge that caps a ladder
			if (!onLadder && !edgeScript.ladderEdge) {
				ChangeDirection ();
			}
			// not on a ladder, but hitting an edge that caps a ladder
			// if not descending this ladder then change direction
			else if (!onLadder && edgeScript.ladderEdge) {
				if (!descendLadder) {
					ChangeDirection ();
				}
			}
			// on a ladder, also hitting an edge that caps a ladder
			// mark as inTopCollider and then follow up with exiting the ladder
			else if (onLadder && edgeScript.ladderEdge) {
				inTopCollider = true;
			} else {
				ChangeDirection ();
			}
				
				
		}

		// ASCEND LADDER
		// entering ladder
		if (go.tag == "Ladder" && ascendLadder) {
			onGround = false;
			onLadder = true;
			Ladder ladderScript = go.GetComponent<Ladder> ();
			if (ladderScript.rightLadder && direction.x == -1) {
//				Debug.Log ("Entering a ladder from right to ascend-----------");
				// add a little more than half since pivot is in centre
				ladderMaxXPos = ladderScript.platformBounds.max.x + (0.6f * collider.bounds.size.x);
				ladderMinXPos = ladderScript.platformBounds.max.x;
			} else if (ladderScript.rightLadder && direction.x == 1) {
//				Debug.Log ("Entering a ladder from left to ascend-------------");
				ladderMaxXPos = ladderScript.platformBounds.max.x + (1.1f * collider.bounds.size.x);
				ladderMinXPos = ladderScript.platformBounds.max.x  + (0.6f * collider.bounds.size.x);
			}

//			Debug.Log ("ladderMaxXPos..." + ladderMaxXPos);
//			Debug.Log ("ladderMinXPos..." + ladderMinXPos);

		}

		// DESCEND LADDER
		// entering ladder
		if (go.layer == 10 && descendLadder) {
			Edge edgeScript = go.GetComponent<Edge> ();
			if (edgeScript.ladderEdge){
				onLadder = true;
				if (edgeScript.right && direction.x == -1) {
					// add a little more than half since pivot is in centre
					ladderMaxXPos = edgeScript.platformBounds.max.x + (0.6f * collider.bounds.size.x);
					ladderMinXPos = edgeScript.platformBounds.max.x;
				} else if (edgeScript.right && direction.x == 1) {
					ladderMaxXPos = platformBounds.max.x + (1.1f * collider.bounds.size.x);
					ladderMinXPos = platformBounds.max.x  + (0.6f * collider.bounds.size.x);

//					Debug.Log ("ladderMaxXPos" + ladderMaxXPos);
//					Debug.Log ("ladderMinXPos" + ladderMinXPos);
				}
			}
		}

		// DESCEND LADDER
		// exiting ladder
		// if enter ground layer while descending
		if(go.layer == 9 && descendLadder){
			onGround = true;
			onLadder = false;
//			direction = Vector3.right;
//			speed = idleSpeed;
			descendLadder = false;
			// set platform to the current "ground" go
			platform = go;
			// recalculate new platform bounds
			SetPlatformBounds();

			// set direction after setting platform, so that the appropriate ladder can be found
			SetDirection ();

		}

		// if entering fishing spot with rod then set fishing rod to active
		if (go.tag == "FishingSpot" && hasFishingRod && fishingRod != null){
			FishingRod fishingRodScript = fishingRod.GetComponent<FishingRod> ();
			fishingRodScript.fishingSpot = fishingSpot;
			fishingRodScript.StartFishing ();
		}

		// kill NPCs that land in the Sea
		if(go.tag == "Sea"){
			blackboard.RemoveGameObjectToList (blackboard.availableNPCs, gameObject);
			if (fishingRod != null) {
				Destroy (fishingRod);
			}
			Destroy (gameObject);
		}

	}

	void OnTriggerExit(Collider col){
		GameObject go = col.gameObject;

		// be sure to size the width of the collider correctly
		// fire ladder climb on exit to make sure the NPC is roughly in place
		if (go.tag == "Ladder") {
			onLadder = false;
		}

		// layer 8 == waypoint layer
		if (go.layer == 8){
			if (go.tag == "Wall") {
				// true if this wall is in activated list
				onWall = false;
			}
		}

		// ASCEND LADDER - EXIT
		// if exiting edge collider and ladder
		if (go.layer == 10) {
			// exiting an edge collider so set inTopCollider to false regardless
			inTopCollider = false;
			if (onLadder) {
				// reset platform to be the same as the edge's platform
				platform = edgeScript.platform;
				// recalculate new platform bounds
				SetPlatformBounds();
				// reset edgeScript so that NPC can ascend another ladder, otherwise it will think its reached the target yPos
				edgeScript = null;
				SetDirection ();
			}
		}

		// if exiting the ground layer then no longer on ground
		if (go.layer == 9) {
			onGround = false;
		}

		// if exiting fishing spot with rod then set fishing spot to null
		if (go.tag == "FishingSpot" && hasFishingRod && fishingRod != null){
//			Debug.Log ("Left fishing spot.... get back to work!!!!!");
			fishingRod.GetComponent<FishingRod> ().StopFishing();
			// wind has probably blown NPC away, so it should now be trying to get back to fishingSpot
			targetObject = go;
			speed = idleSpeed;
		}

	}
		
}
