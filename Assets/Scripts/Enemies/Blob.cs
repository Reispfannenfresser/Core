using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Enemy, ISegmentBlocker {
	protected bool collided = false;
	protected Rigidbody2D rb2d = null;

	protected Vector3 direction = Vector3.zero;
	[SerializeField]
	protected float speed = 5;

	[SerializeField]
	protected float regrab_cooldown = 1;
	protected float current_regrab_cooldown = 0;

	protected HashSet<ConstructionSegment> blocked_segments = new HashSet<ConstructionSegment>();

	protected override void OnSpawned() {
		float distance = 30 + Random.value * 10;
		float angle = Random.value * Mathf.PI * 2 - Mathf.PI;

		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;

		current_regrab_cooldown = regrab_cooldown;

		direction = -transform.position;
		direction.Normalize();

		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	protected override void OnFixedUpdate() {
		if (!collided) {
			rb2d.velocity = direction * speed;
		} else if (current_regrab_cooldown > 0 && blocked_segments.Count == 0) {
			current_regrab_cooldown -= Time.deltaTime;

			if (current_regrab_cooldown <= 0) {
				collided = false;
				direction = -transform.position;
				direction.Normalize();
			}
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
		joint.enableCollision = true;
		segment.AddConnector(joint);
		current_regrab_cooldown = regrab_cooldown;
		OnConnected(segment);
	}

	protected virtual void OnConnected(ConstructionSegment segment) {
		(this as ISegmentBlocker).StartBlocking(segment);
	}

	protected override void OnDestroyed() {
		Debug.Log("Hi");
		foreach (ConstructionSegment segment in blocked_segments) {
			if (segment != null) {
				segment.RemoveBlocker(this);
			}
		}
	}

	void ISegmentBlocker.StartBlocking(ConstructionSegment segment) {
		segment.AddBlocker(this);
		blocked_segments.Add(segment);
	}

	void ISegmentBlocker.StopBlocking(ConstructionSegment segment) {
		segment.RemoveBlocker(this);
		blocked_segments.Remove(segment);
	}
}
