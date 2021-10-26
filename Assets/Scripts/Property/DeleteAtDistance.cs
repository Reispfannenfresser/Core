using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAtDistance : MonoBehaviour {
	[SerializeField]
	int max_distance = 60;
	[SerializeField]
	Vector3 origin = Vector3.zero;

	private void FixedUpdate() {
		if ((origin - transform.position).magnitude > max_distance) {
			Destroy(gameObject);
		}
	}
}
