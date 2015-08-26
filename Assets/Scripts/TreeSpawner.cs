using UnityEngine;
using System.Collections;

public class Trees : MonoBehaviour {
	public Material mat;
	// Use this for initialization
	void Start () {
		foreach(Transform child in transform){
			child.renderer.material = mat;
//			child.gameObject.AddComponent<Walls>();
			child.gameObject.AddComponent<Tree>();
		}

//		for(int i = 0; i < transforms.Length; i++){
//			transforms[i].GetChild(i).renderer.material = mat;
//		}
	}
}
