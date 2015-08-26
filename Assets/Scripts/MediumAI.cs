using UnityEngine;
using System.Collections;

//Have AI difficulty scale with player performance
//Maybe be dependent on how long the player has gone without dying?
//May require a "single-player" setting
//Features:
//Make AI utilize tree arrows or dash to block incoming attacks (low-average RNG)
//Possible vendetta/revenge feature
//Shoot trees and crates
//Collect powerups

public class MediumAI : Character {
	private class TargetInformation {
		public Character target;
		public float distance;
		public float velocityMagnitude;
	}
	
	private ArrayList targetsInRange;
	private TargetInformation primaryTarget;
	
	private float defaultMovespeed = 5.0f;
	private float chargeTime = 0;
	
	private float actionCooldownAmount = 0.7f;
	private float actionCooldown = 0;
	
	Quiver quiver;
	
	public override void initialize (int characterID, GameObject gameManager) {
		targetsInRange = new ArrayList ();
		primaryTarget = null;
		base.initialize (characterID, gameManager);
		bow.Initialize ();
		currentMovementSpeed = defaultMovespeed;
		anim.StopPlayback ();
		
		shotTimer = Random.Range(0.5f, 5.0f);
		
		quiver = new Quiver ();
		quiver.Initialize (7, this.gameObject);
		quiver.addArrow (ArrowType.FireArrow, 3);
		quiver.addArrow (ArrowType.IceArrow, 3);
		quiver.addArrow (ArrowType.ForceArrow, 3);
		quiver.addArrow (ArrowType.TreeArrow, 3);
		quiver.addArrow (ArrowType.PiercingArrow, 3);
		quiver.addArrow (ArrowType.SplitArrow, 3);
		
		this.transform.position = new Vector3 (this.transform.position.x, 0.0f, this.transform.position.z);
	}
	
	//---------------------------------------------------Targetting Section---------------------------------------------------//
	private void populateTargetsInRange(ArrayList characterList) {
		for(int i = 0; i < characterList.Count; i++) {
			Character target = ((GameObject)characterList[i]).GetComponent<Character> ();
			
			if(target.ID != this.ID) {
				float distance = Vector3.Distance(this.transform.position, target.transform.position);
				if(distance <= bow.getMaxRange() * 1.3f) { //25 * 1.3 = 30 range
					if(!targetsInRange.Contains(target)) {
						targetsInRange.Add(target);
					}
				}
			}
		}
	}
	
	private void removeTargetsOutOfRange() {
		for(int i = 0; i < targetsInRange.Count; i++) {
			Character target = (Character)targetsInRange[i];
			float distance = Vector3.Distance(this.transform.position, target.transform.position);
			if(distance > bow.getMaxRange() * 1.3f) {
				targetsInRange.Remove(target);
			}
		}
		primaryTarget = null;
	}
	
	private TargetInformation getClosestTarget(ArrayList characterList) {
		TargetInformation result = new TargetInformation();
		float shortestDistance = 200;
		for(int i = 0; i < characterList.Count; i++) {
			Character potentialTarget = (Character)characterList[i];
			
			if(potentialTarget.ID != this.ID) {
				float newDistance = Vector3.Distance(this.transform.position, potentialTarget.transform.position);
				if(newDistance < shortestDistance && potentialTarget.isAlive()) {
					shortestDistance = newDistance;
					result.target = potentialTarget;
				}
			}
		}
		if (result.target) {
			result.distance = shortestDistance;
			result.velocityMagnitude = Vector3.Magnitude (result.target.rigidbody.velocity);
		}
		
		return result;
	}
	
	private TargetInformation getSlowestTarget(ArrayList characterList) {
		TargetInformation result = new TargetInformation ();
		float slowestVelocity = 10000;
		for(int i = 0; i < characterList.Count; i++) {
			Character potentialTarget = (Character)characterList[i];
			
			if(potentialTarget.ID != this.ID) {
				float targetVelocity = Vector3.Magnitude(potentialTarget.rigidbody.velocity);
				if(targetVelocity < slowestVelocity) {
					slowestVelocity = targetVelocity;
					result.target = potentialTarget;
				}
			}
		}
		
		if (result.target) {
			result.distance = Vector3.Distance (result.target.transform.position, this.transform.position);
			result.velocityMagnitude = slowestVelocity;
		}
		
		return result;
	}
	
