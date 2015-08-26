using UnityEngine;
using System.Collections;

public class TreeArrow : Arrow {

	public override void Initialize (float speed, float range, float charge, ArrowType type = ArrowType.FireArrow) {
		speedModifier = 1.5f;
		rangeModifier = 0.1f;
		base.Initialize (speed, range, charge, type);
		gameObject.renderer.material = (Material)Resources.Load("Materials/TreeArrow");
	}
	
	protected override void OnDestroy() {
		//print ("From: " + startingPos + " To: " + this.transform.position);
		//print ("Distance: " + Vector3.Distance(startingPos, this.transform.position));
		int actualCharge = (int)(3 * charge);
		if (actualCharge > 3) {
			actualCharge = 3;
		}
		int numberOfTrees = 3 + (2 * (int)actualCharge);

		Vector3 position = this.transform.position - new Vector3 (0, 0.5f, 0);
		Vector3 direction = Vector3.Normalize (this.gameObject.rigidbody.velocity);
		Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);

		float offset = (numberOfTrees > 5) ? numberOfTrees * 0.31f : 0.05f;
		//Vector3 perpendicular = Vector3.Normalize(Vector3.Cross(position, Vector3.up)) + position;
		Quaternion lookDirection = Quaternion.LookRotation (perpendicular);
		//perpendicular -= lookDirection;// * new Vector3 (0 , 0, offset * (numberOfTrees / 2));

		for(int i = 0; i < numberOfTrees; i++) {
			GameObject tree = (GameObject)Instantiate (Resources.Load ("Tree"), perpendicular + position + (lookDirection * new Vector3(0, 0, offset)), Quaternion.identity);
			tree.AddComponent<Rigidbody> ();
			tree.rigidbody.mass = 2;
			tree.rigidbody.drag = 2;
			tree.rigidbody.isKinematic = true;
			tree.rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			tree.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			tree.AddComponent<Walls> ();
			offset--;
		}
		//tree.transform.localScale = new Vector3 (1 + charge, 1, 1 + charge);
	}
}
