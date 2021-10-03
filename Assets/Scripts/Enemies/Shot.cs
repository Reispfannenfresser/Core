using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {
	[SerializeField]
	float speed = 5f;
	[SerializeField]
	int damage = 5;
	bool has_hit = false;

	private void Start() {
		GameController.instance.AddShot(this);
	}

	void FixedUpdate() {
		transform.position += Vector3.down * Time.deltaTime * speed;
		if (transform.position.y < -20) {
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
