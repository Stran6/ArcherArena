using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
	private int killsToWin;
	public int leaderIndex = 0;
	public int leadScore = 0;

	public bool tutorialMode = false;

	public class Score {
		public Character character;
		public string characterName;
		public Color characterColor;
		public int score;
		public int rank;
	}

	public ArrayList characterScores;

	int numCharacters;

	public void initialize (int killMode) {
		characterScores = new ArrayList ();
		numCharacters = 0;
		killsToWin = killMode;
	}

	private bool showScores = true;
	private bool gameOver = false;
	public void OnGUI(){
		int offset = 40;
		GUIStyle style = new GUIStyle ();
		style.fontSize = 15;
		style.normal.textColor = new Color (1, 1, 1);
		
		float posMod = 0.86f;
		float vPosition = Screen.height * 0.07f;
		float hSizeMod = 0.12f;
		float vSizeMod = 0.9f;
		if (showScores || gameOver) {
			if(!tutorialMode)
			GUI.Label(new Rect(Screen.width * 0.895f, Screen.height * 0.03f, 100, 50), "Goal: " + killsToWin + " kills", style);
			GUI.Box (new Rect (Screen.width * posMod, vPosition, Screen.width * hSizeMod, Screen.height * vSizeMod), "");
		
			foreach (Score s in characterScores) {
				style.normal.textColor = s.characterColor;
			
				GUI.Label (new Rect (Screen.width * (posMod + 0.0135f), vPosition + (offset * s.rank), 200, 50),
			    (s.rank + 1) + ")    " + s.characterName + ": " + s.score, style);
			}
		}
	}

	public void Update() {
		if (Input.GetKeyDown (KeyCode.Tab)) {
			showScores = (showScores) ? false : true;
		}
	}

	public void characterAdded(Character character, Color color) {
		numCharacters++;
		Score newScore = new Score ();
		newScore.character = character;
		newScore.characterName = "Player " + numCharacters;
		newScore.characterColor = color;
		newScore.score = 0;
		newScore.rank = characterScores.Count;
		characterScores.Add (newScore);
		character.initialize (numCharacters, this.gameObject);
	}

	public int getTargetRankIndex(int currentRank, int currentScore, bool rankUp) {
		int resultIndex = -1;
		int goalRank = currentRank;

		int tempLeadIndex = 0;

		for (int i = 0; i < characterScores.Count; i++) {
			if(((Score)characterScores [i]).rank == 0) {
				tempLeadIndex = i;
			}

			if(rankUp) {
				if(((Score)characterScores[i]).rank < goalRank) {
					if(((Score)characterScores[i]).score < currentScore) {
						goalRank = ((Score)characterScores[i]).rank;
						resultIndex = i;
					}
				}
			} else {
				if(((Score)characterScores[i]).rank > goalRank) {
					if(((Score)characterScores[i]).score > currentScore) {
						goalRank = ((Score)characterScores[i]).rank;
						resultIndex = i;
					}
				}
			}
		}
		if (resultIndex != tempLeadIndex) {
			leaderIndex = tempLeadIndex;
			leadScore = ((Score)characterScores [leaderIndex]).score;
		}

		return resultIndex;
	}

	public void moveUp(int killerIndex) {
		int killerRank = ((Score)characterScores [killerIndex]).rank;
		int killerScore = ((Score)characterScores[killerIndex]).score;

		int targetRankIndex = getTargetRankIndex (killerRank, killerScore, true);
		int goalRank = (targetRankIndex >= 0) ? ((Score)characterScores[targetRankIndex]).rank : -1;

		if (goalRank == 0) {
			leaderIndex = killerIndex;
			leadScore = killerScore;
		}

		if (goalRank >= 0) {
			for (int i = 0; i < characterScores.Count; i++) {
				if (goalRank <= ((Score)characterScores [i]).rank && ((Score)characterScores [i]).rank < killerRank) {
					((Score)characterScores [i]).rank++;
				}
			}
			((Score)characterScores [killerIndex]).rank = goalRank;
		}
	}

	public void moveDown(int killerIndex) {
		int killerRank = ((Score)characterScores [killerIndex]).rank;
		int killerScore = ((Score)characterScores[killerIndex]).score;
		
		int targetRankIndex = getTargetRankIndex (killerRank, killerScore, false);
		int goalRank = (targetRankIndex >= 0) ? ((Score)characterScores[targetRankIndex]).rank : -1;
		
		if (goalRank == 0) {
			leaderIndex = killerIndex;
			leadScore = killerScore;
		}
		
		if (goalRank >= 0) {
			for (int i = 0; i < characterScores.Count; i++) {
				if (goalRank >= ((Score)characterScores [i]).rank && ((Score)characterScores [i]).rank > killerRank) {
					((Score)characterScores [i]).rank--;
				}
			}
			((Score)characterScores [killerIndex]).rank = goalRank;
		}
	}

	public void changeScore(int killerID, int victimID) {
		for (int i = 0; i < characterScores.Count; i++) {
			if(((Score)characterScores [i]).score > ((Score)characterScores [leaderIndex]).score) {
				leaderIndex = i;
				leadScore = ((Score)characterScores [leaderIndex]).score;
			}
			if( ((Score)characterScores[i]).character.ID == killerID) {
				if(killerID == victimID) {
					((Score)characterScores[i]).score--;
					if(((Score)characterScores[i]).rank != characterScores.Count-1) {
						moveDown (i);
					}
				} else {
					((Score)characterScores[i]).score++;
					if(((Score)characterScores[i]).rank > 0) {
						moveUp (i);
					}
				}

				if(((Score)characterScores[i]).score >= killsToWin) {
					setGameOver(killerID);
				}
				i = characterScores.Count;
			}
		}
	}

	public void setGameOver(int ID) {
		foreach (Score s in characterScores) {
			s.character.enabled = false;
			s.character.gameObject.rigidbody.velocity = new Vector3();
		}

		showScores = true;
		gameOver = true;

		this.gameObject.GetComponent<GameManager> ().SendMessage ("setWinner", ID);
	}
}
