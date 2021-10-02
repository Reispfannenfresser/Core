using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgoundController : MonoBehaviour {
	[SerializeField]
	float speed = 1f;
	[SerializeField]
	int tp_distance = 4;
	float current_distance = 0;

	void Update() {
		float change = Time.deltaTime * speed;
		transform.position += Vector3.down * change;
		current_distance += change;
		if(current_distance > tp_distance) {
			current_distance -= tp_distance;
			transform.position -= Vector3.down * tp_distance;
		}
	}
}
