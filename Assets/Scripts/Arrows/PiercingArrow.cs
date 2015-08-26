using UnityEngine;
using System.Collections;

public class PiercingArrow : Arrow {

	GameObject pierceEffect;

	public override void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.ForceArrow) {
		speedModifier = 1.0f;
		rangeModifier = 1.2f;
		base.Initialize(speed, range, charge, type);
		this.gameObject.collider.isTrigger = true;
		gameObject.renderer.material = (Material)Resources.Load("Materials/PiercingArrow");

		pierceEffect = (GameObject)Instantiate (Resources.Load ("Particles/Air Trail Effect"),
		                                        this.transform.position - (Vector3.Normalize(this.gameObject.rigidbody.velocity) * 2.0f),
		                                        Quaternion.LookRotation(Vector3.Normalize (this.gameObject.rigidbody.velocity)));
		pierceEffect.transform.parent = this.gameObject.transform;
		Destroy (pierceEffect, this.range/this.speed);
	}
	
	protected void OnTriggerEnter(Collider collision) {		
		hitDetails [0] = owner.GetComponent<Character> ().ID;
		hitDetails [1] = 0;
		hitDetails [2] = 1;
		
		if (collision.gameObject.GetComponent<Character> ())
			hitDetails[1] = collision.gameObject.GetComponent<Character> ().ID;

		collision.gameObject.SendMessage ("onHit", hitDetails);
	}
}
