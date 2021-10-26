using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConstructionSegment : MonoBehaviour {
	public static HashSet<ConstructionSegment> all_segments = new HashSet<ConstructionSegment>();

	protected List<Joint2D> connectors = new List<Joint2D>();

	[SerializeField]
	protected SpriteRenderer[] sprite_renderers = new SpriteRenderer[0];
	public Rigidbody2D rb2d = null;

	public int max_hp = 100;
	public int hp = 100;
	public bool deletable = true;
	public bool meltable = true;
	public bool blockable = false;
	public bool should_pause = false;
	public int value = 10;
	public float radius = 0.4f;

	public static float max_distance = 0.4f;
	public static float max_overlap = 0.1f;

	protected int blocker_count = 0;

	private bool initialized = false;

	HashSet<Blob> blocked_by = new HashSet<Blob>();

	protected void Awake() {
		all_segments.Add(this);
		rb2d = GetComponent<Rigidbody2D>();
	}

	protected void Start() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius + max_distance, 1 << gameObject.layer);

		foreach (Collider2D collider in colliders) {
			ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment == null || !segment.initialized || segment == this) {
				continue;
			}

			Rigidbody2D segment_rb2d = segment.GetComponent<Rigidbody2D>();
			HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
			joint.connectedBody = segment_rb2d;
			JointMotor2D motor = joint.motor;
			motor.maxMotorTorque = 10;
			motor.motorSpeed = 0;
			joint.motor = motor;
			joint.useMotor = true;
			segment.AddConnector(joint);
		}

		initialized = true;

		OnPlaced();
	}

	protected virtual void OnPlaced() {
	}

	private void FixedUpdate() {
		if ((!blockable || blocker_count == 0) && (!should_pause || Enemy.all_enemies.Count > 0)) {
			OnFixedUpdate();
		}
	}

	protected virtual void OnFixedUpdate() {
	}

	public void Damage(int amount) {
		hp -= amount;
		OnDamaged(amount);
		UpdateColor();

		if (hp < 0) {
			Destroy(gameObject);
		}
	}

	protected virtual void OnDamaged(int amount) {
	}

	public void Heal(int amount) {
		hp += amount;
		if (hp > max_hp) {
			hp = max_hp;
		}
		OnHealed(amount);
		UpdateColor();
	}

	protected virtual void OnHealed(int amount) {
	}

	public void AddBlocker(Blob blob) {
		blocker_count++;
		if (blocker_count == 1) {
			OnBlocked();
			UpdateColor();
		}

		blocked_by.Add(blob);
	}

	protected virtual void OnBlocked() {
	}

	public void RemoveBlocker(Blob blob) {
		blocker_count--;
		if (blocker_count == 0) {
			OnUnBlocked();
			UpdateColor();
		}

		blocked_by.Remove(blob);
	}

	protected virtual void OnUnBlocked() {
	}

	private void UpdateColor() {
		foreach (SpriteRenderer renderer in sprite_renderers) {
			float gb_values = (float) hp / max_hp;
			float r_value = gb_values / 2 + 0.5f;
			float a_value = renderer.color.a;
			if (blockable && blocker_count > 0) {
				r_value *= 0.25f;
				gb_values *= 0.25f;
			}
			renderer.color = new Color(r_value, gb_values, gb_values, a_value);
		}
	}

	public void AddConnector(Joint2D joint) {
		connectors.Add(joint);
	}

	public void Delete() {
		OnDeleted();
		GameController.instance.AddZollars(value * hp / max_hp);
		Destroy(gameObject);
	}

	protected virtual void OnDeleted() {
	}

	protected void OnDestroy() {
		OnDestroyed();

		foreach(Joint2D connector in connectors) {
			if (connector != null) {
				Destroy(connector);
			}
		}
		foreach (Blob blob in blocked_by) {
			blob.StopBlocking(this);
		}

		all_segments.Remove(this);
	}

	protected virtual void OnDestroyed() {
	}
}
