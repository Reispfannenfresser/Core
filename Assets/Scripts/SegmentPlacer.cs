using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPlacer : MonoBehaviour {
	[SerializeField]
	LayerMask construction = 0;

	GameObject to_place = null;
	bool can_place = false;
	Camera current_camera = null;
	Animator animator;
	SpriteRenderer sprite_renderer;

	int cost = 0;

	void Awake() {
		animator = GetComponent<Animator>();
		sprite_renderer = GetComponent<SpriteRenderer>();
	}

	void FixedUpdate() {
		if (current_camera != Camera.current && Camera.current != null) {
			current_camera = Camera.current;
		}
		if (current_camera != null) {
			Vector3 new_pos = current_camera.ScreenToWorldPoint(Input.mousePosition);
			new_pos.z = 0;
			transform.position = new_pos;
		}

		if (GameController.instance.GetZollars() < cost) {
			sprite_renderer.color = new Color(0.75f, 0.5f, 0.5f);
		} else {
			sprite_renderer.color = Color.white;
		}

		can_place = Physics2D.OverlapCircle(transform.position, 0.45f, construction) == null && Physics2D.OverlapCircle(transform.position, 0.75f, construction) != null;
		Color color = sprite_renderer.color;
		color.a = can_place ? 0.75f : 0.25f;
		sprite_renderer.color = color;
	}

	public void SetSegment(GameObject segment_object) {
		ConstructionSegment segment = segment_object.GetComponent<ConstructionSegment>();
		if (segment == null) {
			return;
		}

		cost = segment.value;
		to_place = segment_object;
	}

	void Update() {
		if (can_place && Input.GetMouseButtonDown(0)) {
			Place();
		}

		if (!can_place && Input.GetMouseButtonDown(1)) {
			Collider2D collider = Physics2D.OverlapPoint(transform.position, construction);
			if (collider == null) {
				return;
			}

			ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment.deletable) {
				segment.Delete();
			}
		}
	}

	void Place() {
		if (to_place == null || GameController.instance.GetZollars() < cost) {
			return;
		}

		GameController.instance.RemoveZollars(cost);

		Collider2D[] segments = Physics2D.OverlapCircleAll(transform.position, 0.75f, construction);

		GameObject new_object = GameObject.Instantiate(to_place, transform.position, transform.rotation);
		ConstructionSegment new_segment = new_object.GetComponent<ConstructionSegment>();
		foreach (Collider2D old_collider in segments) {
			ConstructionSegment old_segment = old_collider.gameObject.GetComponent<ConstructionSegment>();
			Rigidbody2D old_segment_rb2d = old_segment.GetComponent<Rigidbody2D>();
			HingeJoint2D joint = new_object.AddComponent<HingeJoint2D>();
			joint.connectedBody = old_segment_rb2d;
			JointMotor2D motor = joint.motor;
			motor.maxMotorTorque = 10;
			motor.motorSpeed = 0;
			joint.motor = motor;
			joint.useMotor = true;
			old_segment.AddConnector(joint);
		}
	}
}
