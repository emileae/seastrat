using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// black board
	public BlackBoard blackboard;

	public float speed = 2.0F;
	public float climbSpeed = 1.5f;
	public float jumpSpeed = 8.0F;
	public float gravity = -10.0F;
	private Vector3 moveDirection = Vector3.zero;

	CharacterController controller;

	private float velocityY = 0;

	// Sea
	public GameObject sea;
	private Bounds seaBounds;
	private bool inSea = false;
	private bool onSeaSurface = false;
	private float minSinkSpeed = 0.001f;
	public float seaFallSmoothTime = 0.3F;
	public float swimUpSpeed = 2f;
	private float dampVelocity;

	// ladder
	private bool onLadder = false;

	// input refining
	private bool pressedDown = false;

	// payments
	private int numPaid = 0;
	private bool purchased = false;

	// waypoints
	public int carryingFish = 0;
	private GameObject waypointGameObject;
	private bool onFishingSpot = false;
	private bool onTreeSpot = false;
	private bool onWallSpot = false;
	private bool onBoatSpot = false;

	// pickup
	public float pickupTime = 0.1f;
	private IEnumerator pickupCoroutine;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		seaBounds = sea.GetComponent<BoxCollider> ().bounds;
	}
	
	// Update is called once per frame
	void Update () {

		// input
		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		// move
		if (!controller.isGrounded && !inSea) {
			velocityY += gravity * Time.deltaTime;
		}

		// adjust velocityY in Sea first then the ladder so can still climb out of sea
		if (inSea) {
			FallInWater ();
		}
		if (inSea && transform.position.y >= seaBounds.max.y) {
			Swim ();
		}
		if (inSea && Mathf.Abs(velocityY) <= minSinkSpeed && !onSeaSurface) {
//			Debug.Log ("SHOULD SWIM TO SURFACE");
			SwimToSurface ();
		}

		if (onLadder && input.y > 0) {
			velocityY = climbSpeed;
		}

		// waypoints
		if(onFishingSpot && input.y < 0 && !pressedDown){
			Debug.Log ("Pick up fish");
			pickupCoroutine = CollectPay();
			StartCoroutine(pickupCoroutine);
			pressedDown = true;
		}
		if(onTreeSpot && input.y < 0 && !pressedDown){
			Debug.Log ("Dropped fish at tree");
			pickupCoroutine = CollectPay();
			StartCoroutine(pickupCoroutine);
			pressedDown = true;
		}
		if(onWallSpot && input.y < 0 && !pressedDown){
			Debug.Log ("Dropped fish at wall");
			pickupCoroutine = CollectPay();
			StartCoroutine(pickupCoroutine);
			pressedDown = true;
		}
		if (onBoatSpot && input.y < 0 && !pressedDown) {
			Debug.Log ("Dropped fish at boat");
			pickupCoroutine = CollectPay();
			StartCoroutine(pickupCoroutine);
			pressedDown = true;
		}

		moveDirection = new Vector3(input.x * speed, velocityY, 0);
		if (blackboard.windBlowing && !inSea) {
			moveDirection += new Vector3 (blackboard.windXDirection * blackboard.windSpeed, 0, 0);
		}
		controller.Move(moveDirection * Time.deltaTime * speed);
	}

	// deals with both picking up and placing fish
	private IEnumerator CollectPay(){
//		Debug.Log ("waypointGameObject == null ? " + (waypointGameObject == null));
//		if (waypointGameObject) {
//			Waypoint wp = waypointGameObject.GetComponent<Waypoint> ();
//		}

		yield return new WaitForSeconds(pickupTime);
		if (waypointGameObject) {
			if (onFishingSpot) {
				FishingSpot fishingSpotScript = waypointGameObject.GetComponent<FishingSpot> ();
				Waypoint wp = waypointGameObject.GetComponent<Waypoint> ();
				// if its occupied but still purchased then can draw fish from fishing spot
				// automatically set waypoint to purchased if there is starting fish
				if (fishingSpotScript.isOpen || wp.purchased) {
					bool removeFish = fishingSpotScript.RemoveFish ();
					if (!removeFish) {
						carryingFish += 1;
					}
				} else if (!fishingSpotScript.isOpen && !wp.purchased) {
					if (carryingFish > 0) {
						purchased = wp.AddFish ();
						numPaid += 1;
//						Debug.Log ("Paid: " + numPaid);
					}
					if (purchased) {
						carryingFish -= numPaid;
						numPaid = 0;// reset for next payment
//						Debug.Log ("Purchased fishingSpot!!! yay!");
						// reset purchased
						purchased = false;
					}

					// don't have enough money/fish
					if (numPaid >= carryingFish && !purchased) {
						for (int i = 0; i < numPaid; i++) {
							waypointGameObject.GetComponent<Waypoint> ().RemoveFish ();
						}
						;
						numPaid = 0;// reset for next payment
						Debug.Log ("Couldn't afford the purchase..... booo");
					}
				}
			}
			if (onTreeSpot) {
				PurchaseSpot ();
			}
			if (onWallSpot) {
				PurchaseSpot ();
			}
			if (onBoatSpot) {
				PurchaseSpot ();
			}
			ResetDownButton ();
//			Debug.Log ("Picked Up Object");
		}
	}

	void PurchaseSpot(){
		Waypoint wp = waypointGameObject.GetComponent<Waypoint> ();
		if (!wp.purchased) {
//			Debug.Log ("numPaid: " + numPaid);
//			Debug.Log ("carryingFish: " + carryingFish);
//			Debug.Log ("numPaid <= carryingFish: " + (numPaid <= carryingFish));
			if (carryingFish > 0) {
				purchased = wp.AddFish ();
//				Debug.Log ("Added a fish?!?!?!");
				numPaid += 1;
				// remove fish from carryingFish / resource
				carryingFish -= 1;
//				Debug.Log ("Paid: " + numPaid);
			}
			if (purchased) {
//				carryingFish -= numPaid;
				numPaid = 0;// reset for next payment
//				Debug.Log ("Purchased spot!!!");
				// reset purchased
				purchased = false;
			}

			// don't have enough money/fish
//			if (numPaid >= carryingFish && !purchased) {
//				for (int i = 0; i < numPaid; i++) {
//					waypointGameObject.GetComponent<Waypoint> ().RemoveFish ();
//					// add fish back to carryingFish / resource
//					carryingFish += 1;
//				}
//				numPaid = 0;// reset for next payment
//				Debug.Log ("Couldn't afford the purchase..... booo");
//			}
		}
	}

	void ResetDownButton(){
		pressedDown = false;
	}

	void Swim(){
		velocityY = 0;
		onSeaSurface = true;
	}

	void FallInWater(){
		velocityY = Mathf.SmoothDamp(velocityY, 0, ref dampVelocity, seaFallSmoothTime);
	}

	void SwimToSurface() {
		velocityY = swimUpSpeed;
	}

