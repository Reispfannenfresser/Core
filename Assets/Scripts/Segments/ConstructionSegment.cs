using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConstructionSegment : MonoBehaviour, IBlockable, IDamageable {
	public static HashSet<ConstructionSegment> all_segments = new HashSet<ConstructionSegment>();

	[SerializeField]
	protected SpriteRenderer[] sprite_renderers = new SpriteRenderer[0];
	[SerializeField]
	protected Rigidbody2D rb2d = null;

	[SerializeField]
	protected int max_hp = 100;
	protected int hp = 0;

	[SerializeField]
	protected bool deletable = true;
	[SerializeField]
	protected bool meltable = true;

	[SerializeField]
	protected int cost = 10;

	[SerializeField]
	protected float radius = 0.5f;

	int blocker_count = 0;
	protected bool blocked = false;

	public static float max_distance {get;} = 0.4f;
	public static float max_overlap {get;} = 0.1f;

	private AttachingObject attaching_object = null;

	private bool started = false;

	private void Awake() {
		Initialize();
	}

	protected virtual void Initialize() {
		rb2d = GetComponent<Rigidbody2D>();
		attaching_object = GetComponent<AttachingObject>();

		all_segments.Add(this);
		hp = max_hp;
	}

	private void Start() {
		Place();
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

	void IBlockable.OnBlocked(IObjectBlocker blocker) {
		if (blocker_count++ == 0) {
			Block();
		}
	}

	void IBlockable.OnFreed(IObjectBlocker blocker) {
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
				float gb_values = (float) hp / max_hp;
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

	public int GetHP() {
		return hp;
	}

	public int GetMaxHP() {
		return max_hp;
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

	void IDamageable.Damage(int amount) {
		Damage(amount);
	}

	void IDamageable.Heal(int amount) {
		Heal(amount);
	}

	void IDamageable.Kill() {
		Kill();
	}

	protected virtual void Damage(int amount) {
		hp -= amount;
		if (hp <= 0) {
			hp = 0;
			Kill();
		}
		UpdateColor();
	}

	protected virtual void Heal(int amount) {
		hp += amount;
		if (hp >= max_hp) {
			hp = max_hp;
		}
		UpdateColor();
	}

	public virtual void Kill() {
		Destroy(gameObject);
	}

	public virtual void Delete() {
		GameController.instance.AddZollars((int) (cost * hp / (float) max_hp));
		Destroy(gameObject);
	}

	protected void OnDestroy() {
		OnDestroyed();
	}

	protected virtual void OnDestroyed() {
		all_segments.Remove(this);
	}
}
