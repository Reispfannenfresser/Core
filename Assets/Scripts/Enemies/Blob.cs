using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Enemy {
	private bool collided = false;
	private Rigidbody2D rb2d = null;

	private Vector3 direction = Vector3.zero;
	[SerializeField]
	float speed = 5;

	HashSet<ConstructionSegment> blocked_segments = new HashSet<ConstructionSegment>();

	protected override void OnSpawned() {
		float distance = 30 + Random.value * 10;
		float angle = Random.value * Mathf.PI * 2 - Mathf.PI;

		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;

		direction = -transform.position;
		direction.Normalize();

		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	protected override void OnFixedUpdate() {
		if (!collided) {
			rb2d.velocity = direction * speed;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		ConstructionSegment segment = collision.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null) {
			return;
		}

		if (!collided) {
			rb2d.velocity = Vector3.zero;
			collided = true;
		}
		HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
		joint.connectedBody = segment.rb2d;
		segment.AddConnector(joint);
		StartBlocking(segment);
	}

	protected override void OnKilled() {
		RemoveAsBlocker();
	}

	protected override void OnDeleted() {
		RemoveAsBlocker();
	}

	private void StartBlocking(ConstructionSegment segment) {
		segment.AddBlocker();
		blocked_segments.Add(segment);
	}

	private void RemoveAsBlocker() {
		foreach (ConstructionSegment segment in blocked_segments) {
			if (segment != null) {
				segment.RemoveBlocker();
			}
		}
	}
}
