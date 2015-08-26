using UnityEngine;
using System.Collections;

public class TutorialSceneSwapper : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Character> ()) {
			Application.LoadLevel(1);
		}
	}
}
