using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSegment : ConstructionSegment {
	[SerializeField]
	Transform gun = null;
	[SerializeField]
	LayerMask construction = 0;
	[SerializeField]
	int damage = 1;
	[SerializeField]
	float shot_cooldown = 1;
	float current_shot_cooldown = 0;
	[SerializeField]
	float search_cooldown = 1;
	float current_search_cooldown = 0;

	LineRenderer fire = null;
	Animator animator = null;

	Enemy target = null;
	AudioSource shot_audio = null;

	protected override void Initialize() {
		base.Initialize();
		fire = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();
		shot_audio = GetComponent<AudioSource>();
	}

	protected override void Place() {
		base.Place();
		shot_audio.pitch += Random.value * 0.125f - 0.0625f;
		current_shot_cooldown += Random.value * shot_cooldown;
		current_search_cooldown += Random.value * search_cooldown;
	}

	protected override void OnFixedUpdate() {
		if (blocked) {
			return;
		}

		current_search_cooldown -= Time.deltaTime;
		current_shot_cooldown -= Time.deltaTime;

		if (current_search_cooldown <= 0) {
			current_search_cooldown += search_cooldown;
			if (target == null || !TargetInSight()) {
				PickTarget();
				return;
			}
		}
		if (current_shot_cooldown <= 0) {
			current_shot_cooldown += shot_cooldown;
			if (target != null) {
				Fire();
			}
		}
	}

	private void Fire() {
		if (!TargetInSight()) {
			return;
		}

		Vector3 direction = target.transform.position - transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gun.rotation = Quaternion.Euler(0, 0, angle);
		((IDamageable) target).Damage(damage);

		fire.SetPosition(0, transform.position + gun.right * 0.5f);
		fire.SetPosition(1, target.transform.position);
		animator.SetTrigger("Fire");
		shot_audio.Play();
	}

	private bool TargetInSight() {
		if (target == null) {
			return false;
		}

		Vector3 direction = target.transform.position - transform.position;
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 30, construction);
		foreach (RaycastHit2D hit in hits) {
			ConstructionSegment segment = hit.collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment != null && segment != this) {
				target = null;
				return false;
			}
			Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
			if (enemy == target) {
				return true;
			}
		}

		return true;
	}

	private void PickTarget() {
		foreach(Enemy enemy in Enemy.all_enemies) {
			Vector3 direction = enemy.transform.position - transform.position;
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 30, construction);
			bool aborted = false;
			foreach (RaycastHit2D hit in hits) {
				ConstructionSegment segment = hit.collider.gameObject.GetComponent<ConstructionSegment>();
				if (segment != null && segment != this) {
					aborted = true;
					break;
				}
				Enemy hit_enemy = hit.collider.gameObject.GetComponent<Enemy>();
				if (hit_enemy != null) {
					target = hit_enemy;
					return;
				}
			}
			if (!aborted) {
				target = enemy;
				return;
			}
		}
		target = null;
	}
}
