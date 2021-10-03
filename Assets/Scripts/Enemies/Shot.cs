using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {
	[SerializeField]
	float speed = 5f;
	[SerializeField]
	int damage = 5;
	bool has_hit = false;
	Vector3 direction = Vector3.zero;

	private void Start() {
		GameController.instance.AddShot(this);

		direction -= transform.position;
		direction.Normalize();
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
	}

	void FixedUpdate() {
		transform.position += direction * Time.deltaTime * speed;
		if (transform.position.y < -40) {
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null || has_hit) {
			return;
		}

		segment.Damage(damage);
		has_hit = true;
		GameController.instance.RemoveShot(this);
		Destroy(gameObject);
	}
}
