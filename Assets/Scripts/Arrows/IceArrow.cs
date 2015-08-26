using UnityEngine;
using System.Collections;

public class IceArrow : Arrow {

	public override void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.IceArrow) {
		speedModifier = 0.9f;
		rangeModifier = 0.8f;
		base.Initialize(speed, range, charge, type);
		gameObject.renderer.material = (Material)Resources.Load("Materials/IceArrow");
	}
	
	protected override void OnDestroy() {
		//print ("From: " + startingPos + " To: " + this.transform.position);
		//print ("Distance: " + Vector3.Distance(startingPos, this.transform.position));
		GameObject ice = (GameObject)Instantiate (Resources.Load ("Particles/Ice"), this.transform.position - new Vector3(0, 0.4f, 0), Quaternion.identity);
		float scale = 1 + (charge * 5);
		ice.transform.localScale += new Vector3 (scale, 0, scale);
		ParticleSystem[] clouds = ice.GetComponentsInChildren<ParticleSystem> ();
		clouds [0].startSize += charge;
		clouds [0].emissionRate += 120 * (1 + charge);
		clouds [0].particleSystem.Simulate (1);
		clouds [0].Play ();
		IceEffect effect = ice.AddComponent<IceEffect>();
		effect.Initialize (owner, charge);

	}

	public override void onHit() {
		
	}
}