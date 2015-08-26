using UnityEngine;
using System.Collections;

public class CreateObstacles : MonoBehaviour {
	public GameObject obstacles;
	// Use this for initialization
	void Start () {
		//min = -499.5, max = 495.5
		for(float leftRight = -53.5f; leftRight < 52.5f; leftRight+=20) {
			for(float upDown = -53.5f; upDown < 52.5f; upDown+=20) {
				for(int i = Random.Range(0, 2); i < 1; i++) {

					Instantiate (obstacles, new Vector3(Random.Range(leftRight, leftRight + 4), 1.0f, Random.Range(upDown, upDown + 4)), Quaternion.identity);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
