using UnityEngine;
using System.Collections;

public class SplitArrow : Arrow {
	
	bool split = false;
	float splitAngle;
	float splitIncrement;
	float splitRange;
	int numSplits = 4;
	Vector3 initialDirection;

	public override void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.SplitArrow) {
		speedModifier = 0.8f;
		rangeModifier = 1.0f;

		this.speed = speedModifier * speed;
		this.range = rangeModifier * range;
		splitRange = range * 0.5f;
		this.charge = charge;
		this.type = type;

		if (split) {
			shootSound = (AudioClip)Resources.Load ("Audio/shooting");;
		} else {
			shootSound = (AudioClip)Resources.Load ("Audio/swordSwing2");
		}
		AudioSource source = AudioHelper.PlayClipAt (shootSound, this.gameObject.transform.position);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 30.0f;

		hitDetails = new int[3];
		gameObject.renderer.material = (Material)Resources.Load("Materials/SplitArrow");
		Destroy (this.gameObject, (splitRange/speed) / 6);

		splitAngle = -60.0f / (charge + 1);
		splitIncrement = 120 / ((charge + 1) * (numSplits));
	}

	protected override void OnDestroy() {
		if (!split) {
			initialDirection = Vector3.Normalize (this.gameObject.rigidbody.velocity);
			Vector3 perpendicular = Vector3.Cross(initialDirection, Vector3.up);

			spawnSplitArrows(splitAngle, perpendicular);
			splitAngle += splitIncrement;

			spawnSplitArrows(splitAngle, perpendicular / 2);
			splitAngle += splitIncrement;

			spawnSplitArrows(splitAngle, new Vector3());
			splitAngle += splitIncrement;

			spawnSplitArrows(splitAngle, -perpendicular / 2);
			splitAngle += splitIncrement;

			spawnSplitArrows(splitAngle, -perpendicular);
			splitAngle += splitIncrement;

			GameObject splitEffect = (GameObject)Instantiate (Resources.Load ("Particles/Split Effect"), this.transform.position, Quaternion.identity);
			ParticleSystem splitSystem = splitEffect.GetComponent<ParticleSystem> ();
			splitSystem.startSpeed += (charge + 1) * 5.0f;
			splitSystem.transform.Rotate(Quaternion.LookRotation(initialDirection).eulerAngles);
			splitSystem.Emit(Mathf.FloorToInt(40 * (charge + 1)));
			Destroy(splitEffect, splitSystem.duration);
		}
	}

	private void spawnSplitArrows(float angle, Vector3 offset) {
		Vector3 initialPosition = this.transform.position;

		GameObject splitArrow;
		
		Vector3 splitDirection = Quaternion.Euler(0, angle, 0) * initialDirection;
		splitArrow = (GameObject)Instantiate (Resources.Load ("ArrowPrefab"), initialPosition + offset, Quaternion.LookRotation(splitDirection));
		this.transform.RotateAround(this.gameObject.transform.position, Vector3.up, angle);
		splitArrow.rigidbody.velocity = Quaternion.LookRotation(splitDirection) * new Vector3(0.0f, 0.0f, speed);
		
		splitArrow.renderer.material = (Material)Resources.Load("Materials/SplitArrow");
		SplitArrow splitComponent = splitArrow.AddComponent<SplitArrow> ();
		splitComponent.owner = this.owner;
		splitComponent.split = true;
		splitComponent.Initialize(speed, range * 10, charge, type);
	}
}