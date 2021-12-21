using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Damageable))]
public class ConstructionSegment : MonoBehaviour, IBlockable {
	public static HashSet<ConstructionSegment> all_segments = new HashSet<ConstructionSegment>();

	[SerializeField]
	protected SpriteRenderer[] sprite_renderers = new SpriteRenderer[0];
	[SerializeField]
	protected Rigidbody2D rb2d = null;

	[SerializeField]
	protected bool deletable = true;
	[SerializeField]
	protected bool meltable = true;

	[SerializeField]
	protected int cost = 10;

	[SerializeField]
	protected float radius = 0.5f;

	int blocker_count = 0;
	public bool blocked {get; protected set;} = false;

	public static float max_distance {get;} = 0.4f;
	public static float max_overlap {get;} = 0.1f;

	public Damageable damageable {get; protected set;} = null;
	private AttachingObject attaching_object = null;

	private bool started = false;

	protected HashSet<ObjectBlocker> blocked_by = new HashSet<ObjectBlocker>();

	private void Awake() {
		Initialize();
	}

	protected virtual void Initialize() {
		rb2d = GetComponent<Rigidbody2D>();
		attaching_object = GetComponent<AttachingObject>();
		damageable = GetComponent<Damageable>();

		all_segments.Add(this);
	}

	private void Start() {
		Place();

		damageable.on_damaged_wrapper.AddAction("Segment_ChangeColor", e => {
			UpdateColor();
		});
		damageable.on_healed_wrapper.AddAction("Segment_ChangeColor", e => {
			UpdateColor();
		});
		damageable.on_killed_wrapper.AddAction("Destroy", e => {
			Destroy(gameObject);
		});

		started = true;
	}

	protected virtual void Place() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius + max_distance, 1 << gameObject.layer);

		foreach (Collider2D collider in colliders) {
			ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment == null || !segment.started || segment == this) {
				continue;
			}

			attaching_object.AttachTo(segment.attaching_object);
		}
	}

	private void FixedUpdate() {
		OnFixedUpdate();
	}

	protected virtual void OnFixedUpdate() {

	}

	void IBlockable.OnBlocked(ObjectBlocker blocker) {
		blocked_by.Add(blocker);
		if (blocker_count++ == 0) {
			Block();
		}
	}

	void IBlockable.OnFreed(ObjectBlocker blocker) {
		blocked_by.Remove(blocker);
		if (--blocker_count == 0) {
			Free();
		}
	}

	protected virtual void Block() {
		blocked = true;
		UpdateColor();
	}

	protected virtual void Free() {
		blocked = false;
		UpdateColor();
	}

	private void UpdateColor() {
		foreach (SpriteRenderer renderer in sprite_renderers) {
			if (renderer != null) {
				float gb_values = (float) damageable.hp / damageable.max_hp;
				float r_value = gb_values / 2 + 0.5f;
				float a_value = renderer.color.a;
				if (blocked) {
					r_value *= 0.25f;
					gb_values *= 0.25f;
				}
				renderer.color = new Color(r_value, gb_values, gb_values, a_value);
			}
		}
	}

	public int GetCost() {
		return cost;
	}

	public float GetRadius() {
		return radius;
	}

	public bool IsDeletable() {
		return deletable;
	}

	public bool IsMeltable() {
		return meltable;
	}

	public virtual void Delete() {
		GameController.instance.AddZollars((int) (cost * damageable.hp / (float) damageable.max_hp));
		Destroy(gameObject);
	}

	protected void OnDestroy() {
		OnDestroyed();
	}

	protected virtual void OnDestroyed() {
		all_segments.Remove(this);

		foreach (ObjectBlocker blocker in blocked_by) {
			blocker.OnFreed(this);
		}
		blocked_by.Clear();
	}
}
