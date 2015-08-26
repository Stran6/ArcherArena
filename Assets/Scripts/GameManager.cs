using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	ScoreManager scoreManager;
	public SpawnManager spawnManager;
	bool gameOver;
	int winner;

	int killGoal = 15;
	int players = 20;
	void Start () {
		scoreManager = this.gameObject.AddComponent<ScoreManager> ();
		scoreManager.initialize (killGoal);

		spawnManager = this.gameObject.AddComponent<SpawnManager> ();
		spawnManager.initialize (players);

		gameOver = false;
	}

	float displayTimer = 0;
	void Update () {
		displayTimer += Time.deltaTime;
	}

	public void setWinner(int winnerID) {
		winner = winnerID;
		gameOver = true;
	}

	public void OnGUI() {
		GUIStyle style = new GUIStyle ();

		style.fontSize = 30;
		style.normal.textColor = new Color (1, 1, 1);

		if (displayTimer < 3.0f) {
			string goal = "Get " + killGoal + " kills to win.";
			GUI.Label (new Rect ((Screen.width / 2) - 120, Screen.height / 4, 150, 70), goal, style);
		}

		if (gameOver) {
			string winnerText = "Player " + winner + " has won.";
			GUI.Label (new Rect ((Screen.width / 2) - 120, Screen.height / 4, 150, 70), winnerText, style);

			GUIStyle replayStyle = new GUIStyle(GUI.skin.button);
			replayStyle.fontSize = 24;

			if (GUI.Button (new Rect ((Screen.width / 2) - 100, (Screen.height / 2) - 30, 200, 60), "Play Again", replayStyle)) {
				Application.LoadLevel("archerArena");
			}
		}
	}
}