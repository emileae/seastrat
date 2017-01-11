using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour {

	public bool left;
	public bool right;
	public GameObject platform;
	public Bounds platformBounds;

	public bool ladderEdge = false;// set in generator
	public GameObject ladder;// set in generator

	// Use this for initialization
	void Start () {
		platformBounds = platform.GetComponent<BoxCollider> ().bounds;
	}
}
