using UnityEngine;
using System.Collections;

public class SeamonsterAlarm : MonoBehaviour {

	// black board
	public BlackBoard blackboard;
	public bool fromLeft = false;
	public bool fromRight = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col){
//		Debug.Log ("Something entered the Seamonster Trigger");
		GameObject go = col.gameObject;
		if (col.gameObject.layer == 11) {
			blackboard.seamonster = go;
			if (fromLeft) {
				blackboard.seamonsterOnLeft = true;
			}
			if (fromRight) {
				blackboard.seamonsterOnRight = true;
			}
			blackboard.CallHunterNPCs ();
		}
	}
	void OnTriggerExit(Collider col){
//		Debug.Log ("Something exited the Seamonster Trigger");
		GameObject go = col.gameObject;
		if (col.gameObject.layer == 11) {
			blackboard.seamonster = go;
			if (fromLeft) {
				blackboard.seamonsterOnRight = false;
			}
			if (fromRight) {
				blackboard.seamonsterOnLeft = false;
			}
		}
	}

}
