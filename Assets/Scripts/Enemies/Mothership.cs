using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mothership : Enemy {
	[SerializeField]
	GameObject ufo = null;
	[SerializeField]
	float spawn_cooldown = 5f;
	float current_spawn_cooldown = 0f;
	[SerializeField]
	float move_cooldown = 2f;
	float current_move_cooldown = 0f;
	[SerializeField]
	float laser_cooldown = 7f;
	float current_laser_cooldown = 0f;

	LineRenderer laser = null;
	Animator animator = null;

	ConstructionSegment target = null;
	private bool is_attacking = false;

	private float angle = 0f;
	private float distance = 0f;
	protected override void OnSpawned() {
		angle = Random.value * 2 * Mathf.PI;
		distance = 15 + (Random.value - 0.5f) * 10;

		current_move_cooldown = Random.value * move_cooldown;
		current_spawn_cooldown = Random.value * spawn_cooldown;
		current_laser_cooldown = Random.value * laser_cooldown;

		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 40;
		laser = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();

		GameController.instance.AddBoss();
	}

	private void FixedUpdate() {
		if (current_move_cooldown <= 0) {
			current_move_cooldown += move_cooldown;
			angle += (Random.value - 0.5f) * Mathf.PI;
		}

		transform.position += (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance - transform.position) * Time.deltaTime;

		current_spawn_cooldown -= Time.deltaTime;
		current_move_cooldown -= Time.deltaTime;
		current_laser_cooldown -= Time.deltaTime;

		if (current_spawn_cooldown <= 0) {
			current_spawn_cooldown += spawn_cooldown;
			CallUfo();
		}

		if (current_spawn_cooldown <= 0) {
			current_spawn_cooldown += spawn_cooldown;
			CallUfo();
		}

		if (current_laser_cooldown <= 0) {
			current_laser_cooldown += laser_cooldown;
			Laser();
		}

		laser.SetPosition(0, transform.position + Vector3.down);
		if (target != null) {
			laser.SetPosition(1, target.transform.position);
		}

		if (target != null && is_attacking) {
			target.Damage(20);
		}
	}

	void CallUfo() {
		Instantiate(ufo, transform.position, transform.rotation);
	}

	void Laser() {
		int segment_count = GameController.instance.segments.Count;
		if (segment_count > 0) {
			int index = Random.Range(0, segment_count);
			foreach (ConstructionSegment segment in GameController.instance.segments) {
				if (index <= 0 && segment != null) {
					target = segment;
					animator.SetTrigger("Fire");
					is_attacking = true;
					return;
				}
				index--;
			}
		}
	}

	void DamageTarget() {
		is_attacking = false;
	}

	protected override void OnKilled() {
		GameController.instance.RemoveBoss();
	}

	protected override void OnDeleted() {
		GameController.instance.RemoveBoss();
	}
}
