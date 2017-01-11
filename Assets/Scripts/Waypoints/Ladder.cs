using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	public bool leftLadder;// which side of the platform the ladder is on
	public bool rightLadder;// which side of the platform the ladder is on
	public GameObject platform;
	public Bounds platformBounds;

	public GameObject lowerPlatform;

	// TODO
	// make sure ladder and edge colliders don't interact when they overlap... consult lthe layer grid

	// Use this for initialization
	void Start () {
		platformBounds = platform.GetComponent<BoxCollider>().bounds;
	}
}