	private int indexInScoreList() {
		int result = -1;
		ScoreManager scoreManager = gameManager.GetComponent<ScoreManager> ();
		ArrayList list = scoreManager.characterScores;
		for(int i = 0; i < list.Count; i++) {
			if(((ScoreManager.Score)list[i]).character == this.GetComponent<Character>()) {
				result = i;
				i = list.Count;
			}
		}
		return result;
	}
	
	//Target Priority:
	//1. If the current leader is winning by 5+ points more than this AI and is within firing range
	//2. If the slowest target is also the closest
	//3. If the slowest target is closer than half of the max Range
	//4. If the slowest target is not moving
	//5. The closest target
	private void choosePrimaryTarget() {
		ScoreManager scoreManager = gameManager.GetComponent<ScoreManager> ();
		int index = indexInScoreList();
		int AIScore = ( (ScoreManager.Score)scoreManager.characterScores[index] ).score;
		
		TargetInformation leaderTarget = null;
		if (scoreManager.leadScore - AIScore > 4) {
			leaderTarget = new TargetInformation();
			leaderTarget.target = ((ScoreManager.Score)scoreManager.characterScores [scoreManager.leaderIndex]).character;
			if(leaderTarget.target != null) {
				leaderTarget.distance = Vector3.Distance (this.transform.position, leaderTarget.target.transform.position);
				leaderTarget.velocityMagnitude = Vector3.Magnitude (leaderTarget.target.rigidbody.velocity);
			}
		}
		
		if (leaderTarget != null) {
			if(leaderTarget.target != null) {
				if(leaderTarget.distance <= bow.getMaxRange ()) {
					primaryTarget = leaderTarget;
				}
			}
		}
		if(primaryTarget == null) {
			TargetInformation closest = getClosestTarget (targetsInRange);
			TargetInformation slowest = getSlowestTarget (targetsInRange);
			if (!closest.target) {
				primaryTarget = slowest;
			} else if (!slowest.target) {
				primaryTarget = closest;
			} else if (closest.distance <= 10) {
				primaryTarget = closest;
			} else if (closest.target.ID == slowest.target.ID) {
				primaryTarget = slowest;
			} else if (slowest.distance <= bow.getMaxRange () / 2) {
				primaryTarget = slowest;
			} else if (slowest.velocityMagnitude == 0) {
				primaryTarget = slowest;
			} else {
				primaryTarget = closest;
			}
		}
	}
	//---------------------------------------------------Charging Section---------------------------------------------------//
	private void calculateChargeTime() {
		if (primaryTarget.distance <= (bow.getMaxRange () * 0.4f)) { //25 * 0.4 = 10
			chargeTime = Random.Range (0.5f, 1.0f);
		} else if (primaryTarget.distance <= bow.getMaxRange () * 0.8f) { //25 * 0.8 = 20
			chargeTime = Random.Range (1.0f, 2.0f);
		} else if (primaryTarget.distance < (bow.getMaxRange () * 1.2f)) { //25 * 1.2 = 30
			chargeTime = Random.Range (2.0f, 3.0f);
		}
		isCharging = true;
	}
	
	private void ChargingLogic() {
		bow.Charge();
	}
	//---------------------------------------------------Firing Section---------------------------------------------------//
	private Quaternion aimAtTarget() {
		float randomRange = 35.0f;
		Vector3 position = (primaryTarget.target) ? primaryTarget.target.transform.position : 
			new Vector3(Random.Range (-randomRange, randomRange), 1.0f, Random.Range (-randomRange, randomRange));
		float trackingStrength = Random.Range(0.6f, 1.5f);
		float timeToImpact = primaryTarget.distance / bow.getSpeed ();
		
		Vector3 movementDueToVelocity = (timeToImpact * trackingStrength) * primaryTarget.target.rigidbody.velocity;
		
		Vector3 finalTargetPosition = position + movementDueToVelocity;
		Vector3 direction = finalTargetPosition - this.transform.position;
		direction.y = 1.0f;
		
		return Quaternion.LookRotation(Vector3.Normalize(direction));
	}
	
