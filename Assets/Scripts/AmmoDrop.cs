using UnityEngine;
using System.Collections;

public class AmmoDrop : MonoBehaviour {

	private const int numberOfArrowTypes = 6;
	private ArrowType[] ammoType = new ArrowType[numberOfArrowTypes];
	private int num;
	private bool showMessage;
	public bool tutorialMode;
	//public Material[] mats;
	AudioClip pickUpSound;

	void Start() {
		ammoType [0] = ArrowType.FireArrow;
		ammoType [1] = ArrowType.IceArrow;
		ammoType [2] = ArrowType.ForceArrow;
		ammoType [3] = ArrowType.SplitArrow;
		ammoType [4] = ArrowType.PiercingArrow;
		ammoType [5] = ArrowType.TreeArrow;
		num = Random.Range (0, numberOfArrowTypes);
		showMessage = false;
		//this.gameObject.renderer.material = mats [num];
	}

	private string displayText = "";
	private void generateGUILabel(ArrowType type) {
		displayText = "+2 All SPECIAL Arrows";
	}

	private float yPosition = (Screen.height/2) + 5;
	void OnGUI() {
		if (showMessage) {
			GUI.Label(new Rect(Screen.width/2 - 35, yPosition, 150, 50), displayText);
			yPosition -= 10 * Time.deltaTime;
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Player> () || collision.gameObject.GetComponent<TutorialPlayer> ()) {
			showMessage = true;
		}
		generateGUILabel (ammoType [num]);

		for (int i = 0; i < ammoType.Length; i++) {
			collision.gameObject.SendMessage ("addAmmo", ammoType [i]);
		}

		pickUpSound = (AudioClip)Resources.Load ("Audio/enchant");
		AudioSource source = AudioHelper.PlayClipAt (pickUpSound, this.gameObject.transform.position);
		source.rolloffMode = AudioRolloffMode.Linear;
		source.maxDistance = 5.0f;
		Destroy (this.gameObject.collider);
		Destroy (this.gameObject.rigidbody);
		Destroy (this.gameObject.renderer);
		Destroy (this.gameObject, 0.8f);
	}
}
