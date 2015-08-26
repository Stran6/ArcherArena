using UnityEngine;
using System.Collections;

public class Walls : MonoBehaviour {

	AudioClip arrowCollisionSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void onHit(int[] hitDetail) {

	}

	protected virtual void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Arrow> ()) {
			Dirt dirt = this.gameObject.AddComponent<Dirt> ();
			dirt.Initialize (collision.gameObject.transform.position);
			arrowCollisionSound = (AudioClip)Resources.Load ("Audio/shieldBlock");
			AudioSource.PlayClipAtPoint (arrowCollisionSound, this.gameObject.transform.position);
		}
	}
}
