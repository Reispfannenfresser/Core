using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSegment : ConstructionSegment {
	[SerializeField]
	GameObject gun = null;
	[SerializeField]
	LayerMask construction = 0;
	[SerializeField]
	int damage = 1;
	[SerializeField]
	float cooldown = 1;
	float current_cooldown = 0;

	LineRenderer fire = null;
	Animator animator = null;

	Enemy target = null;
	AudioSource shot_audio = null;

	protected override void OnPlaced() {
		fire = gun.gameObject.GetComponent<LineRenderer>();
		animator = gun.gameObject.GetComponent<Animator>();
		shot_audio = gun.gameObject.GetComponent<AudioSource>();
		current_cooldown += Random.value * cooldown;
	}

	protected override void OnFixedUpdate() {
		current_cooldown -= Time.deltaTime;

		if (current_cooldown <= 0) {
			current_cooldown += cooldown;
			if (target == null) {
				PickTarget();
				return;
			}
			Fire();
		}
	}

	private void Fire() {
		if (!TargetInSight()) {
			return;
		}

		Vector3 direction = target.transform.position - transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gun.transform.rotation = Quaternion.Euler(0, 0, angle);
		target.Damage(damage);

		fire.SetPosition(0, transform.position + gun.transform.right * 0.5f);
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
		foreach(Enemy enemy in GameController.instance.enemies) {
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
