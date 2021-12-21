using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {
	[SerializeField]
	float speed = 5f;
	[SerializeField]
	int damage = 5;
	bool has_hit = false;

	void FixedUpdate() {
		transform.position += transform.right * Time.deltaTime * speed;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		ConstructionSegment segment = other.gameObject.GetComponentInParent<ConstructionSegment>();
		if (segment == null || has_hit) {
			return;
		}

		segment.damageable.Damage(damage);
		has_hit = true;
		Destroy(gameObject);
	}
}
