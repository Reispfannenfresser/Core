using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	[SerializeField]
	private int max_hp = 0;

	public static int harmful_enemies = 0;
	public static int boss_enemies = 0;
	public static int total_enemies = 0;

	protected SpriteRenderer[] sprite_renderers = null;

	public Damageable damageable {get; protected set;} = null;
	public int unique_id {get; private set;} = 0;

	[field: SerializeField]
	public bool is_boss {get; protected set;} = false;

	[field: SerializeField]
	public bool is_harmful {get; protected set;} = true;

	[field: SerializeField]
	public bool is_bomb_resistant {get; protected set;} = false;

	private Event<Enemy> on_spawned_event = null;
	private Event<Enemy> fixed_update_event = null;
	private Event<Enemy> on_destroyed_event = null;

	public EventWrapper<Enemy> on_spawned_wrapper = null;
	public EventWrapper<Enemy> fixed_update_wrapper = null;
	public EventWrapper<Enemy> on_destroyed_wrapper = null;

	protected void Awake() {
		damageable = new Damageable(max_hp);
		sprite_renderers = GetComponentsInChildren<SpriteRenderer>();

		total_enemies++;
		if (is_harmful) {
			harmful_enemies++;
		}
		if (is_boss) {
			boss_enemies++;
		}

		unique_id = ObjectRegistry<Enemy>.Add(this);

		on_spawned_event = new Event<Enemy>(this);
		fixed_update_event = new Event<Enemy>(this);
		on_destroyed_event = new Event<Enemy>(this);

		on_spawned_wrapper = new EventWrapper<Enemy>(on_spawned_event);
		fixed_update_wrapper = new EventWrapper<Enemy>(fixed_update_event);
		on_destroyed_wrapper = new EventWrapper<Enemy>(on_destroyed_event);

		damageable.on_hp_changed_wrapper.AddAction("Enemy_ChangeColor", damageable => {
			UpdateColor();
		});
		damageable.on_killed_wrapper.AddAction("Enemy_Destroy", damageable => {
			Destroy(gameObject);
		});
	}

	protected void Start() {
		on_spawned_event.RunEvent();
	}

	private void UpdateColor() {
		float gb_values = (float) damageable.hp / damageable.max_hp;
		float r_value = gb_values / 2 + 0.5f;

		Color c = new Color(r_value, gb_values, gb_values);
		foreach (SpriteRenderer r in sprite_renderers) {
			r.color = c;
		}
	}

	private void FixedUpdate() {
		fixed_update_event.RunEvent();
	}

	protected virtual void OnDestroy() {
		if (is_harmful) {
			harmful_enemies--;
		}
		if (is_boss) {
			boss_enemies--;
		}
		total_enemies--;

		ObjectRegistry<Enemy>.Remove(unique_id);

		on_destroyed_event.RunEvent();
	}
}
