using UnityEngine;
using System.Collections;

public class Harpoon : MonoBehaviour {

	public bool throwHarpoon = false;
	public GameObject targetObject = null;

	public bool translateForward = false;
	public bool translateRight = false;
	public bool translateUp = false;

	private bool followMonster = false;
	private GameObject harpoonedMonster = null;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if (throwHarpoon && !followMonster) {
//			Vector3 offset = transform.position - targetObject.transform.position;
//			transform.Translate (offset * Time.deltaTime);
			transform.Translate(transform.forward * Time.deltaTime * 10, Space.World);
		}

//		if (followMonster) {
//			// TODO
//			// follow monster isn't crucial, but might be a nice effect
//			FollowMonster ();
//		}

//		if (translateForward){
//			transform.Translate(transform.forward * Time.deltaTime * 10);
//		}
//		if (translateRight){
//			transform.Translate(transform.right * Time.deltaTime * 10);
//		}
//		if (translateUp){
//			transform.Translate(transform.up * Time.deltaTime * 10);
//		}

	}

	void FollowMonster(){
//		transform.position = new Vector3 (harpoonedMonster.transform.position.x, transform.position.y, transform.position.z);
		transform.Translate(Vector3.right * Time.deltaTime * 0.6f, Space.World);
	}

	void OnTriggerEnter(Collider col){
		GameObject go = col.gameObject;
		Debug.Log ("Harpoon trigger enter yo'");
		if (go.layer == 11) {
			Debug.Log ("Stop the Harpoon");
			// destroy the harpoon for now??
			Destroy (gameObject);
//			harpoonedMonster = go;
//			followMonster = true;
		}
	}

}
