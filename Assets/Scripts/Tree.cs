using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {
	private int hitCount;
	AudioClip arrowCollisionSound;
	// Use this for initialization
	void Start () {
		hitCount = 2 + (int)this.gameObject.transform.localScale.x;
	}
	
	void Update () {
		if (hitCount <= 0) {
			Destroy (this.gameObject);
		}
		//		if(isBurning) {
		//			hitCount -= 1;
		//		}
	}
	
	//	protected virtual void OnCollisionEnter(Collision collision) {
	//		if (collision.gameObject.GetComponent<FireArrow> ()) {
	//			isBurning = true;
	//		}
	//	}
	
	void OnDestroy() {
		Destroy (this.gameObject.GetComponent<Dirt>());
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Arrow> ()) {
			Dirt dirt = this.gameObject.AddComponent<Dirt> ();
			dirt.Initialize (collision.gameObject.transform.position);
			arrowCollisionSound = (AudioClip)Resources.Load ("Audio/shieldBlock");
			AudioSource.PlayClipAtPoint (arrowCollisionSound, this.gameObject.transform.position);
		}
	}
	
	void onHit(int[] hitDetail) {
		//float newColor = 1.0f;
		
		
		//this.renderer.material.color = new Color(newColor, newColor, newColor);
		hitCount--;
		arrowCollisionSound = (AudioClip)Resources.Load ("Audio/shieldBlock");
		AudioSource source = AudioHelper.PlayClipAt (arrowCollisionSound, this.gameObject.transform.position);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 30.0f;


	}
}
