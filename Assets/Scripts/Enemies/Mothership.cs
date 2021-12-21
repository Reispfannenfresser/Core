using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mothership : Enemy {
	[SerializeField]
	GameObject ufo = null;
	[SerializeField]
	float spawn_cooldown = 5f;
	[SerializeField]
	int ufo_call_count = 3;
	float current_spawn_cooldown = 0f;
	[SerializeField]
	float move_cooldown = 2f;
	float current_move_cooldown = 0f;
	[SerializeField]
	float laser_cooldown = 7f;
	float current_laser_cooldown = 0f;

	LineRenderer laser = null;
	Animator animator = null;
	AudioSource laser_audio = null;

	ConstructionSegment target = null;
	private bool is_attacking = false;

	private int total_damage = 0;

	private float angle = 0f;
	private float distance = 0f;

	protected override void Initialize() {
		base.Initialize();

		laser = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();
		laser_audio = GetComponent<AudioSource>();

		angle = Random.value * 2 * Mathf.PI;
		distance = 15 + (Random.value - 0.5f) * 10;

		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 40;
	}

	protected override void Spawn() {
		base.Spawn();

		current_move_cooldown = Random.value * move_cooldown;
		current_spawn_cooldown = Random.value * spawn_cooldown;
		current_laser_cooldown = Random.value * laser_cooldown;
	}

	protected override void OnFixedUpdate() {
		base.OnFixedUpdate();

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
			CallUfos();
		}

		if (current_laser_cooldown <= 0) {
			current_laser_cooldown += laser_cooldown;
			Laser();
		}

		laser.SetPosition(0, transform.position + Vector3.down);
		if (target != null) {
			laser.SetPosition(1, target.transform.position);
		}

		if (target != null && is_attacking && total_damage < 150) {
			target.damageable.Damage(50);
			total_damage += 50;
		}
	}

	void CallUfos() {
		for (int i = 0; i < ufo_call_count; i++) {
			Instantiate(ufo, transform.position, transform.rotation);
		}
	}

	void Laser() {
		int segment_count = ConstructionSegment.all_segments.Count;
		if (segment_count > 0) {
			int index = Random.Range(0, segment_count);
			foreach (ConstructionSegment segment in ConstructionSegment.all_segments) {
				if (index <= 0 && segment != null) {
					target = segment;
					animator.SetTrigger("Fire");
					laser_audio.Play();
					total_damage = 0;
					is_attacking = true;
					return;
				}
				index--;
			}
		}
	}

	void StopAttack() {
		is_attacking = false;
	}
}