//	void OnControllerColliderHit(ControllerColliderHit col){
//		GameObject go = col.gameObject;
//		Debug.Log ("Player collided with something: " + go.tag);
//	}

	void OnTriggerEnter(Collider col){
		GameObject go = col.gameObject;
		if (go.tag == "Ladder") {
			onLadder = true;
		}

		if (go.tag == "Sea") {
			inSea = true;
//			Debug.Log ("Landed in sea");
		}

		if (go.layer == 8) {
//			Debug.Log (">>>>>>>>>>>>>>>>>> Player entered a waypoint <<<<<<<<<<<<<<<<<<");
			waypointGameObject = go;
			if (go.tag == "FishingSpot") {
				onFishingSpot = true;
			}
			if (go.tag == "TreeSpot") {
				onTreeSpot = true;
			}
			if (go.tag == "Wall"){
				onWallSpot = true;
			}
			if (go.tag == "Boat"){
				onBoatSpot = true;
			}
		}
	}

	void OnTriggerExit(Collider col){
		GameObject go = col.gameObject;
		if (go.tag == "Ladder") {
//			Debug.Log ("Exited ladder.'.'.'.'.");
			onLadder = false;
		}

		if (go.tag == "Sea") {
			inSea = false;
			onSeaSurface = false;
//			Debug.Log ("Exited in sea");
		}

		if (go.layer == 8) {
//			Debug.Log (">>>>>>>>>>>>>>>>>> Player exited a waypoint <<<<<<<<<<<<<<<<<<");
			//waypointGameObject = null;
			// if in the middle of a purchase then cancel the purchase
			if (numPaid > 0 && !purchased){
				for (int i = 0; i < numPaid; i++) {
					go.GetComponent<Waypoint>().RemoveFish ();
					carryingFish += 1;
				}
				numPaid = 0;
			}
			if (go.tag == "FishingSpot") {
//				Debug.Log ("Left a Fishing Spot");
				onFishingSpot = false;
			}
			if (go.tag == "TreeSpot") {
				onTreeSpot = false;
			}
			if (go.tag == "Boat") {
				onBoatSpot = false;
			}
		}
	}

}
