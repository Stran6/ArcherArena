using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	int[] hitDetails;

	public void Initialize(int[] hitDetails, float duration) {
		this.hitDetails = hitDetails;
		Destroy (this, duration);
	}

	void Update(){
		if (this.gameObject.rigidbody.velocity.magnitude <= 2.0f) {
			Accelerate acc = this.gameObject.GetComponent<Accelerate> ();

			if (acc)
				Destroy (acc);

			Destroy (this);
		}
	}

	void OnDestroy() {
		if(this.gameObject.GetComponent<Character>() != null) {
			this.gameObject.GetComponent<Character>().unlockMovement();
			this.gameObject.SendMessage ("onHit", hitDetails);
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name != "Floor" &! collision.gameObject.GetComponent<Arrow>()) {
			if (collision.gameObject.GetComponent<Character> ()) {
				collision.gameObject.SendMessage ("onHit", hitDetails);
			}

			if (collision.gameObject.GetComponent<ObstacleCollision> () ||
			    collision.gameObject.GetComponent<Tree>()) {

				if(this.gameObject.GetComponent<Character>()) {
					Destroy(this);
				} else {
					Vector3 forceVelocity;

					if(this.gameObject.rigidbody.mass <= collision.gameObject.rigidbody.mass)
						forceVelocity = this.gameObject.rigidbody.velocity * this.gameObject.rigidbody.mass / collision.gameObject.rigidbody.mass;
					else
						forceVelocity = this.gameObject.rigidbody.velocity * this.gameObject.rigidbody.mass;

					forceVelocity.y = 0;

					collision.gameObject.AddComponent<Accelerate> ().Initialize (forceVelocity, 1.0f, 0.0f);
					collision.gameObject.AddComponent<Projectile> ().Initialize(hitDetails, 99.0f);
				}
			} else if (collision.gameObject.GetComponent<Walls>() && this.gameObject.GetComponent<Character>()) {
					Destroy(this);
			}
		}
	}
}
