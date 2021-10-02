using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : Enemy {
	private bool back = false;
	private Vector3 start_pos;
	private float min_height;

	private float x_offset = 0;

	protected override void OnSpawned() {
		start_pos = transform.position;
		min_height = 5 + (Random.value - 0.5f) * 3;
		x_offset = (Random.value - 0.5f) * 2 * Mathf.PI;
		back = Random.value > 0.5f;
		Debug.Log(min_height);
	}

	void Update() {
		if (start_pos.y > min_height) {
			start_pos = start_pos + Vector3.down * Time.deltaTime;
		}

		x_offset += (back ? -1 : 1) * Time.deltaTime;
		if (x_offset > Mathf.PI) {
			back = true;
		}
		if (x_offset < -Mathf.PI) {
			back = false;
		}

		transform.position = start_pos + new Vector3(2 * x_offset, (back ? 1 : -1) * Mathf.Sin(x_offset), 0);
	}
}
