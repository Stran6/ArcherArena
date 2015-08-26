using UnityEngine;
using System.Collections;

public class Player : Character {
	Quiver quiver;
	ArrowType currentAmmo;
	private float noAmmoTimer = 0.0f;
	private bool noAmmoDisplay = false;
	private Controller controller;

	public override void initialize (int characterID, GameObject gameManager) {
		base.initialize (characterID, gameManager);
		bow.Initialize ();
		bow.instantiateRangeIndicator (this.gameObject);
		quiver = new Quiver ();
		quiver.Initialize (7, this.gameObject);
		controller = new Controller ();
		controller.Initialize (currentMovementSpeed);
		setMainPlayer ();

		quiver.addArrow (ArrowType.FireArrow, 3);
		quiver.addArrow (ArrowType.IceArrow, 3);
		quiver.addArrow (ArrowType.ForceArrow, 3);
		quiver.addArrow (ArrowType.TreeArrow, 3);
		quiver.addArrow (ArrowType.PiercingArrow, 3);
		quiver.addArrow (ArrowType.SplitArrow, 3);

		currentAmmo = ArrowType.Arrow;
	}

	public void setMainPlayer() {
		//Camera.main.transform.parent = this.gameObject.transform;
		Camera.main.transform.position = new Vector3 (this.gameObject.transform.position.x,
		                                              36.0f, this.gameObject.transform.position.z + 10);
	}

	Vector3 getMovementPoint() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit = new RaycastHit ();
		
