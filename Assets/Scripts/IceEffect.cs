using UnityEngine;
using System.Collections;

public class IceEffect : MonoBehaviour {

	public GameObject owner;
	private float charge;
	AudioClip iceSound;

	public void Initialize(GameObject o, float charge) {
		owner = o;
		this.gameObject.renderer.enabled = false;
		this.charge = charge;
		this.gameObject.AddComponent<AudioSource> ();
		iceSound = (AudioClip)Resources.Load ("Audio/freeze");
		AudioSource source = AudioHelper.PlayClipAt (iceSound, this.gameObject.transform.position, 3 + charge);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 40.0f;
		source.loop = true;

		Destroy (this.gameObject, 3 + charge);
	}
	
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
	}

	void OnTriggerExit (Collider collision) {
		if(collision.gameObject.GetComponent<Character>())
			collision.gameObject.SendMessage ("iceEffect", 3 + charge);
	}

	void OnTriggerStay (Collider collision) {
		if(collision.gameObject.GetComponent<Character>())
			collision.gameObject.SendMessage ("iceEffect", 3 + charge);
	}

	void OnTriggerEnter (Collider collision) {
		if(collision.gameObject.GetComponent<Character>())
			collision.gameObject.SendMessage ("iceEffect", 3 + charge);
	}
}