	private Arrow getRandomArrowType(GameObject arrowThing) {
		Arrow arrowComponent = null;
		ArrowType type = ArrowType.Arrow;
		bool validResult = false;
		int numberOfTries = 0;
		do {
			int value = Random.Range (0, 100);
			if (value > 85) {
				type = ArrowType.FireArrow;
			} else if (value > 65) {
				type = ArrowType.SplitArrow;
			} else if (value > 50) {
				type = ArrowType.IceArrow;
			} else if (value > 35) {
				type = ArrowType.ForceArrow;
			} else if (value > 20) {
				type = ArrowType.PiercingArrow;
			} else {
				type = ArrowType.TreeArrow;
			}
			
			if(quiver.pullArrow(type)) {
				switch(type) {
				case ArrowType.FireArrow:
					arrowComponent = arrowThing.AddComponent<FireArrow> ();
					break;
				case ArrowType.IceArrow:
					arrowComponent = arrowThing.AddComponent<IceArrow> ();
					break;
				case ArrowType.ForceArrow:
					arrowComponent = arrowThing.AddComponent<ForceArrow> ();
					break;
				case ArrowType.SplitArrow:
					arrowComponent = arrowThing.AddComponent<SplitArrow> ();
					break;
				case ArrowType.PiercingArrow:
					arrowComponent = arrowThing.AddComponent<PiercingArrow> ();
					break;
				case ArrowType.TreeArrow:
					arrowComponent = arrowThing.AddComponent<TreeArrow> ();
					break;
				default:
					arrowComponent = arrowThing.AddComponent<Arrow>();
					break;
				}
				validResult = true;
			}
			numberOfTries++;
		} while (!validResult && numberOfTries < 3);
		
		if (!arrowComponent) {
			arrowComponent = arrowThing.AddComponent<Arrow>();
		}
		return arrowComponent;
	}
	
	private void FiringLogic() {
		choosePrimaryTarget ();
		if (primaryTarget != null) {
			Quaternion victim = aimAtTarget ();
			
			Vector3 extraZ = victim * (new Vector3 (0, 0, 1.4f));
			GameObject arrowThing = (GameObject)Instantiate (Resources.Load ("ArrowPrefab"), this.transform.position + extraZ, victim);
			
			int specialArrowChance = 35;
			Arrow arrowComponent = null;
			if (Random.Range (0, 100) < specialArrowChance) {
				arrowComponent = getRandomArrowType (arrowThing);
			} else {
				arrowComponent = arrowThing.AddComponent<Arrow> ();
			}
			arrowComponent.owner = this.gameObject;
			bow.Release (victim, arrowThing);
			this.rigidbody.velocity = new Vector3 (0, 0, 0);
			anim.SetFloat("Speed", 0);
		}
		
		removeTargetsOutOfRange ();
	}
	//---------------------------------------------------Movement Section---------------------------------------------------//
	private Vector3 getMovementPoint() {
		float movementRange = bow.getMaxRange ();
		Vector3 randomMovePoint = new Vector3(Random.Range (-movementRange, movementRange), 0.5f, Random.Range (-movementRange, movementRange));
		return primaryTarget.target.transform.position + randomMovePoint;
	}
	
	private Vector3 getRandomMovementPoint() {
		float mapRange = 100;
		return new Vector3 (Random.Range (-mapRange, mapRange), 0.5f, Random.Range (-mapRange, mapRange));
	}
	
	private Vector3 getMoveDirection(Vector3 to) {
		return Vector3.Normalize ((to - this.transform.position));
	}
	
