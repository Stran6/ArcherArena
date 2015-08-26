using UnityEngine;
using System.Collections;

public class Dirt : MonoBehaviour {

	// Use this for initialization
	float timer = 0.5f;
	GameObject dirt;
	public void Initialize (Vector3 position) {
		dirt = (GameObject)Instantiate (Resources.Load ("Particles/Dirt2"), position, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		if(timer <= 0) {
			Destroy (dirt);
			Destroy(this);
		}
		timer -= Time.deltaTime;
	}

	void OnDestroy() {
		Destroy (dirt, timer);
	}
}
