using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	private float movementSpeed;
	private Vector3 currentMovementSpeed;
	void Start () {
		currentMovementSpeed = Vector3.zero;
		movementSpeed = 0;
	}

	public void Initialize(float movementSpeed) {
		this.movementSpeed = movementSpeed;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void updateMovementSpeed(float newMovementSpeed) {
		movementSpeed = newMovementSpeed;
	}

	public Vector3 movementControls(){
		currentMovementSpeed = Vector3.zero;
		if(Input.GetKey (KeyCode.W)) {
			currentMovementSpeed -= new Vector3(0.0f, 0.0f, movementSpeed);
		}
		
		if(Input.GetKey (KeyCode.S)) {
			currentMovementSpeed += new Vector3(0.0f, 0.0f,movementSpeed);
		}
		
		if(Input.GetKey (KeyCode.A)) {
			currentMovementSpeed += new Vector3(movementSpeed, 0.0f, 0.0f);
		}
		
		if(Input.GetKey (KeyCode.D)) {
			currentMovementSpeed -= new Vector3(movementSpeed, 0.0f, 0.0f);
		}
		return currentMovementSpeed;
	}
}
