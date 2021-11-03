using UnityEngine;
using System.Collections.Generic;

public class AttachingObject : MonoBehaviour {
	Dictionary<AttachingObject, Joint2D> obj_to_joint = new Dictionary<AttachingObject, Joint2D>();

	protected Rigidbody2D rb2d = null;
	protected int attached_count {get;} = 0;

	private void Awake() {
		rb2d = GetComponent<Rigidbody2D>();
	}

	public bool AttachTo(AttachingObject obj) {
		if (obj == null || obj_to_joint.ContainsKey(obj)) {
			return false;
		}
		HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
		joint.connectedBody = obj.rb2d;
		joint.enableCollision = true;
		JointMotor2D motor = joint.motor;
		motor.maxMotorTorque = 10;
		motor.motorSpeed = 0;
		joint.motor = motor;
		joint.useMotor = true;

		obj_to_joint.Add(obj, joint);
		obj.OnAttachedTo(this, joint);

		return true;
	}

	public void DetachFrom(AttachingObject obj) {
		if (!obj_to_joint.ContainsKey(obj)) {
			return;
		}
		Joint2D joint = obj_to_joint[obj];
		obj_to_joint.Remove(obj);
		obj.OnDetachedFrom(this);
		Destroy(joint);
	}

	protected void OnAttachedTo(AttachingObject obj, Joint2D joint) {
		obj_to_joint.Add(obj, joint);
	}

	protected void OnDetachedFrom(AttachingObject obj) {
		obj_to_joint.Remove(obj);
	}

	public void DetachFromEverything() {
		foreach (KeyValuePair<AttachingObject, Joint2D> pair in obj_to_joint) {
			pair.Key.OnDetachedFrom(this);
			Destroy(pair.Value);
		}
	}
}
