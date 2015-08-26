using UnityEngine;
using System.Collections;

public class TutorialAI : Character {
	private float defaultMovespeed = 5.0f;
	private float chargeTime = 0;
	private float currentChargeTime = 0;
	private int reverser = 1;
	private GameObject shootTarget;
	
	Quiver quiver;

	public bool tutorialMove = false;
	public bool shootEnabled = false;
	public bool patrol = false;


	public override void initialize (int characterID, GameObject gameManager) {
		base.initialize (characterID, gameManager);
		bow.Initialize ();
		currentMovementSpeed = defaultMovespeed;

		quiver = new Quiver ();
		quiver.Initialize (7, this.gameObject);
		quiver.addArrow (ArrowType.FireArrow, 3);
		quiver.addArrow (ArrowType.IceArrow, 3);
		quiver.addArrow (ArrowType.ForceArrow, 3);;
	}
	//---------------------------------------------------Firing Section---------------------------------------------------//
	public void setShooting(GameObject target, float charge){
		shootTarget = target;
		chargeTime = charge;
		currentChargeTime = charge;
		shootEnabled = true;
	}

	private Quaternion aimAtTarget() {
		Vector3 startPos = this.transform.position;
		Vector3 endPos = shootTarget.transform.position;
		Vector3 direction = endPos - startPos;
		direction.y = 1.0f;
		
		return Quaternion.LookRotation(Vector3.Normalize(direction));
	}
	
	private void FiringLogic() {
		Quaternion victim = aimAtTarget();
		
		Vector3 extraZ = victim * (new Vector3 (0, 0, 1.3f));
		Vector3 startPos = this.transform.position;
		startPos.y = 1.0f;
		GameObject arrowThing = (GameObject)Instantiate (Resources.Load("ArrowPrefab"), startPos + extraZ, victim);
		
		Arrow arrowComponent = arrowThing.AddComponent<Arrow>();
		arrowComponent.owner = this.gameObject;
		
		bow.Release(victim, arrowThing);
		this.rigidbody.velocity = new Vector3 (0, 0, 0);
	}
	//---------------------------------------------------Movement Section---------------------------------------------------//
	private Vector3 getRandomMovementPoint() {
		float mapRange = 100;
		return new Vector3 (Random.Range (-mapRange, mapRange), 0.5f, Random.Range (-mapRange, mapRange));
	}
	
	private Vector3 getMoveDirection(Vector3 to) {
		return Vector3.Normalize ((to - this.transform.position));
	}
	
	private void MovementLogic(float delay) {
		Vector3 movePoint = getRandomMovementPoint();
		Quaternion dir = Quaternion.LookRotation (getMoveDirection (movePoint));
		
		Vector3 moveVelocity = new Vector3 (0, 0, currentMovementSpeed);
		this.rigidbody.velocity = ((dir * moveVelocity));
		
		moveDelay = delay;
		moveTimer = 0;
	}

	private void PatrolLogic(int reverse) {
		this.rigidbody.velocity = (new Vector3(reverse * currentMovementSpeed, 0, 0));
		this.gameObject.transform.rotation = Quaternion.LookRotation (Vector3.Normalize (this.gameObject.rigidbody.velocity));
		anim.SetFloat ("Speed", 5);
		moveDelay = 3;
		moveTimer = 0;
	}
	//---------------------------------------------------Update Function---------------------------------------------------//
	public override void Update () {
		base.Update ();
		if (!isDead && !movementLocked) {
			if(tutorialMove) {
				float velocityMagnitude = Vector3.Magnitude(this.rigidbody.velocity);
				if(isSlowed && velocityMagnitude > 1) {
					currentMovementSpeed = 0.5f;
					Quaternion dir = Quaternion.LookRotation(Vector3.Normalize(this.rigidbody.velocity));
					this.rigidbody.velocity = dir * new Vector3(0, 0, currentMovementSpeed);
				} else if(!isSlowed && velocityMagnitude < (defaultMovespeed * 0.55f)) {
					MovementLogic(4.0f);
				}
				
				if (moveTimer >= moveDelay) {
					MovementLogic(Random.Range(4.0f, 8.0f));
				}
				else if(moveDelay > 0.0f)
				{
					moveDelay -= Time.deltaTime;
				}
			}

			if(patrol) {
				if(isSlowed) {
					currentMovementSpeed = 0.5f;
					PatrolLogic(reverser);
				}
				else {
					currentMovementSpeed = defaultMovespeed;
				}

				if(moveTimer >= moveDelay) {
					reverser *= -1;
					PatrolLogic(reverser);
				}
				else if(moveDelay > 0.0f){
					moveDelay -= Time.deltaTime;
				}
			}
			
			if (shootEnabled) {
				if(shotTimer <= 0.0f){
					if(currentChargeTime > 0.0f){
						bow.Charge();
					} else {
						FiringLogic();
						currentChargeTime = chargeTime;
						shotTimer = 0.9f;
					}
				}	
			}
			
			moveTimer += Time.deltaTime;
			shotTimer -= Time.deltaTime;
			currentChargeTime -= Time.deltaTime;
		}

		spawnTimer();
	}

	public override void onHit(int[] hitDetails) {
		bow.resetCharge();
		this.gameObject.rigidbody.velocity = new Vector3(0,0,0);
		abilityCooldown = 0;
		abilityOnCooldown = false;
		abilityExecuted = false;
		
		Vector3 newPos = new Vector3 (this.transform.position.x, -1, this.transform.position.z);
		this.transform.position = newPos;
		moveTimer = 0;
		shotTimer = 0;
		isDead = true;

		Vector3 bloodPosition = this.gameObject.transform.position;
		bloodPosition.y = 0.1f;
		GameObject bloodSystem = (GameObject)Instantiate(Resources.Load("Particles/Blood Splatter"), bloodPosition, Quaternion.identity);
		Destroy (bloodSystem, 11.0f);

		Destroy (this.gameObject);
	}
}
