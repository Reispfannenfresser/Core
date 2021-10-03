using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Enemy {
	private bool collided = false;
	private Rigidbody2D rb2d = null;

	private Vector3 direction = Vector3.zero;
	[SerializeField]
	float speed = 5;

	protected override void OnSpawned() {
		float distance = 30 + Random.value * 10;
		float angle = Random.value * Mathf.PI * 2 - Mathf.PI;

		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;

		direction = -transform.position;
		direction.Normalize();

		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		if (!collided) {
			rb2d.velocity = direction * speed;
		}
		if (transform.position.magnitude > 80) {
			Delete();
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null) {
			return;
		}
		collided = true;
		HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
		joint.connectedBody = segment.rb2d;
		JointMotor2D motor = joint.motor;
		motor.maxMotorTorque = 10;
		motor.motorSpeed = 0;
		joint.motor = motor;
		joint.useMotor = true;
		segment.AddConnector(joint);
	}
}
