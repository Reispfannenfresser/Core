using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConstructionSegment : MonoBehaviour, IBlockable {
	[SerializeField]
	private SpriteRenderer[] sprite_renderers = new SpriteRenderer[0];

	public Rigidbody2D rb2d {get; private set;} = null;

	[SerializeField]
	private int max_hp = 0;

	public int unique_id {get; private set;} = 0;

	[field: SerializeField]
	public bool deletable {get; private set;} = true;
	[field: SerializeField]
	public bool meltable {get; private set;} = true;

	[field: SerializeField]
	public int cost {get; protected set;} = 10;

	[field: SerializeField]
	public float radius {get; protected set;} = 0.5f;

	private int blocker_count = 0;
	public bool blocked {get; protected set;} = false;

	public static float max_distance {get;} = 0.4f;
	public static float max_overlap {get;} = 0.1f;

	public Damageable damageable {get; protected set;} = null;
	private AttachingObject attaching_object = null;

	public bool started {get; protected set;} = false;

	private HashSet<ObjectBlocker> blocked_by = new HashSet<ObjectBlocker>();

	private Event<ConstructionSegment> on_spawned_event = null;
	private Event<ConstructionSegment> fixed_update_event = null;
	private Event<ConstructionSegment> on_destroyed_event = null;
	private Event<ConstructionSegment> on_blocked_event = null;
	private Event<ConstructionSegment> on_freed_event = null;

	public EventWrapper<ConstructionSegment> on_spawned_wrapper = null;
	public EventWrapper<ConstructionSegment> fixed_update_wrapper = null;
	public EventWrapper<ConstructionSegment> on_destroyed_wrapper = null;
	public EventWrapper<ConstructionSegment> on_blocked_wrapper = null;
	public EventWrapper<ConstructionSegment> on_freed_wrapper = null;

	private void Awake() {
		rb2d = GetComponent<Rigidbody2D>();
		attaching_object = GetComponent<AttachingObject>();

		damageable = new Damageable(max_hp);

		on_spawned_event = new Event<ConstructionSegment>(this);
		fixed_update_event = new Event<ConstructionSegment>(this);
		on_destroyed_event = new Event<ConstructionSegment>(this);
		on_blocked_event = new Event<ConstructionSegment>(this);
		on_freed_event = new Event<ConstructionSegment>(this);

		on_spawned_wrapper = new EventWrapper<ConstructionSegment>(on_spawned_event);
		fixed_update_wrapper = new EventWrapper<ConstructionSegment>(fixed_update_event);
		on_destroyed_wrapper = new EventWrapper<ConstructionSegment>(on_destroyed_event);
		on_blocked_wrapper = new EventWrapper<ConstructionSegment>(on_blocked_event);
		on_freed_wrapper = new EventWrapper<ConstructionSegment>(on_freed_event);


		on_blocked_wrapper.AddAction("Segment_ChangeColor", e => {
			UpdateColor();
		});
		on_freed_wrapper.AddAction("Segment_ChangeColor", e => {
			UpdateColor();
		});

		damageable.on_hp_changed_wrapper.AddAction("Segment_ChangeColor", e => {
			UpdateColor();
		});
		damageable.on_killed_wrapper.AddAction("Destroy", e => {
			Destroy(gameObject);
		});

		unique_id = ObjectRegistry<ConstructionSegment>.Add(this);
	}

	private void Start() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius + max_distance, 1 << gameObject.layer);

		foreach (Collider2D collider in colliders) {
			ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment == null || !segment.started || segment == this) {
				continue;
			}

			attaching_object.AttachTo(segment.attaching_object);
		}

		started = true;

		on_spawned_event.RunEvent();
	}

	private void FixedUpdate() {
		if (!blocked) {
			fixed_update_event.RunEvent();
		}
	}

	void IBlockable.Block(ObjectBlocker blocker) {
		blocked_by.Add(blocker);
		if (blocker_count++ == 0) {
			blocked = true;
			on_blocked_event.RunEvent();
		}
	}

	void IBlockable.Free(ObjectBlocker blocker) {
		blocked_by.Remove(blocker);
		if (--blocker_count == 0) {
			blocked = false;
			on_freed_event.RunEvent();
		}
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

	public void Delete() {
		GameController.instance.AddZollars((int) (cost * damageable.hp / (float) damageable.max_hp));
		Destroy(gameObject);
	}

	private void OnDestroy() {
		foreach (ObjectBlocker blocker in blocked_by) {
			blocker.OnFreed(this);
		}
		blocked_by.Clear();

		ObjectRegistry<ConstructionSegment>.Remove(unique_id);

		on_destroyed_event.RunEvent();
	}
}
