using UnityEngine;
using System.Collections;

public class Seamonster : MonoBehaviour {

	public int hp = 50;
	public float speed = 1;
	public int attackingLevel;
	public GameObject attackPlatform;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.right * speed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "Harpoon") {
			hp -= 1;
		}
	}

}
