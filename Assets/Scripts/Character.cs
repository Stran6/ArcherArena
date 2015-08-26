using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	public int ID;
	protected LayerMask mask = 1 << LayerMask.NameToLayer("Floor");
	protected GameObject gameManager;
	
	protected float shotTimer = 2.0f;
	
	protected const float movementSpeed = 6.5f;
	protected float currentMovementSpeed = movementSpeed;
	protected const float chargingMovementSpeed = 2.5f;
	protected float moveTimer = 10.0f;
	protected float moveDelay = 0.0f;
	
	protected float abilityCooldown = 0.0f;
	protected bool abilityExecuted = false;
	protected bool abilityOnCooldown = false;
	
	protected float respawnDelay = 3.0f;
	protected float respawnTimer = 0.0f;
	protected float invulTimer = 0.0f;
	
	protected bool isDead = false;
	protected bool isCharging = false;
	protected bool isSlowed = false;
	protected bool movementLocked = false;

	protected GameObject invulSystem;
	protected GameObject dashSystem;
	
	protected Vector3 currentMovement = new Vector3();
	protected Bow bow = new Bow();
	
	protected AudioClip dashSound;
	protected AudioClip deathSound;
	
	public bool tutorialMode = false;
	public Vector3 tutorialRespawnLocation;

	protected Animator anim;
	
	public virtual void initialize(int characterID, GameObject gameManager) {
		ID = characterID;
		this.gameManager = gameManager;
		
		dashSound = (AudioClip)Resources.Load("Audio/fireSpell");
		deathSound = (AudioClip)Resources.Load("Audio/die2");

		anim = this.GetComponent<Animator> ();
	}
	
	public virtual void Update () {
		if (invulTimer > 0) {
			invulTimer -= Time.deltaTime;
			if(invulSystem)
				invulSystem.transform.position = gameObject.transform.position;
		}
	}
	
	protected void spawnTimer() {
		if (isDead) {
			if (respawnTimer < respawnDelay) {
				respawnTimer += Time.deltaTime;
				shotTimer = 2.0f;
			} else {
				revive ();
				respawnTimer = 0;
			}
		}
	}
	
	public void lockMovement() {
		movementLocked = true;
	}
	public void unlockMovement() {
		movementLocked = false;
	}
	
	public bool isAlive() {
		return !isDead;
	}
	
	public virtual void onHit(int[] hitDetails) {
		if(!isDead && invulTimer <= 0) {
			if (System.Convert.ToBoolean(hitDetails[2])) {
				ScoreManager scoreManager = gameManager.GetComponent<ScoreManager> ();
				scoreManager.changeScore(hitDetails[0], hitDetails[1]);
			}
			
			bow.resetCharge();
			
			isDead = true;
			AudioSource source = AudioHelper.PlayClipAt (deathSound, this.gameObject.transform.position);
			source.rolloffMode = AudioRolloffMode.Linear;
			source.maxDistance = 50.0f;
			
			this.gameObject.rigidbody.velocity = new Vector3(0,0,0);
			abilityCooldown = 0;
			abilityOnCooldown = false;
			abilityExecuted = false;
			
			GameObject bloodSystem = (GameObject)Instantiate(Resources.Load("Particles/Blood Splatter"), this.gameObject.transform.position, Quaternion.identity);
			Destroy (bloodSystem, 11.0f);
			
			Vector3 newPos = new Vector3 (this.transform.position.x, -4, this.transform.position.z);
			this.transform.position = newPos;
		}
	}
	
	public virtual void addAmmo(ArrowType type) {
	}
	
	public virtual void iceEffect(float duration) {
		isSlowed = true;
		currentMovementSpeed = 0.5f;
		StartCoroutine (slowEffect (duration));
	}
	
	protected virtual IEnumerator slowEffect(float duration) {
		yield return new WaitForSeconds (duration);
		isSlowed = false;
		currentMovementSpeed = movementSpeed;
	}
	
	public virtual void revive() {
		isDead = false;
		currentMovementSpeed = movementSpeed;
		anim.SetFloat ("Speed", 0);
		
		if (tutorialMode) {
			this.gameObject.transform.position = tutorialRespawnLocation;
		} else {
			SpawnManager spawner = gameManager.GetComponent<SpawnManager> ();
			spawner.respawn (this.gameObject);
			invulTimer = 2.0f;
			invulSystem = (GameObject)Instantiate (Resources.Load("Particles/Invulnerable"));
			invulSystem.transform.position = gameObject.transform.position;
			invulSystem.particleSystem.Play();
			Destroy (invulSystem, 2.0f);
		}
	}
	
	public virtual void abilityComplete(float cooldown) {
		if (!isDead) {
			abilityCooldown = cooldown;
			abilityOnCooldown = true;
			abilityExecuted = false;
		}
	}
}
