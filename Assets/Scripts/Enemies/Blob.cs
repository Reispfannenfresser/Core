using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AttachingObject))]
[RequireComponent(typeof(Enemy))]
public class Blob : MonoBehaviour {
	protected bool collided = false;
	protected Rigidbody2D rb2d = null;

	[SerializeField]
	protected float speed = 5;

	protected AttachingObject attaching_object = null;
	protected ObjectBlocker object_blocker = null;
	protected Enemy enemy = null;

	protected float mass = 0;

	protected void Awake() {
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		attaching_object = GetComponent<AttachingObject>();
		object_blocker = GetComponent<ObjectBlocker>();
		enemy = GetComponent<Enemy>();

		mass = rb2d.mass;

		float angle = Random.value * Mathf.PI * 2 - Mathf.PI;
		float distance = 30 + Random.value * 10;

		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;
	}

	protected void Start() {
		enemy.on_spawned_wrapper.AddAction("Blob", OnSpawned);
		enemy.fixed_update_wrapper.AddAction("Blob", OnFixedUpdate);
		enemy.on_destroyed_wrapper.AddAction("Blob", OnDestroyed);
	}

	private void OnSpawned(Enemy e) {
		rb2d.mass = 0.0001f;
	}

	private void OnFixedUpdate(Enemy e) {
		if (!collided) {
			Vector3 direction = -transform.position;
			direction = direction.normalized;
			rb2d.velocity = direction * speed;
		} else if (object_blocker.block_count == 0) {
			collided = false;
			rb2d.mass = 0.0001f;
		}
	}

	private void OnDestroyed(Enemy e) {
		attaching_object.DetachFromEverything();
		object_blocker.FreeEverything();
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
				rb2d.mass = mass;
			}
		}
	}
}
