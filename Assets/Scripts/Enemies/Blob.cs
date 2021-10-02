using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Enemy {
	private bool connected = false;
	private Rigidbody2D rb2d = null;

	protected override void OnSpawned() {
		transform.position += Vector3.right * ((Random.value - 0.5f) * 8);
		transform.position += Vector3.up * ((Random.value - 0.5f) * 2);
		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		if (!connected) {
			rb2d.velocity = Vector3.down * 5;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null) {
			return;
		}
		else {
			connected = true;
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
}
