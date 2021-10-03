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
		transform.position += Vector3.down * Time.deltaTime * speed;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("HiIII");
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null || has_hit) {
			return;
		}

		segment.Damage(damage);
		has_hit = true;
		Destroy(gameObject);
	}
}