	private void OffensiveDash() {
		if (primaryTarget != null) {
			Vector3 direction = Vector3.Normalize(primaryTarget.target.transform.position - this.transform.position);
			float angle = Random.Range(-25.0f, 25.0f);
			direction = Quaternion.Euler(0, angle, 0) * direction;
			this.gameObject.AddComponent<Accelerate>().Initialize(50 * direction,
			                                                      0.4f, 4.0f, true);
			AudioSource source = AudioHelper.PlayClipAt (dashSound, this.gameObject.transform.position);
			source.rolloffMode = AudioRolloffMode.Linear;
			abilityExecuted = true;
			source.maxDistance = 50.0f;

			dashSystem = (GameObject)Instantiate (Resources.Load("Particles/Air Dash Effect"), this.transform.position - (Vector3.Normalize(this.gameObject.rigidbody.velocity)),
			                                      Quaternion.LookRotation (Vector3.Normalize (-this.gameObject.rigidbody.velocity)));
			dashSystem.transform.parent = gameObject.transform;
			Destroy (dashSystem, 0.4f);
		}
	}
	
	private void MovementLogic(float delay) {
		Vector3 movePoint = (primaryTarget != null) ? getMovementPoint() : getRandomMovementPoint();
		Quaternion dir = Quaternion.LookRotation (getMoveDirection (movePoint));
		
		Vector3 moveVelocity = new Vector3 (0, 0, currentMovementSpeed);
		this.rigidbody.velocity = ((dir * moveVelocity));
		
		if(Vector3.Magnitude(this.gameObject.rigidbody.velocity) > 0) {
			this.gameObject.transform.rotation = Quaternion.LookRotation (Vector3.Normalize (this.gameObject.rigidbody.velocity));
			anim.SetFloat("Speed", 5);
		}
		
		moveDelay = delay;
		moveTimer = 0;
	}
	//---------------------------------------------------Update Function---------------------------------------------------//
	public override void Update () {
		base.Update ();
		if(!isDead && !movementLocked && actionCooldown <= 0 ) {
			float velocityMagnitude = Vector3.Magnitude(this.rigidbody.velocity);
			if(isSlowed && velocityMagnitude > 1) {
				currentMovementSpeed = 0.5f;
				Quaternion dir = Quaternion.LookRotation(Vector3.Normalize(this.rigidbody.velocity));
				this.rigidbody.velocity = dir * new Vector3(0, 0, currentMovementSpeed);
				anim.SetFloat("Speed", 5);
			} else if(!isSlowed && velocityMagnitude < (defaultMovespeed * 0.55f)) {
				MovementLogic(4.0f);
			}
			
			if(shotTimer <= 0) {
				ArrayList characterList = gameManager.GetComponent<SpawnManager> ().characterList;
				populateTargetsInRange(characterList);
				
				if(targetsInRange.Count > 0) {
					choosePrimaryTarget ();
					calculateChargeTime ();
					shotTimer = 50;
				} else {
					shotTimer = 1;
				}
			}
			
			if(isCharging) {
				ChargingLogic();
				if(!isSlowed) {
					currentMovementSpeed = defaultMovespeed * 0.6f;
				}
				chargeTime -= Time.deltaTime;
				
				if(0.43f < chargeTime && chargeTime < 0.45f) {
					int specialManeuverChance = 35;
					if(Random.Range(0, 100) < specialManeuverChance &! abilityExecuted &! abilityOnCooldown) {
						OffensiveDash();
					}
				}
				
				if (chargeTime <= 0) {
					FiringLogic();
					if(!isSlowed) {
						currentMovementSpeed = defaultMovespeed;
					}
					
					actionCooldown = actionCooldownAmount;
					moveDelay = actionCooldown;
					
					shotTimer = Random.Range(1.5f, 3.0f);
					moveTimer = 0;
					isCharging = false;
				}
			}
			
			if (moveTimer >= moveDelay) {
				MovementLogic(Random.Range(4.0f, 8.0f));
			}
			else if(moveDelay > 0.0f)
			{
				moveDelay -= Time.deltaTime;
			}
		}
		
		if (abilityCooldown > 0.0f) {
			abilityCooldown -= Time.deltaTime;
			
			if (abilityCooldown <= 0.0f)
				abilityOnCooldown = false;
		}
		
		if (isDead) {
			bow.resetCharge();
			isCharging = false;
			chargeTime = 5;
			shotTimer = 0;
		}
		
		actionCooldown -= Time.deltaTime;
		shotTimer -= Time.deltaTime;
		moveTimer += Time.deltaTime;
		spawnTimer();
	}
	
	public override void addAmmo(ArrowType type) {
		quiver.addArrow (type, 1);
	}
}
