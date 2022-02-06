using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Enemy))]
public class Muncher : MonoBehaviour {
	[SerializeField]
	float search_cooldown = 1;
	float current_search_cooldown = 0;

	[SerializeField]
	float munch_cooldown = 1;
	float current_munch_cooldown = 0;

	Bomb target = null;

	Vector3 current_velocity = Vector3.zero;

	private int state = 0;

	private AudioSource munch_audio = null;
	private Animator animator = null;
	private Enemy enemy = null;

	protected void Awake() {
		munch_audio = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		enemy = GetComponent<Enemy>();

		float angle = Random.value * 2 * Mathf.PI;
		float distance = 15 + (Random.value - 0.5f) * 10;
		current_search_cooldown =  Random.value * search_cooldown;

		if (transform.position == Vector3.zero) {
			transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 50;
			transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
		}

		munch_audio.pitch += Random.value * 0.125f - 0.0625f;
	}

	protected void Start() {
		enemy.fixed_update_wrapper.AddAction("Muncher", OnFixedUpdate);
		enemy.on_destroyed_wrapper.AddAction("Muncher", OnDestroyed);
	}

	private void OnFixedUpdate(Enemy e) {
		float urgency = 0.3f;
		Vector3 direction = -transform.right;
		Vector3 target_position = transform.position + direction * 2;

		switch (state) {
			case 0:
				if (current_search_cooldown > 0) {
					current_search_cooldown -= Time.deltaTime;
				}
				else {
					current_search_cooldown = search_cooldown;
					SearchForTarget();
					if ((transform.position.x > 15 && direction.x > 0) || (transform.position.x < -15 && direction.x < 0)) {
						direction.x *= -1;
					}
					if ((transform.position.y > 15 && direction.y > 0) || (transform.position.y < -15 && direction.y < 0)) {
						direction.y *= -1;
					}
					if (target != null) {
						state++;
						current_munch_cooldown = munch_cooldown;
					}
				}
				break;
			case 1:
				if (current_munch_cooldown > 0) {
					current_munch_cooldown -= Time.deltaTime;
				} else {
					current_munch_cooldown = 0;
					animator.SetTrigger("Munch");
					state++;
				}
				direction = target.transform.position - transform.position;
				target_position = target.transform.position - 1.5f * direction.normalized;
				urgency = current_munch_cooldown / munch_cooldown;
				break;
			case 2:
				if (target != null) {
					direction = target.transform.position - transform.position;
					target_position = target.transform.position - 1.5f * direction.normalized;
					urgency = current_munch_cooldown / munch_cooldown;
				}
				break;
		}

		transform.position = Vector3.SmoothDamp(transform.position, target_position, ref current_velocity, urgency);

		float target_rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
		transform.rotation = Quaternion.Euler(0, 0, target_rotation);
	}

	private void Munch() {
		if (target == null) {
			return;
		}

		target.on_destroyed_wrapper.RemoveAction("MuncherTarget_" + enemy.unique_id);
		target.Kill();

		LoseTarget();
	}

	private void LoseTarget() {
		transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
		current_search_cooldown = search_cooldown;
		target = null;

		state = 0;
	}

	private void PickTarget(Bomb bomb) {
		target = bomb;
		target.on_destroyed_wrapper.AddAction("MuncherTarget_" + enemy.unique_id, bomb => {
			LoseTarget();
		});
	}

	protected void SearchForTarget() {
		float min_distance = Mathf.Infinity;
		Bomb new_target = null;

		foreach (KeyValuePair<int, Bomb> kvp in ObjectRegistry<Bomb>.objects) {
			Bomb bomb = kvp.Value;
			if (bomb.is_launched) {
				float distance = (bomb.transform.position - transform.position).magnitude;
				if (distance < min_distance) {
					new_target = bomb;
					min_distance = distance;
				}
			}
		}
		if (new_target != null) {
			PickTarget(new_target);
		}
	}

	private void OnDestroyed(Enemy enemy) {
		if (target != null) {
			target.on_destroyed_wrapper.RemoveAction("MuncherTarget_" + enemy.unique_id);
		}
	}
}
