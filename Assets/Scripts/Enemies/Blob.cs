using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Enemy {
	protected bool collided = false;
	protected Rigidbody2D rb2d = null;

	protected Vector3 direction = Vector3.zero;
	[SerializeField]
	protected float speed = 5;

	protected AttachingObject attaching_object = null;
	protected ObjectBlocker object_blocker = null;

	protected override void Initialize() {
		base.Initialize();

		rb2d = gameObject.GetComponent<Rigidbody2D>();
		attaching_object = GetComponent<AttachingObject>();
		object_blocker = GetComponent<ObjectBlocker>();

		float angle = Random.value * Mathf.PI * 2 - Mathf.PI;
		float distance = 30 + Random.value * 10;

		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;
	}

	protected override void Spawn() {
		base.Spawn();

		direction = -transform.position;
		direction.Normalize();
	}

	protected override void OnFixedUpdate() {
		base.OnFixedUpdate();

		if (!collided) {
			rb2d.velocity = direction * speed;
		} else if (object_blocker.block_count == 0) {
			collided = false;
			direction = -transform.position;
			direction.Normalize();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		AttachingObject attach_to = collision.gameObject.GetComponent<AttachingObject>();
		if (attach_to == null) {
			return;
		}
		if (attaching_object.AttachTo(attach_to)) {
			OnAttached(attach_to);
		}
	}

	protected virtual void OnAttached(AttachingObject attach_to) {
		collided = true;
		IBlockable[] blockables = attach_to.gameObject.GetComponents<IBlockable>();
		foreach (IBlockable blockable in blockables) {
			if (blockable != null) {
				object_blocker.StartBlocking(blockable);
			}
		}
	}

	protected override void OnDestroyed() {
		base.OnDestroyed();

		attaching_object.DetachFromEverything();
		object_blocker.FreeEverything();
	}
}
