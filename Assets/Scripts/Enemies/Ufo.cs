using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Ufo : MonoBehaviour {
	private bool back = false;
	private Vector3 start_pos;
	private float min_height;

	[SerializeField]
	GameObject shot = null;

	[SerializeField]
	float cooldown = 1f;
	float current_cooldown = 0f;

	float flee_cooldown = 0;

	float angle = 0f;
	float distance = 0f;
	float speed = 1f;

	float flee_speed = 7;

	AudioSource shot_audio = null;
	protected Enemy enemy = null;

	protected void Awake() {
		shot_audio = GetComponent<AudioSource>();
		enemy = GetComponent<Enemy>();

		angle = Random.value * 2 * Mathf.PI;
		distance = 15 + (Random.value - 0.5f) * 10;
		back = Random.value > 0.5f;
		speed = 0.2f + (Random.value - 0.5f) * 0.2f;

		if (transform.position == Vector3.zero) {
			transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 50;
		}

		shot_audio.pitch += Random.value * 0.125f - 0.0625f;
	}

	protected void Start() {
		enemy.on_spawned_wrapper.AddAction("Ufo", OnSpawned);
		enemy.fixed_update_wrapper.AddAction("Ufo", OnFixedUpdate);
	}

	private void OnSpawned(Enemy e) {
		flee_cooldown = 20 + (Random.value - 0.5f) * 10;
		current_cooldown = Random.value * cooldown;
	}

	private void OnFixedUpdate(Enemy e) {
		if (Enemy.boss_enemies <= 0) {
			flee_cooldown -= Time.deltaTime;
		}
		if (flee_cooldown < 0) {
			distance += flee_speed * Time.deltaTime;
		}
		angle += (back ? -1 : 1) * Time.deltaTime * speed;

		Vector3 wanted_pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;

		transform.position += (wanted_pos - transform.position) * Time.deltaTime;

		current_cooldown -= Time.deltaTime;
		if (current_cooldown <= 0) {
			current_cooldown += cooldown;
			Shoot();
		}
	}

	private void Shoot() {
		Vector3 direction = -transform.position;
		direction = direction.normalized;

		Instantiate(shot, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
		shot_audio.Play();
	}
}
