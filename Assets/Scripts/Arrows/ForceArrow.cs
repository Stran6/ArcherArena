using UnityEngine;
using System.Collections;

public class ForceArrow : Arrow {
	float forceTime = 1.0f;
	AudioClip forceSound;
	GameObject forceEffect;

	public override void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.ForceArrow) {
		BoxCollider collider = gameObject.GetComponent<BoxCollider>();
		collider.center = new Vector3 (-0.0002f, 0.0001f, .9f);
		collider.size = new Vector3 (1.0f, 1.0f, 28.0f);
		speedModifier = 1.2f;
		rangeModifier = 1.0f;
		base.Initialize(speed, range, charge, type);
		this.gameObject.collider.isTrigger = true;
		gameObject.renderer.material = (Material)Resources.Load("Materials/ForceArrow");
		
		this.gameObject.transform.localScale += new Vector3(0.2f, 0.0f, 0.0f);

		forceTime += charge/3.0f;

		forceEffect = (GameObject)Instantiate (Resources.Load ("Particles/Air Rush Effect"),
		                                       this.transform.position + (Vector3.Normalize(this.gameObject.rigidbody.velocity) * 2.0f), Quaternion.identity);
		ParticleSystem forceSystem = forceEffect.GetComponent<ParticleSystem> ();
		Vector3 rotationAngles = Quaternion.LookRotation (Vector3.Normalize (this.gameObject.rigidbody.velocity)).eulerAngles;
		rotationAngles.y += 90.0f;
		forceSystem.transform.Rotate(rotationAngles);
		forceSystem.transform.parent = this.gameObject.transform;
		Destroy (forceEffect, this.range/this.speed);
	}

	protected void OnTriggerEnter(Collider collision) {
		forceSound = (AudioClip)Resources.Load ("Audio/forcepush");
		AudioSource source = AudioHelper.PlayClipAt (forceSound, this.gameObject.transform.position);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 30.0f;

		hitDetails [0] = owner.GetComponent<Character> ().ID;
		hitDetails [1] = 0;
		hitDetails [2] = 1;
		
		if (collision.gameObject.GetComponent<Character> ())
			hitDetails[1] = collision.gameObject.GetComponent<Character> ().ID;

		if (collision.gameObject.GetComponent<Character> () ||
		    collision.gameObject.GetComponent<ObstacleCollision> () ||
		    collision.gameObject.GetComponent<MovableWalls>() ||
		    collision.gameObject.GetComponent<Tree>()) {
			Vector3 v = new Vector3 (0, 0, 1);
			Quaternion direction = this.gameObject.rigidbody.rotation;
			Vector3 forceVelocity = direction * v * (Vector3.Magnitude(this.gameObject.rigidbody.velocity));
			forceVelocity.y = 0;
			
			collision.gameObject.AddComponent<Accelerate> ().Initialize (forceVelocity, forceTime, 0.0f);
			collision.gameObject.AddComponent<Projectile> ().Initialize(hitDetails, forceTime);
		}
		
		Destroy(forceEffect);
		Destroy (this.gameObject);
	}
}
