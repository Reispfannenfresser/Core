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
	private float total_x_offset = 0f;

	float angle = 0f;
	float distance = 0f;
	float speed = 1f;

	protected override void OnSpawned() {
		angle = Random.value * 2 * Mathf.PI;
		distance = 15 + (Random.value - 0.5f) * 10;
		back = Random.value > 0.5f;
		speed = 0.2f + (Random.value - 0.5f) * 0.2f;
		current_cooldown = Random.value * cooldown;
		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 40;
	}

	private void FixedUpdate() {
		angle += (back ? -1 : 1) * Time.deltaTime * speed;

		transform.position += (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance - transform.position) * Time.deltaTime;

		current_cooldown -= Time.deltaTime;
		if (current_cooldown <= 0) {
			current_cooldown += cooldown;
			Shoot();
		}
	}

	private void Shoot() {
		Instantiate(shot, transform.position, transform.rotation);
	}
}
