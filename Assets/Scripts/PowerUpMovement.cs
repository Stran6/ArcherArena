using UnityEngine;
using System.Collections;

public class PowerUpMovement : MonoBehaviour {
	private Vector3 spawnPosition;
	private float angle = 0;
	
	void Start () {
		spawnPosition = this.transform.position;
		Vector3 position = spawnPosition;
		position.x += -0.3f;
	}
	
	void Update() {
		angle += Time.deltaTime;

		Vector3 translation = spawnPosition;
		translation.y += Mathf.Sin (angle)/2;
		this.transform.Rotate (new Vector3 (0, 1, 0), 90 * Time.deltaTime);
		this.transform.position = translation;
	}
}