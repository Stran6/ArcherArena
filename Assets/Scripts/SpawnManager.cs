using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	public ArrayList spawnPoints;

	public ArrayList characterList;
	int numCharactersInList = 0;

	public void initialize (int numCharacters) {
		spawnPoints = new ArrayList ();

		for (int i = 1; i <= 40; i++) {
			GameObject tempObject = GameObject.Find("SpawnPoint" + i);

			if(tempObject)
				spawnPoints.Add(tempObject);
		}

		ArrayList initialSpawnPoints = new ArrayList();
		initialSpawnPoints.AddRange (spawnPoints);

		characterList = new ArrayList ();

		int randomSpawnIndex = Random.Range (0, initialSpawnPoints.Count - 1);
		makeCharacter("Player", "Materials/Player1Color", "Player", ((GameObject)initialSpawnPoints[randomSpawnIndex]).transform.position, new Color (0, 1, 0));
		initialSpawnPoints.RemoveAt (randomSpawnIndex);

		for(int j = 1; j < numCharacters; j++) {
			randomSpawnIndex = Random.Range (0, initialSpawnPoints.Count - 1);
			makeCharacter ("Player", "Materials/EnemyColor", "MediumAI", ((GameObject)initialSpawnPoints[randomSpawnIndex]).transform.position, new Color (1, 0, 0));
			initialSpawnPoints.RemoveAt (randomSpawnIndex);
		}
	}

	public void makeCharacter(string characterModelLocation, string materialLocation, string className, Vector3 spawnLocation, Color color) {
		GameObject character = (GameObject)Instantiate (Resources.Load(characterModelLocation), spawnLocation, new Quaternion ());

		character.renderer.material = (Material)Resources.Load (materialLocation);

		Character characterComponent = (Character)character.AddComponent(className);
		this.gameObject.GetComponent<ScoreManager> ().characterAdded (characterComponent, color);

		characterList.Add (character);
		numCharactersInList++;
	}

	public void Update () {
	
	}

	public void respawn(GameObject character) {
		character.transform.position = ((GameObject)spawnPoints [Random.Range (0, 39)]).transform.position;
	}
}
