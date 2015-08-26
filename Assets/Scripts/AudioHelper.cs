using UnityEngine;
using System.Collections;

public static class AudioHelper {

	public static AudioSource PlayClipAt(AudioClip clip, Vector3 position, float duration = 0.0f) {
		GameObject tempObject = new GameObject("TempAudioObject");
		tempObject.transform.position = position;
		AudioSource source = tempObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.Play();

		if (duration > 0.0f) {
			Object.Destroy (tempObject, duration);
		} else {
			Object.Destroy (tempObject, clip.length);
		}

		return source;
	}
}