		Vector3 hitLocation = new Vector3();
		if (Physics.Raycast (ray, out hit, 300, mask.value)) {
			hitLocation = hit.point;
		}
		hitLocation.y = 1.0f;
		return hitLocation;
	}
	
	Vector3 getMouseDirection() {
		return Vector3.Normalize ((getMovementPoint() - this.transform.position));
	}

	bool arrowOnCooldown = false;
	Rect arrowDisplay;
	Rect abilityDisplay;
	void OnGUI() {
		quiver.GUIDisplay (currentAmmo);

		float minWidth = 0.20f;
		int graphicalHorizontalOffset = 30;
		int horizontalOffset = 12;
		int verticalOffset = 50;
		int boxWidth = 50;
		int boxHeight = 20;

		#region Death GUI
		GUIStyle style = new GUIStyle ();
		style.fontSize = 16;
		style.normal.textColor = new Color (1, 0, 0);
		if (isDead) {
			string deathMessage = "You have died. Respawn in";
			float deathCooldown = respawnDelay - respawnTimer;
			deathCooldown = (deathCooldown <= 0) ? 0 : deathCooldown;
			string deathCooldownString = deathCooldown.ToString("F2") + " seconds";
			GUI.Label(new Rect(Screen.width/2 - (graphicalHorizontalOffset*2), Screen.height/2 + (verticalOffset/2) , 300, 100), deathMessage, style);
			GUI.Label(new Rect(Screen.width/2 - horizontalOffset, Screen.height/2 + verticalOffset, 300, 100), deathCooldownString, style);
		}
		#endregion

		#region ShootGUI
		if (shotTimer < bow.getReloadDelay()) {
			if (!arrowOnCooldown) {
				arrowDisplay = new Rect(Screen.width / 2 - graphicalHorizontalOffset, (Screen.height / 2) - verticalOffset, boxWidth, boxHeight/2);
				arrowOnCooldown = true;
			}
			//Graphical Cooldown
			arrowDisplay.width = (boxWidth) * ( ((bow.getReloadDelay() - shotTimer)/bow.getReloadDelay()) + minWidth);
			GUI.Box(arrowDisplay, "");
			//Numerical Cooldown
			string cooldown = (bow.getReloadDelay () - shotTimer).ToString ("F2");
			GUI.Label (new Rect (Screen.width / 2 - horizontalOffset, (Screen.height / 2) - (verticalOffset + 5), boxWidth, boxHeight), cooldown);
		} else {
			arrowOnCooldown = false;
		}
		#endregion

		#region DashGUI
		if(abilityCooldown > 0) {
			abilityDisplay = new Rect(Screen.width / 2 - graphicalHorizontalOffset, (Screen.height / 2) - verticalOffset/4, boxWidth, boxHeight/2);

			//Graphical Cooldown
			abilityDisplay.width = (boxWidth) * ((abilityCooldown/4) + minWidth);
			GUI.Box(abilityDisplay, "");

			//Numerical Cooldown
			string abilityCooldownText = abilityCooldown.ToString ("F2");
			GUI.Label (new Rect (Screen.width / 2 - horizontalOffset, (Screen.height / 2) - (verticalOffset/4 + 4), boxWidth, boxHeight), abilityCooldownText);
		}
		#endregion

		if (noAmmoDisplay) {
			style.fontSize = 20;
			string noAmmoText = "NO AMMO";
			GUI.Label (new Rect (Screen.width / 2 - (horizontalOffset*3.4f), (Screen.height / 2) - (verticalOffset*0.1f), boxWidth, boxHeight), noAmmoText, style);
		}
	}

	//GameObject rangeIndicator;
	public override void Update () {
		base.Update ();
		controller.updateMovementSpeed (currentMovementSpeed);

		Quaternion direction = Quaternion.LookRotation (getMouseDirection ());
		Vector3 extraZ = direction * (new Vector3 (0, 0, 1.3f));
		if (!isDead) {
			if ((Input.GetMouseButton (0) || Input.GetMouseButton(1)) && shotTimer >= bow.getReloadDelay () ) {
				if(!(Input.GetMouseButton(0) && Input.GetMouseButton (1))) {
					if (!isCharging) {
						if(Input.GetMouseButton(1)) {
							bool arrowAvailable = false;
							if(quiver.arrowInQuiver(currentAmmo)) {
								arrowAvailable = true;
							} else {
								arrowAvailable = false;
								noAmmoDisplay = true;
								noAmmoTimer = 1.0f;
							}
							bow.initializeRangeIndicator(direction, currentAmmo, arrowAvailable);

						} else {
							bow.initializeRangeIndicator(direction, ArrowType.Arrow, true);
						}
					}

					bow.updateIndicator(direction);
					bow.Charge ();
					isCharging = true;
					if(currentMovementSpeed > chargingMovementSpeed) {
						currentMovementSpeed -= Time.deltaTime;
					}
				}
				else {
					bow.resetCharge();
					isCharging = false;
					currentMovementSpeed = movementSpeed;
					shotTimer = 0.90f;
				}
			}
			else if (Input.GetMouseButtonUp (0) && shotTimer >= bow.getReloadDelay () && isCharging) {
				this.rigidbody.velocity = new Vector3 (0, 0, 0);
				Vector3 startPos = this.transform.position;
				startPos.y = 1.0f;
				GameObject arrowThing = (GameObject)Instantiate (Resources.Load ("ArrowPrefab"), startPos + extraZ, direction);

				Arrow arrowComponent = arrowThing.AddComponent<Arrow> ();
				
				arrowComponent.owner = this.gameObject;
				bow.Release (direction, arrowThing);
				isCharging = false;
				shotTimer = 0;
				moveTimer = 0;
				if(!isSlowed)
					currentMovementSpeed = movementSpeed;
			}
			else if (Input.GetMouseButtonUp (1) && shotTimer >= bow.getReloadDelay () && isCharging) {
				//Destroy (rangeIndicator);
				this.rigidbody.velocity = new Vector3 (0, 0, 0);
				GameObject arrowThing = (GameObject)Instantiate (Resources.Load ("ArrowPrefab"), this.transform.position + extraZ, direction);

				Arrow arrowComponent = null;

				if(quiver.pullArrow(currentAmmo)) {
					//arrowComponent = arrowThing.AddComponent<currentAmmo>();
					switch(currentAmmo) {
					case ArrowType.FireArrow:
						arrowComponent = arrowThing.AddComponent<FireArrow> ();
						break;
					case ArrowType.IceArrow:
						arrowComponent = arrowThing.AddComponent<IceArrow> ();
						break;
					case ArrowType.ForceArrow:
						arrowComponent = arrowThing.AddComponent<ForceArrow> ();
						break;
					case ArrowType.TreeArrow:
						arrowComponent = arrowThing.AddComponent<TreeArrow> ();
						break;
					case ArrowType.PiercingArrow:
						arrowComponent = arrowThing.AddComponent<PiercingArrow> ();
						break;
					case ArrowType.SplitArrow:
						arrowComponent = arrowThing.AddComponent<SplitArrow> ();
						break;
					default:
						arrowComponent = arrowThing.AddComponent<Arrow>();
						break;
					}
				} else {
					arrowComponent = arrowThing.AddComponent<Arrow> ();
				}

				arrowComponent.owner = this.gameObject;

				bow.Release (direction, arrowThing);
				isCharging = false;
				shotTimer = 0;
				moveTimer = 0;
				if(!isSlowed)
					currentMovementSpeed = movementSpeed;
			}
		} else {
			bow.resetCharge ();
			isCharging = false;
		}

		if(Input.GetKeyDown(KeyCode.Space) &! abilityExecuted &! abilityOnCooldown &! isDead && !movementLocked) {
			if(Vector3.Magnitude(this.gameObject.rigidbody.velocity) != 0) {
				this.gameObject.AddComponent<Accelerate>().Initialize(
															50.0f * Vector3.Normalize(this.gameObject.rigidbody.velocity),
															0.4f, 4.0f, true);
				abilityExecuted = true;
				AudioSource source = AudioHelper.PlayClipAt (dashSound, this.gameObject.transform.position);
				source.rolloffMode = AudioRolloffMode.Linear;
				source.maxDistance = 50.0f;

				dashSystem = (GameObject)Instantiate (Resources.Load("Particles/Air Dash Effect"), this.transform.position - (Vector3.Normalize(this.gameObject.rigidbody.velocity)),
				                                      Quaternion.LookRotation (Vector3.Normalize (-this.gameObject.rigidbody.velocity)));
				dashSystem.transform.parent = gameObject.transform;
				Destroy (dashSystem, 0.4f);
			}
		}
		else if (moveDelay <= 0.0f && !abilityExecuted && !isDead && !movementLocked) {
			this.gameObject.rigidbody.velocity = controller.movementControls();
			if(Vector3.Magnitude(this.gameObject.rigidbody.velocity) > 0) {
				anim.SetFloat ("Speed", 5);
				this.gameObject.transform.rotation = Quaternion.LookRotation (Vector3.Normalize (this.gameObject.rigidbody.velocity));
			} else {
				anim.SetFloat ("Speed", 0);
			}
		}
		else if(moveDelay > 0.0f) {
			moveDelay -= Time.deltaTime;
		}

		ArrowType oldAmmo = currentAmmo;
		if (Input.GetKey (KeyCode.Alpha1)) {
			currentAmmo = quiver.arrows[1].type;
		}
		if (Input.GetKey (KeyCode.Alpha2)) {
			currentAmmo = quiver.arrows[2].type;
		}
		if (Input.GetKey (KeyCode.Alpha3)) {
			currentAmmo = quiver.arrows[3].type;
		}
		if(Input.GetKey (KeyCode.Alpha4)) {
			currentAmmo = quiver.arrows[4].type;
		}
		if(Input.GetKey (KeyCode.Alpha5)) {
			currentAmmo = quiver.arrows[5].type;
		}
		if(Input.GetKey (KeyCode.Alpha6)) {
			currentAmmo = quiver.arrows[6].type;
		}
		
		float scrollWheel = Input.GetAxis (("Mouse ScrollWheel"));
		
		if (Input.GetKeyDown (KeyCode.Q) || scrollWheel < 0.0f) {
			if(((int)currentAmmo - 1) <= 0) {
				currentAmmo = quiver.arrows[6].type;
			}
			else {
				currentAmmo = quiver.arrows[(int)currentAmmo - 1].type;
			}
		}
		
		if (Input.GetKeyDown(KeyCode.E) || scrollWheel > 0.0f) {
			if(((int)currentAmmo + 1) == 7) {
				currentAmmo = quiver.arrows[1].type;
			}
			else {
				currentAmmo = quiver.arrows[(int)currentAmmo + 1].type;
			}
		}
		
		if (oldAmmo != currentAmmo && isCharging) {
			bow.deleteRangeIndicator();
			bool arrowAvailable = false;
			if(quiver.arrowInQuiver(currentAmmo)) {
				arrowAvailable = true;
			} else {
				arrowAvailable = false;
				noAmmoDisplay = true;
				noAmmoTimer = 1.0f;
			}
			bow.initializeRangeIndicator(direction, currentAmmo, arrowAvailable);
		}

		if (abilityCooldown > 0.0f) {
			abilityCooldown -= Time.deltaTime;

			if (abilityCooldown <= 0.0f)
				abilityOnCooldown = false;
		}

		if (noAmmoDisplay) {
			noAmmoTimer -= Time.deltaTime;
			if(noAmmoTimer <= 0.0f) {
				noAmmoDisplay = false;
			}
		}

		shotTimer += Time.deltaTime;
		moveTimer += Time.deltaTime;
		spawnTimer ();
		
		Camera.main.transform.position = new Vector3 (this.gameObject.transform.position.x,
		                                              36.0f, this.gameObject.transform.position.z + 10);
	}

	public override void addAmmo(ArrowType type) {
		quiver.addArrow (type, 2);
	}
}
