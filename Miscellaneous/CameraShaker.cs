using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {

	/* Controls the camera shake effect */
	[SerializeField] private float shakeMultiplier; 
	private Vector3 shake; //the new position of the camera caused by shake

	// Shakes the camera in a set direction for a set magnitude and duration 
	public IEnumerator CameraShake(Vector3 direction, float magnitude, float duration){
		Vector3 startPosition = transform.localPosition;
		float timeElapsed = 0f;

		while (timeElapsed < duration) {

			shake = direction * Random.Range (0f, magnitude * shakeMultiplier);

			transform.localPosition = new Vector3 (shake.x, shake.z, 0f);

			timeElapsed += Time.deltaTime;

			yield return null;
		}

		transform.localPosition = startPosition;
	}
}
