using UnityEngine;
using System.Collections;

public class MovableWalls: MonoBehaviour {
	AudioClip arrowCollisionSound;
	int[] hitDetails;
	
	void onHit(int[] hitDetail) {
		hitDetails = hitDetail;
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
