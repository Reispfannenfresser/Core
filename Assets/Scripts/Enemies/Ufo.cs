using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : Enemy {
	private bool back = false;
	private Vector3 start_pos;
	private float min_height;

	[SerializeField]
	GameObject shot = null;

	[SerializeField]
	float cooldown = 1f;
	float current_cooldown = 0f;

	private float x_offset = 0f;

	protected override void OnSpawned() {
		start_pos = transform.position;
		min_height = 8 + (Random.value - 0.5f) * 5;
		x_offset = (Random.value - 0.5f) * 2 * Mathf.PI;
		back = Random.value > 0.5f;
		current_cooldown = Random.value * cooldown;
	}

	private void FixedUpdate() {
		current_cooldown -= Time.deltaTime;
		if (current_cooldown <= 0) {
			current_cooldown += cooldown;
			Shoot();
		}
	}

	private void Shoot() {
		Instantiate(shot, transform.position, transform.rotation);
	}

	void Update() {
		if (start_pos.y > min_height) {
			start_pos = start_pos + Vector3.down * Time.deltaTime * 5;
		}

		x_offset += (back ? -1 : 1) * Time.deltaTime;
		if (x_offset > Mathf.PI) {
			back = true;
		}
		if (x_offset < -Mathf.PI) {
			back = false;
		}

		transform.position = start_pos + new Vector3(x_offset, (back ? 1 : -1) * Mathf.Sin(x_offset), 0);
	}
}
