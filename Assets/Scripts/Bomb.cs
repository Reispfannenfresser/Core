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
	GameObject fire = null;

	private Animator animator = null;

	private bool is_launched = false;

	public void Start() {
		animator = GetComponent<Animator>();
	}

	public void Launch() {
		is_launched = true;
		fire.SetActive(true);
	}

	private void FixedUpdate() {
		if (is_launched) {
			transform.position += transform.up * Time.deltaTime * speed;
		}
	}

	public void OnTriggerEnter2D(Collider2D other) {
		Detonate();
	}

	private void Detonate() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosion_radius, enemies);

		animator.SetTrigger("Detonate");
		Debug.Log("Boom!");

		foreach (Collider2D collider in colliders) {
			Enemy enemy = collider.gameObject.GetComponent<Enemy>();
			if (enemy != null) {
				enemy.Damage(damage);
			}
		}
	}
}
