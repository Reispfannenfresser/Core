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
	[SerializeField]
	float turn_speed = 1;

	public float current_turn_speed = 0;
	public float turn_acceleration = 90;

	public Event<Bomb> detonate_event;
	public Event<Bomb> launch_event;

	[SerializeField]
	float search_cooldown = 3;
	float current_search_cooldown = 0;

	Vector3 goal_pos = Vector3.zero;

	[SerializeField]
	GameObject fire = null;

	bool detonated = false;

	private Animator animator = null;
	private SpriteRenderer sr = null;
	private AudioSource explosion_audio = null;

	private bool is_launched = false;

	GameObject target = null;

	public void Awake() {
		detonate_event = new Event<Bomb>(this);
		launch_event = new Event<Bomb>(this);
		animator = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
		explosion_audio = GetComponent<AudioSource>();
	}

	public void Start() {
		current_search_cooldown = search_cooldown * Random.value;
	}

	public void Launch() {
		is_launched = true;
		fire.SetActive(true);
		sr.sortingOrder = 7;

		launch_event.RunEvent();
	}

	private void FixedUpdate() {
		if (!is_launched || detonated) {
			return;
		}

		current_search_cooldown -= Time.deltaTime;
		if (current_search_cooldown <= 0){
			current_search_cooldown += search_cooldown;
			if (target == null) {
				PickTarget();
				current_turn_speed = 0;
			}
			if (target == null) {
				current_turn_speed = 0;
				goal_pos = transform.position + new Vector3(30 * Random.value - 15, 30 * Random.value - 15, 0);
				goal_pos.x = Mathf.Clamp(goal_pos.x, -20, 20);
				goal_pos.y = Mathf.Clamp(goal_pos.y, -20, 20);
			}
		}

		Vector3 direction = transform.InverseTransformDirection(goal_pos - transform.position);
		if (target != null) {
			direction = transform.InverseTransformDirection(target.transform.position - transform.position);
		}
		direction.Normalize();

		float change_needed = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
		while (change_needed < -180) {
			change_needed += 360;
		}
		while (change_needed > 180) {
			change_needed -= 360;
		}

		if (current_turn_speed < turn_speed) {
			current_turn_speed += Time.deltaTime * turn_acceleration;
		} else {
			current_turn_speed = turn_speed;
		}

		transform.Rotate(new Vector3(0, 0, Mathf.Clamp(change_needed, -current_turn_speed, current_turn_speed)));
		transform.position += transform.up * Time.deltaTime * speed;
	}

	private void PickTarget() {
		GameObject new_target = null;
		float shortest_distance = Mathf.Infinity;
		foreach (Enemy enemy in Enemy.all_enemies) {
			float distance_to = (enemy.transform.position - transform.position).magnitude;
			if (distance_to < shortest_distance) {
				new_target = enemy.gameObject;
				shortest_distance = distance_to;
			}
		}
		target = new_target;
	}

	public void OnTriggerStay2D(Collider2D other) {
		if (is_launched && !detonated) {
			Detonate();
		}
	}

	public void Detonate() {
		detonated = true;
		fire.SetActive(false);
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosion_radius, enemies);
		foreach (Collider2D collider in colliders) {
			Enemy enemy = collider.gameObject.GetComponent<Enemy>();
			if (enemy != null) {
				((IDamageable) enemy).Damage(Mathf.Max(0, (int) (damage)));
			}
		}
		animator.SetTrigger("Boom");
		explosion_audio.Play();

		detonate_event.RunEvent();
	}

	private void Remove() {
		Destroy(gameObject);
	}
}
