using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public int wallLevel = 0;
	private float growHeight = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GrowWall(){
		transform.position = new Vector3 (transform.position.x, transform.position.y + growHeight, 0);
	}

}
