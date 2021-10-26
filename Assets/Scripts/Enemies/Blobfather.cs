using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blobfather : Blob {
	[SerializeField]
	float shot_amount = 3;
	[SerializeField]
	float shot_spread = 5;

	[SerializeField]
	GameObject shot = null;
	[SerializeField]
	Transform mouth = null;

	private void Puke() {
		Vector3 direction = -mouth.position;
		direction.Normalize();
		mouth.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

		for (int i = 0; i < shot_amount; i++) {
			Instantiate(shot, mouth.position + new Vector3(Random.value * 0.1f, Random.value * 0.1f, 0), mouth.rotation);
			mouth.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (Random.value * 2 * shot_spread - shot_spread));
		}
	}
}
