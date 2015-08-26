using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	protected ArrowType type;
	public GameObject owner;
	protected float speed, range, charge;
	protected float speedModifier = 1.0f;
	protected float rangeModifier = 1.0f;
	protected int[] hitDetails;
	protected AudioClip shootSound;
	
	public virtual void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.Arrow) {
		this.speed = speedModifier * speed;
		this.range = rangeModifier * range;
		this.charge = charge;
		this.type = type;
		hitDetails = new int[3];
		shootSound = (AudioClip)Resources.Load ("Audio/swordSwing");
		AudioSource source = AudioHelper.PlayClipAt (shootSound, this.gameObject.transform.position);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 50.0f;
		Destroy (this.gameObject, this.range/this.speed);
	}

	protected virtual void OnDestroy() {
		//print ("From: " + startingPos + " To: " + this.transform.position);
		//print ("Distance: " + Vector3.Distance(startingPos, this.transform.position));
	}

	protected virtual void Update() {
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		hitDetails [0] = owner.GetComponent<Character> ().ID;
		hitDetails [1] = 0;
		hitDetails [2] = 1;

		if (collision.gameObject.GetComponent<Character> ())
			hitDetails[1] = collision.gameObject.GetComponent<Character> ().ID;

		collision.gameObject.SendMessage ("onHit", hitDetails);
		Destroy (this.gameObject);
	}
	
	public virtual void onHit() {
		
	}

	public ArrowType getArrowType() {
		return type;
	}
}