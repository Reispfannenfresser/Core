using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
	[SerializeField]
	private int damage = 100;
	[SerializeField]
	private int explosion_radius = 3;
	[SerializeField]
	LayerMask enemies = 0;
	[SerializeField]
	float speed = 3;
	float current_speed = 3;
	[SerializeField]
	float max_acceleration = 0.1f;
	[SerializeField]
	float turn_speed = 1;
	float current_turn_speed = 1;
	[SerializeField]
	float max_turn_acceleration = 0.1f;

	[SerializeField]
	float search_cooldown = 3;
	float current_search_cooldown = 0;

	Vector3 goal_pos = Vector3.zero;

	[SerializeField]
	GameObject fire = null;

	bool detonated = false;

	private Animator animator = null;
	private SpriteRenderer sr = null;

	private bool is_launched = false;

	GameObject target = null;

	public void Awake() {
		GameController.instance.AddBomb(this);
		animator = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
	}

	public void Start() {
		current_search_cooldown = search_cooldown * Random.value;
	}

	public void Launch() {
		is_launched = true;
		fire.SetActive(true);
		current_speed = speed;
		current_turn_speed = turn_speed;
		sr.sortingOrder = 7;
	}

	private void FixedUpdate() {
		if (!is_launched || detonated) {
			return;
		}
		transform.position += transform.up * Time.deltaTime * current_speed;

		current_search_cooldown -= Time.deltaTime;
		if (current_search_cooldown <= 0){
			current_search_cooldown += search_cooldown;
			if (target == null) {
				PickTarget();
			}
			goal_pos = transform.position + new Vector3(10 * Random.value - 5, 10 * Random.value - 5, 0);
			goal_pos.x = Mathf.Clamp(goal_pos.x, -20, 20);
			goal_pos.y = Mathf.Clamp(goal_pos.y, -20, 20);
		}

		Vector3 direction = transform.InverseTransformDirection(goal_pos - transform.position);

		if (target != null) {
			direction = transform.InverseTransformDirection(target.transform.position - transform.position);
		}
		float distance = direction.magnitude;
		direction.Normalize();

		float change_needed = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

		float angle_factor = 1 - Mathf.Abs(change_needed / 180);
		float wanted_speed = ((angle_factor * 2) - 1) * speed;

		current_speed += Mathf.Clamp(wanted_speed - current_speed, -max_acceleration, max_acceleration);
		current_speed = Mathf.Clamp(current_speed, 0, speed);

		float distance_factor = 1 - (Mathf.Min(5, distance) / 5);
		float wanted_turn_speed = ((distance_factor * 2) - 1) * turn_speed;
		current_turn_speed += Mathf.Clamp(wanted_turn_speed - current_turn_speed, -max_turn_acceleration, max_turn_acceleration);
		current_turn_speed = Mathf.Clamp(current_turn_speed, 0, turn_speed);

		float rotational_change = Mathf.Clamp(change_needed, -current_turn_speed, current_turn_speed) * Time.deltaTime;
		transform.Rotate(new Vector3(0, 0, rotational_change));
	}

	private void PickTarget() {
		GameObject new_target = null;
		float shortest_distance = Mathf.Infinity;
		foreach (Enemy enemy in GameController.instance.enemies) {
			float distance_to = (enemy.transform.position - transform.position).magnitude;
			if (distance_to < shortest_distance) {
				new_target = enemy.gameObject;
				shortest_distance = distance_to;
			}
		}
		target = new_target;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (!detonated) {
			Detonate();
		}
	}

	private void Detonate() {
		detonated = true;
		fire.SetActive(false);
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosion_radius, enemies);
		foreach (Collider2D collider in colliders) {
			Enemy enemy = collider.gameObject.GetComponent<Enemy>();
			if (enemy != null) {
				float multiplier = 1 - (enemy.transform.position - transform.position).magnitude / explosion_radius;
				enemy.Damage(Mathf.Max(0, (int) (damage * multiplier)));
			}
		}
		animator.SetTrigger("Boom");
	}

	private void Remove() {
		Destroy(gameObject);
	}
}
