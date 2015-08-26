using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour {

	ScoreManager scoreManager;
	int currentLevel;
	TutorialPlayer player;
	List<TutorialAI> enemies;
	string objective;
	Vector3 exitPosition;
	bool started, regularShotEnemiesDead, showObjectives;
	void Start () {
		scoreManager = this.gameObject.AddComponent<ScoreManager> ();
		scoreManager.tutorialMode = true;
		scoreManager.initialize (9999);

		showObjectives = true;
		started = false;
		regularShotEnemiesDead = false;
		enemies = new List<TutorialAI> ();
		
		scoreManager.characterAdded(makeTutorialPlayer(1, "Player", "Materials/Player1Color", "TutorialPlayer", new Vector3 (0.0f, 0.1f, 30.0f), new Color (0, 1, 0)), new Color(0, 1, 0));
//		
//		//Shooting Enemies
//		character = makeTutorialCharacter (9, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (15.0f, 0.1f, -35.0f), new Color (1, 0, 0));
//		((TutorialAI)character).setShooting (GameObject.Find ("Target1") 0.1f);
//		character = makeTutorialCharacter (10, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (15.0f, 0.1f, -30.0f), new Color (1, 0, 0));
//		((TutorialAI)character).setShooting (GameObject.Find ("Target2"), 1.5f);
//		character = makeTutorialCharacter (11, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (15.0f, 0.1f, -25.0f), new Color (1, 0, 0));
//		((TutorialAI)character).setShooting (GameObject.Find ("Target3"), 3.0f);
//		
//		character = makeTutorialCharacter (12, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-35.0f, 0.1f, -35.0f), new Color (1, 0, 0));
//		((TutorialAI)character).setShooting (new Vector3 (-35.0f, 0.1f, -15.0f), 2.5f);
//		character = makeTutorialCharacter (13, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-35.0f, 0.1f, -15.0f), new Color (1, 0, 0));
//		((TutorialAI)character).setShooting (new Vector3 (-35.0f, 0.1f, -35.0f), 2.5f);
	}
	
	public Character makeTutorialPlayer(int ID, string characterModelLocation, string materialLocation, string className, Vector3 spawnLocation, Color color) {
		GameObject obj = (GameObject)Instantiate (Resources.Load(characterModelLocation), spawnLocation, new Quaternion ());
		
		obj.renderer.material = (Material)Resources.Load (materialLocation);
		
		player = (TutorialPlayer)obj.AddComponent(className);
		player.tutorialMode = true;
		player.initialize (ID, this.gameObject);
		player.tutorialRespawnLocation = spawnLocation;

		return player;
	}

	public TutorialAI makeTutorialAI(int ID, string characterModelLocation, string materialLocation, string className, Vector3 spawnLocation, Color color) {
		GameObject obj = (GameObject)Instantiate (Resources.Load(characterModelLocation), spawnLocation, new Quaternion ());
		
		obj.renderer.material = (Material)Resources.Load (materialLocation);
		TutorialAI tempAI = (TutorialAI)obj.AddComponent(className);

		tempAI = (TutorialAI)obj.AddComponent(className);
		tempAI.tutorialMode = true;
		tempAI.initialize (ID, this.gameObject);
		tempAI.tutorialRespawnLocation = spawnLocation;

		enemies.Add (tempAI);
		return tempAI;
	}


	
	float displayTimer = 0;
	
	void Update () {
		currentLevel = Application.loadedLevel;
		if(!started) {
			if(Application.loadedLevelName == "Movement") {
				player.canMove = true;
				player.canDash = true;
				objective = "Reach the Blue area";
				exitPosition = new Vector3(0, 0.3f, 5);
			}
			else if(Application.loadedLevelName == "RegularShot") {
				player.canMove = true;
				player.canDash = true;
				player.canShoot = true;
				objective = "Kill all Enemies(the RED circles) then reach the Blue Plate";
				makeTutorialAI (2, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 10.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (3, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (4, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-10.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				exitPosition = new Vector3(0, 0.3f, 10);
			}
			else if(Application.loadedLevelName == "FireArrow") {
				player.canMove = true;
				player.canDash = true;
				player.canSpecialShot = true;
				objective = "Kill all Enemies(the RED circles) then reach the Blue area";
				//Top Left
				makeTutorialAI ( 2, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  7.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI ( 3, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 10.0f, -0.1f, 8.0f), new Color (1, 0, 0));
				makeTutorialAI ( 4, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 10.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI ( 5, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 10.0f, -0.1f, 2.0f), new Color (1, 0, 0));
				makeTutorialAI ( 6, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 13.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				//Center
				makeTutorialAI ( 7, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  3.0f, -0.1f,  5.0f), new Color (1, 0, 0));
				makeTutorialAI ( 8, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f,  2.0f), new Color (1, 0, 0));
				makeTutorialAI ( 9, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f,  5.0f), new Color (1, 0, 0));
				makeTutorialAI (10, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f,  8.0f), new Color (1, 0, 0));
				makeTutorialAI (11, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -3.0f, -0.1f,  5.0f), new Color (1, 0, 0));
				//Top Right
				makeTutorialAI (12, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -7.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (13, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-10.0f, -0.1f, 8.0f), new Color (1, 0, 0));
				makeTutorialAI (14, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-10.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (15, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-10.0f, -0.1f, 2.0f), new Color (1, 0, 0));
				makeTutorialAI (16, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-13.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				exitPosition = new Vector3(0, 0.3f, 5);
			}
			else if(Application.loadedLevelName == "IceArrow") {
				player.canMove = true;
				player.canDash = true;
				player.canShoot = true;
				player.canSpecialShot = true;
				TutorialAI character;
				objective = "Kill all Enemies(the RED circles) then reach the Blue area";
				character = makeTutorialAI (5, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (15.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				character.patrol = true;
				character = makeTutorialAI (6, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (5.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				character.patrol = true;
				character = makeTutorialAI (7, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-5.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				character.patrol = true;
				character = makeTutorialAI (8, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (15.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				character.patrol = true;
				character = makeTutorialAI (9, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (5.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				character.patrol = true;
				character = makeTutorialAI (10, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-5.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				character.patrol = true;
				exitPosition = new Vector3(0, 0.3f, 5);
			}
			else if(Application.loadedLevelName == "ForceArrow") {
				player.canMove = true;
				player.canDash = true;
				player.canSpecialShot = true;
				objective = "Kill all Enemies(the RED circles) then reach the Blue area";
				makeTutorialAI ( 2, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  14.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI ( 3, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  14.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI ( 4, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  14.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				makeTutorialAI ( 5, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f,  15.0f), new Color (1, 0, 0));
				makeTutorialAI ( 6, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  5.0f, -0.1f,  15.0f), new Color (1, 0, 0));
				makeTutorialAI ( 7, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 10.0f, -0.1f,  15.0f), new Color (1, 0, 0));
				makeTutorialAI ( 8, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -6.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI ( 9, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -6.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (10, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -6.0f, -0.1f, 15.0f), new Color (1, 0, 0));

				makeTutorialAI (11, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 0.0f, -0.1f, -15.0f), new Color (1, 0, 0));
				makeTutorialAI (12, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 5.0f, -0.1f, -15.0f), new Color (1, 0, 0));
				makeTutorialAI (13, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-5.0f, -0.1f, -15.0f), new Color (1, 0, 0));
				makeTutorialAI (14, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 0.0f, -0.1f, -10.0f), new Color (1, 0, 0));
				makeTutorialAI (15, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -5.0f, -0.1f, -10.0f), new Color (1, 0, 0));
				makeTutorialAI (16, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 5.0f, -0.1f, -10.0f), new Color (1, 0, 0));
				makeTutorialAI (17, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 0.0f, -0.1f, -5.0f), new Color (1, 0, 0));
				makeTutorialAI (18, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -5.0f, -0.1f, -5.0f), new Color (1, 0, 0));
				makeTutorialAI (19, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 5.0f, -0.1f, -5.0f), new Color (1, 0, 0));
				
				makeTutorialAI (20, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -14.0f, -0.1f, 20.0f), new Color (1, 0, 0));
				makeTutorialAI (21, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -14.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				makeTutorialAI (22, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -14.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (23, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -14.0f, -0.1f, 5.0f), new Color (1, 0, 0));

				exitPosition = new Vector3(0, 0.3f, -10);
			}
			else if(Application.loadedLevelName == "TreeArrow") {
				player.canMove = true;
				player.canDash = true;
				player.canSpecialShot = true;
				TutorialAI character;
				objective = "Reach the Blue area";
				character = makeTutorialAI ( 2, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 10.0f, -0.1f, -10.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 3);
				character = makeTutorialAI ( 3, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f,  5.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 3);
				character = makeTutorialAI ( 4, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-10.0f, -0.1f, -10.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 3);

				character = makeTutorialAI ( 5, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -5.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 1);
				character = makeTutorialAI ( 6, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  5.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 1);
				character = makeTutorialAI ( 7, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 0.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 1);

				character = makeTutorialAI ( 8, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 10.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 2);
				character = makeTutorialAI ( 9, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 2);
				character = makeTutorialAI (10, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-10.0f, -0.1f, 15.0f), new Color (1, 0, 0));
				character.setShooting(player.gameObject, 2);
				exitPosition = new Vector3(0, 0.3f, -10);
			}
			else if(Application.loadedLevelName == "PiercingArrow") {
				player.canMove = true;
				player.canDash = true;
				player.canSpecialShot = true;
				objective = "Kill all Enemies(the RED circles) then reach the Blue area";
				//Center
				makeTutorialAI ( 2, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  5.0f, -0.1f,  5.0f), new Color (1, 0, 0));
				makeTutorialAI ( 3, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (  0.0f, -0.1f,  5.0f), new Color (1, 0, 0));
				makeTutorialAI ( 4, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( -5.0f, -0.1f,  5.0f), new Color (1, 0, 0));
				//TopLeft
				makeTutorialAI ( 5, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 14.0f, -0.1f,  -15.0f), new Color (1, 0, 0));
				makeTutorialAI ( 6, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 14.0f, -0.1f,  -10.0f), new Color (1, 0, 0));
				makeTutorialAI ( 7, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 ( 14.0f, -0.1f,  -5.0f), new Color (1, 0, 0));
				//TopRight
				makeTutorialAI ( 8, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-14.0f, -0.1f,  -15.0f), new Color (1, 0, 0));
				makeTutorialAI ( 9, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-14.0f, -0.1f,  -10.0f), new Color (1, 0, 0));
				makeTutorialAI (10, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-14.0f, -0.1f,  -5.0f), new Color (1, 0, 0));
				exitPosition = new Vector3(0, 0.3f, -10);
			}
			else if(Application.loadedLevelName == "SplitArrow") {
				player.canMove = true;
				player.canDash = true;
				player.canSpecialShot = true;
				objective = "Kill all Enemies(the RED circles) then reach the Blue area";
				makeTutorialAI ( 2, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (12.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI ( 3, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (9.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI ( 4, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (6.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI ( 5, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (3.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI ( 6, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (0.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI ( 7, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-3.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI ( 8, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-6.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI ( 9, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-9.0f, -0.1f, 0.0f), new Color (1, 0, 0));
				makeTutorialAI (10, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-12.0f, -0.1f, 0.0f), new Color (1, 0, 0));

				makeTutorialAI (11, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (12.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (12, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (9.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (13, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (6.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (14, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (3.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (15, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (0.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (16, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-3.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (17, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-6.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (18, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-9.0f, -0.1f, 5.0f), new Color (1, 0, 0));
				makeTutorialAI (19, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-12.0f, -0.1f, 5.0f), new Color (1, 0, 0));

				makeTutorialAI (20, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (12.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (21, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (9.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (22, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (6.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (23, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (3.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (24, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (0.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (25, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-3.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (26, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-6.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (27, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-9.0f, -0.1f, 10.0f), new Color (1, 0, 0));
				makeTutorialAI (28, "Player", "Materials/EnemyColor", "TutorialAI", new Vector3 (-12.0f, -0.1f, 10.0f), new Color (1, 0, 0));

				exitPosition = new Vector3(0, 0.3f, 5);
			}
			started = true;
		}
		if (getRemainingEnemies() <= 0 || Application.loadedLevelName == "TreeArrow") {
			this.gameObject.transform.position = exitPosition;
		}
		displayTimer += Time.deltaTime;

		if (Input.GetKeyDown (KeyCode.Tab)) {
			showObjectives = (showObjectives) ? false : true;
		}
	}

	public int getRemainingEnemies() {
		int count = enemies.Count;
		for (int i = 0; i < enemies.Count; i++) {
			if(enemies[i] == null)
				count --;
		}

		return count;
	}
	
	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	public void OnGUI() {
		GUIStyle style = new GUIStyle ();
		
		style.fontSize = 30;
		style.normal.textColor = new Color (1, 1, 1);
		
		if (GUI.Button (new Rect (Screen.width * 0.01f, Screen.height * 0.80f, 160, 25), "Skip Tutorials")) {
				Application.LoadLevel("archerArena");
		}
		
		if (GUI.Button (new Rect (Screen.width * 0.01f, Screen.height * 0.85f, 160, 25), "Reset Tutorial")) {
			Application.LoadLevel(Application.loadedLevelName);
		}

		if (GUI.Button (new Rect (Screen.width * 0.01f, Screen.height * 0.90f, 160, 25), "Previous")) {
			if(Application.loadedLevel - 1 >= 0) {
				Application.LoadLevel(Application.loadedLevel - 1);
			}
		}

		if (GUI.Button (new Rect (Screen.width * 0.01f, Screen.height * 0.95f, 160, 25), "Next")) {
			if(Application.loadedLevel + 1 <= 7) {
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
		
		GUIStyle controlsStyle = new GUIStyle (GUI.skin.box);
		controlsStyle.fontSize = 18;
		controlsStyle.normal.background = MakeTex (50, 50, new Color (0, 0, 0));

		float objectiveWidth = 0, objectiveHeight = 0;
		float width = 500;
		float height = 110;
		float verticalPosition = .6f;
		if (showObjectives) {
			if (Application.loadedLevelName == "Movement") {
				height = 80;
				GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
 						"Use WASD to move." +
						"\n\nUse Space Bar to Dash in the direction you are moving", controlsStyle);
				objectiveWidth = 200;
				objectiveHeight = 30;
			} else if (Application.loadedLevelName == "RegularShot") {
					height = 80;
					GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
         "Left-click to Shoot, hold to charge." +
							"\n\nCharging slows you down.", controlsStyle);
					objectiveWidth = 500;
					objectiveHeight = 30;
			} else if (Application.loadedLevelName == "FireArrow") {
					height = 120;
					GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
         "Press 1 to select the Fire Arrow." +
							"\n\nRight-click to Shoot, hold to charge." +
							"\n\nTry shooting crates!", controlsStyle);
					objectiveWidth = 500;
					objectiveHeight = 30;
			} else if (Application.loadedLevelName == "IceArrow") {
					height = 80;
					GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
         "Press 2 to select the Ice Arrow." +
							"\n\nRight-click to Shoot, hold to charge.", controlsStyle);
					objectiveWidth = 500;
					objectiveHeight = 30;
			} else if (Application.loadedLevelName == "ForceArrow") {
					height = 120;
					GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
         "Press 3 to select the Force Arrow." +
							"\n\nRight-click to Shoot, hold to charge." +
							"\n\nTry shooting walls, trees, & crates!", controlsStyle);
					objectiveWidth = 500;
					objectiveHeight = 30;
			} else if (Application.loadedLevelName == "TreeArrow") {
					height = 80;
					GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
         "Press 4 to select the Tree Arrow." +
							"\n\nRight-click to Shoot, hold to charge.", controlsStyle);
					objectiveWidth = 400;
					objectiveHeight = 30;
			} else if (Application.loadedLevelName == "PiercingArrow") {
					height = 120;
					GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
         "Press 5 to select the Piercing Arrow." +
							"\n\nRight-click to Shoot, hold to charge." +
							"\n\nTry shooting crates!", controlsStyle);
					objectiveWidth = 500;
					objectiveHeight = 30;
			} else if (Application.loadedLevelName == "SplitArrow") {
					height = 80;
					GUI.Box (new Rect (Screen.width / 2 - width / 2, Screen.height * verticalPosition, width, height),
         "Press 6 to select the Split Arrow." +
							"\n\nRight-click to Shoot, hold to charge.", controlsStyle);
					objectiveWidth = 500;
					objectiveHeight = 30;
			}
			GUI.Box (new Rect (Screen.width / 2 - objectiveWidth / 2, Screen.height * 0.25f, objectiveWidth, objectiveHeight), objective, controlsStyle);
			GUI.Box (new Rect (Screen.width / 2 - 400 / 2, Screen.height * 0.95f, 400, 30), "Press Tab to Hide Instructions & Objectives", controlsStyle);
		}
		else {
			GUI.Box (new Rect (Screen.width / 2 - 400 / 2, Screen.height * 0.95f, 400, 30), "Press Tab to Show Instructions & Objectives", controlsStyle);
		}
//		         "\n\nWASD =\nMovement" +
//		         "\n\nSpacebar + \nMovement Key =\nDash" +
//		         "\n\nMouse Left-Click =\n Shoot Normal Arrow" +
//		         "\n\nMouse Right-Click =\n Shoot Selected\nSpecial Arrow" +
//		         "\n\nNumbers 1-6 =\nSelect special arrow" +
//		         "\n\nTab - Show/Hide Scoreboard" +
//		         "\n\nHold Mouse Button to charge" +
//		         "\n\nDestroy crates to collect\nmore special arrows", controlsStyle);
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "Movement") {
			Application.LoadLevel("RegularShot");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "RegularShot" && getRemainingEnemies() <= 0) {
			Application.LoadLevel("FireArrow");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "FireArrow" && getRemainingEnemies() <= 0) {
			Application.LoadLevel("IceArrow");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "IceArrow" && getRemainingEnemies() <= 0) {
			Application.LoadLevel("ForceArrow");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "ForceArrow" && getRemainingEnemies() <= 0) {
			Application.LoadLevel("TreeArrow");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "TreeArrow") {
			Application.LoadLevel("PiercingArrow");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "PiercingArrow" && getRemainingEnemies() <= 0) {
			Application.LoadLevel("SplitArrow");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "SplitArrow" && getRemainingEnemies() <= 0) {
			Application.LoadLevel("archerArena");
		}
		if (collision.gameObject.GetComponent<TutorialPlayer> () && Application.loadedLevelName == "tutorial" && getRemainingEnemies() <= 0) {
			Application.LoadLevel("archerArena");
		}
	}
}