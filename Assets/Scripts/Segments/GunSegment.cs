using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ConstructionSegment))]
public class GunSegment : MonoBehaviour {
	[SerializeField]
	Transform gun = null;
	[SerializeField]
	LayerMask construction = 0;
	[SerializeField]
	int damage = 1;
	[SerializeField]
	float shot_cooldown = 1;
	float current_shot_cooldown = 0;

	LineRenderer fire = null;
	Animator animator = null;

	Enemy target = null;
	AudioSource shot_audio = null;

	private ConstructionSegment segment = null;

	private void Awake() {
		fire = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();
		shot_audio = GetComponent<AudioSource>();
		segment = GetComponent<ConstructionSegment>();
	}

	private void Start() {
		shot_audio.pitch += Random.value * 0.125f - 0.0625f;
		current_shot_cooldown += Random.value * shot_cooldown;

		segment.fixed_update_wrapper.AddAction("Gun", OnFixedUpdate);
	}

	private void OnFixedUpdate(ConstructionSegment segment) {
		current_shot_cooldown -= Time.deltaTime;

		if (current_shot_cooldown <= 0) {
			current_shot_cooldown += shot_cooldown;
			if (!TargetInSight(target)) {
				PickTarget();
			}
			if (target != null) {
				Fire();
			}
		}
	}

	private bool TargetInSight(Enemy target) {
		if (target == null) {
			return false;
		}

		Vector3 direction = target.transform.position - transform.position;
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 30, construction);
		foreach (RaycastHit2D hit in hits) {
			ConstructionSegment segment = hit.collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment != null && segment != this.segment) {
				return false;
			}
		}

		return true;
	}

	private void PickTarget() {
		foreach (KeyValuePair<int, Enemy> kvp in ObjectRegistry<Enemy>.objects) {
			Enemy enemy = kvp.Value;

			if (TargetInSight(enemy)) {
				target = enemy;
				return;
			}
		}
		target = null;
	}

	private void Fire() {
		Vector3 direction = target.transform.position - transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gun.rotation = Quaternion.Euler(0, 0, angle);
		target.damageable.Damage(damage);

		fire.SetPosition(0, transform.position + gun.right * 0.5f);
		fire.SetPosition(1, target.transform.position);
		animator.SetTrigger("Fire");
		shot_audio.Play();
	}
}
