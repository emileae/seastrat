using UnityEngine;
using System.Collections;

public class IslandGenerator : MonoBehaviour {

	public Transform platform1;
	public Transform platform2;
	public Transform platform3;

	// Use this for initialization
	void Start () {
		Debug.Log ("Should Generate a map on Start");
		GenerateIsland ();
	}

	public void GenerateIsland(){
		Debug.Log ("Should Generate a map function");

//		Transform generatedPlatform1 = GameObject.Instantiate(platform1, new Vector3(0, 0, 0), Quaternion.identity) as Transform;
//		Transform generatedPlatform2 = GameObject.Instantiate(platform2, new Vector3(-2.5f, 2f, 0), Quaternion.identity) as Transform;
//		Transform generatedPlatform3 = GameObject.Instantiate(platform3, new Vector3(-3.5f, 2f, 0), Quaternion.identity) as Transform;
	}
}
