using UnityEngine;
using System.Collections;

public class Accelerate : MonoBehaviour {
	float cooldown;
	bool isAbility;

	public void Initialize(Vector3 velocity, float duration, float cooldown, bool isAbility = false) {
		if(this.gameObject.GetComponent<Rigidbody>() != null) {
			if(this.gameObject.GetComponent<Character>() != null && !isAbility) {
				this.gameObject.GetComponent<Character>().lockMovement();
			}

			this.gameObject.rigidbody.isKinematic = false;
			this.gameObject.rigidbody.velocity = velocity;
		}

		this.cooldown = cooldown;
		this.isAbility = isAbility;
		Destroy (this, duration);
	}

	void OnDestroy() {
		if(this.gameObject.GetComponent<Rigidbody>() != null) {
			if(this.gameObject.GetComponent<Character>() == null) {
				this.gameObject.rigidbody.isKinematic = false;
			}
			this.gameObject.rigidbody.velocity = new Vector3();
		}

		if(isAbility)
			this.gameObject.GetComponent<Character> ().abilityComplete (cooldown);
		else
			if(this.gameObject.GetComponent<Rigidbody>() != null && this.gameObject.GetComponent<Character>() == null)
				this.gameObject.rigidbody.isKinematic = true;
	}
}
