using UnityEngine;
using System.Collections;

public class ObstacleCollision : MonoBehaviour {
	private int hitCount = 2;
	private bool isBurning = false;
	AudioClip arrowCollisionSound;
	public bool tutorialMode;

	void Start() {
		if(Application.loadedLevelName != "archerArena") {
			tutorialMode = true;
		}
		else {
			tutorialMode = false;
		}
		//this.renderer.material.color = new Color(1, 1, 1);
	}
	
	void Update () {
		if (hitCount <= 0) {
			Destroy (this.gameObject);
		}
		if(isBurning) {
			hitCount -= 1;
		}
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<FireEffect> ()) {
			isBurning = true;
		}
	}

	void OnDestroy() {
		if (hitCount <= 0) {
			GameObject drop = (GameObject)Instantiate (Resources.Load ("AmmoDrop"), this.transform.position, Quaternion.identity);

			drop.GetComponent<AmmoDrop>().tutorialMode = tutorialMode;
		}
	}

	void onHit(int[] hitDetail) {
		//float newColor = 1.0f;

		if (hitCount == 3) {
			this.renderer.material.mainTexture = (Texture)Resources.Load("crate2_Diffuse");
		}
		else if (hitCount == 2){
			this.renderer.material.mainTexture = (Texture)Resources.Load("crate3_Diffuse");
		}

		//this.renderer.material.color = new Color(newColor, newColor, newColor);
		hitCount--;
		arrowCollisionSound = (AudioClip)Resources.Load ("Audio/shieldBlock");
		AudioSource source = AudioHelper.PlayClipAt (arrowCollisionSound, this.gameObject.transform.position);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 30.0f;
	}
}
