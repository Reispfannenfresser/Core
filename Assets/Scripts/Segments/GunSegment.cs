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

	protected override void OnPlaced() {
		fire = gun.gameObject.GetComponent<LineRenderer>();
		animator = gun.gameObject.GetComponent<Animator>();
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

	private void Fire() {
		if (target == null || GameController.instance.GetZollars() < cost) {
			return;
		}

		GameController.instance.RemoveZollars(cost);

		Vector3 direction = target.transform.position - transform.position;
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 30, construction);
		if (hits.Length > 1) {
			target = null;
			return;
		}


		current_cooldown = cooldown;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gun.transform.rotation = Quaternion.Euler(0, 0, angle);
		target.Damage(damage);

		fire.SetPosition(0, transform.position + gun.transform.right * 0.5f);
		fire.SetPosition(1, target.transform.position);
		animator.SetTrigger("Fire");
	}

	private void PickTarget() {
		foreach(Enemy enemy in GameController.instance.enemies) {
			Vector3 direction = enemy.transform.position - transform.position;
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 30, construction);
			if (hits.Length == 1) {
				target = enemy;
				return;
			}
		}
		target = null;
	}
}
