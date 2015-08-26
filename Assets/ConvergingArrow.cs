using UnityEngine;
using System.Collections;

public class ConvergingArrow : Arrow {
	
	bool converging = false;
	int numConvergingArrows;
	float convergeAngle, convergeIncrement;
	Vector3 initialDirection;
	
	public override void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.SplitArrow) {
		this.speed = speed;
		this.range = range;
		this.charge = charge;
		this.type = type;

		speedModifier = 1.0f;
		rangeModifier = 1.0f;
		
		if (converging) {
			shootSound = (AudioClip)Resources.Load ("Audio/shooting");
		} else {

			initialDirection = this.gameObject.transform.forward.normalized;
			
			numConvergingArrows = 8 + Mathf.RoundToInt(charge * 4);
			convergeAngle = 0;
			convergeIncrement = 360.0f / numConvergingArrows;

			shootSound = (AudioClip)Resources.Load ("Audio/swordSwing2");
		}

		AudioSource source = AudioHelper.PlayClipAt (shootSound, this.gameObject.transform.position);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 30.0f;
		
		hitDetails = new int[3];
		gameObject.renderer.material = (Material)Resources.Load("Materials/SplitArrow");
		Destroy (this.gameObject, range/speed);
	}
	
	protected override void OnDestroy() {
		if (!converging) {
			for(int i = 0; i < numConvergingArrows; i++){
				spawnConvergingArrows();
				convergeAngle += convergeIncrement;
			}
		}
	}
	
	private void spawnConvergingArrows() {
		GameObject convergeArrow = (GameObject)Instantiate (Resources.Load ("ArrowPrefab"), this.gameObject.transform.position + initialDirection * (5.0f + (charge* 2)), Quaternion.LookRotation(initialDirection));
		convergeArrow.transform.RotateAround(this.gameObject.transform.position, Vector3.up, convergeAngle);
		convergeArrow.transform.Rotate (new Vector3 (0.0f, 180.0f, 0.0f));

		float chargeEffect = (charge/2) + 1;
		convergeArrow.rigidbody.velocity = Quaternion.LookRotation(convergeArrow.transform.forward) * new Vector3(0.0f, 0.0f, speed/(8.0f * chargeEffect));

		convergeArrow.renderer.material = (Material)Resources.Load("Materials/SplitArrow");
		ConvergingArrow convergeComponent = convergeArrow.AddComponent<ConvergingArrow> ();
		convergeComponent.owner = this.owner;
		convergeComponent.converging = true;
		convergeComponent.Initialize(speed/(8.0f * chargeEffect), (range/1.5f) * chargeEffect, charge, type);
	}
}