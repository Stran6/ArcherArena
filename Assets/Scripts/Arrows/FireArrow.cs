using UnityEngine;
using System.Collections;

public class FireArrow : Arrow {

	public override void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.FireArrow) {
		speedModifier = 0.7f;
		rangeModifier = 0.6f;
		base.Initialize (speed, range, charge, type);
		gameObject.renderer.material = (Material)Resources.Load("Materials/FireArrow");
	}
	
	protected override void OnDestroy() {
		//print ("From: " + startingPos + " To: " + this.transform.position);
		//print ("Distance: " + Vector3.Distance(startingPos, this.transform.position));
		GameObject fire = (GameObject)Instantiate (Resources.Load ("Particles/Fire"), this.transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
		float scale = 1 + charge;
		fire.transform.localScale += new Vector3 (scale, 0, scale);
		ParticleSystem[] flames = fire.GetComponentsInChildren<ParticleSystem> ();
		flames [0].startSize += charge;
		flames [0].emissionRate += 30 * (1 + charge);
		flames [0].particleSystem.Simulate (1);
		flames [0].Play ();
		FireEffect effect = fire.AddComponent<FireEffect>();
		effect.Initialize (owner, charge);
	}

	public override void onHit() {
		
	}
}
