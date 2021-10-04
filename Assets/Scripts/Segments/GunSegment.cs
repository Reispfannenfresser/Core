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

	[SerializeField]
	int cost = 1;

	LineRenderer fire = null;
	Animator animator = null;

	Enemy target = null;
	AudioSource shot_audio = null;
	bool is_playing = false;

	public static int sound_amount = 0;

	protected override void OnPlaced() {
		fire = gun.gameObject.GetComponent<LineRenderer>();
		animator = gun.gameObject.GetComponent<Animator>();
		shot_audio = gun.gameObject.GetComponent<AudioSource>();
	}

	void FixedUpdate() {
		current_cooldown -= Time.deltaTime;
		if (current_cooldown < 0) {
			current_cooldown = 0;
		}

		if (target == null) {
			PickTarget();
			return;
		}

		if (current_cooldown <= 0) {
			Fire();
		}
	}

	protected override void OnDestroyed() {
		if (is_playing) {
			sound_amount -= 1;
		}
	}

	protected override void OnDeleted() {
		if (is_playing) {
			sound_amount -= 1;
		}
	}

	private void Fire() {
		if (target == null || GameController.instance.GetZollars() < cost) {
			return;
		}

		GameController.instance.RemoveZollars(cost);

		Vector3 direction = target.transform.position - transform.position;
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 30, construction);
		foreach (RaycastHit2D hit in hits) {
			ConstructionSegment segment = hit.collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment != null && segment != this) {
				target = null;
				return;
			}
			Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
			if (enemy != null && enemy == target) {
				break;
			}
		}

		current_cooldown = cooldown;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gun.transform.rotation = Quaternion.Euler(0, 0, angle);
		target.Damage(damage);

		fire.SetPosition(0, transform.position + gun.transform.right * 0.5f);
		fire.SetPosition(1, target.transform.position);
		animator.SetTrigger("Fire");

		if (sound_amount < 3 && !shot_audio.isPlaying) {
			shot_audio.Play();
			is_playing = true;
			sound_amount += 1;
		}
		if (is_playing && !shot_audio.isPlaying) {
			sound_amount -= 1;
		}
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
				Enemy enemy_component = hit.collider.gameObject.GetComponent<Enemy>();
				if (enemy_component != null) {
					target = enemy_component;
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
