using UnityEngine;
using System.Collections;

public class FireEffect : MonoBehaviour {

	AudioClip fireSound;

	public GameObject owner;
	public void Initialize(GameObject o, float charge) {
		owner = o;
		this.gameObject.renderer.enabled = false;
		this.gameObject.AddComponent<AudioSource> ();
		fireSound = (AudioClip)Resources.Load ("Audio/explode3");
		AudioSource source = AudioHelper.PlayClipAt (fireSound, this.gameObject.transform.position, 1 + charge);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 40.0f;
		Destroy (this.gameObject, 2 + (charge / 3));
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerExit (Collider collision) {
		int[] hitDetails = new int[3];
		hitDetails [0] = owner.GetComponent<Character> ().ID;
		hitDetails [1] = 0;
		hitDetails [2] = 1;
		
		if (collision.gameObject.GetComponent<ObstacleCollision>())
			collision.gameObject.SendMessage ("onHit", hitDetails);
		
		if (collision.gameObject.GetComponent<Character> ()) {
			hitDetails[1] = collision.gameObject.GetComponent<Character> ().ID;
			
			collision.gameObject.SendMessage ("onHit", hitDetails);
		}
	}

	void OnTriggerStay (Collider collision) {
		int[] hitDetails = new int[3];
		hitDetails [0] = owner.GetComponent<Character> ().ID;
		hitDetails [1] = 0;
		hitDetails [2] = 1;
		
		if (collision.gameObject.GetComponent<ObstacleCollision>()) {
			collision.gameObject.SendMessage ("onHit", hitDetails);
		}
		
		if (collision.gameObject.GetComponent<Character> ()) {
			hitDetails[1] = collision.gameObject.GetComponent<Character> ().ID;
			
			collision.gameObject.SendMessage ("onHit", hitDetails);
		}
	}

	void OnTriggerEnter (Collider collision) {
		int[] hitDetails = new int[3];
		hitDetails [0] = owner.GetComponent<Character> ().ID;
		hitDetails [1] = 0;
		hitDetails [2] = 1;

		if (collision.gameObject.GetComponent<ObstacleCollision>())
			collision.gameObject.SendMessage ("onHit", hitDetails);

		if (collision.gameObject.GetComponent<Character> ()) {
			hitDetails[1] = collision.gameObject.GetComponent<Character> ().ID;

			collision.gameObject.SendMessage ("onHit", hitDetails);
		}
	}
}