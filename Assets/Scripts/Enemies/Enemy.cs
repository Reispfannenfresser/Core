using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class Enemy : MonoBehaviour {
	public static HashSet<Enemy> all_enemies = new HashSet<Enemy>();
	public static int harmful_enemies = 0;
	public static int boss_enemies = 0;
	public static int total_enemies = 0;

	protected SpriteRenderer sprite_renderer = null;

	public Damageable damageable {get; protected set;} = null;

	[SerializeField]
	public bool is_boss = false;

	[SerializeField]
	public bool is_harmful = true;

	protected void Awake() {
		Initialize();
	}

	protected virtual void Initialize() {
		damageable = GetComponent<Damageable>();
		sprite_renderer = GetComponent<SpriteRenderer>();

		all_enemies.Add(this);
		if (is_harmful) {
			harmful_enemies++;
		}
		if (is_boss) {
			boss_enemies++;
		}
		total_enemies++;
	}

	protected void Start() {
		Spawn();
	}

	protected virtual void Spawn() {
		damageable.on_damaged_wrapper.AddAction("Enemy_ChangeColor", e => {
			UpdateColor();
		});
		damageable.on_healed_wrapper.AddAction("Enemy_ChangeColor", e => {
			UpdateColor();
		});
		damageable.on_killed_wrapper.AddAction("Destroy", e => {
			Destroy(gameObject);
		});
	}

	private void UpdateColor() {
		float gb_values = (float) damageable.hp / damageable.max_hp;
		float r_value = gb_values / 2 + 0.5f;
		sprite_renderer.color = new Color(r_value, gb_values, gb_values);
	}

	private void FixedUpdate() {
		OnFixedUpdate();
	}

	protected virtual void OnFixedUpdate() {

	}

	protected virtual void OnDestroy() {
		OnDestroyed();
	}

	protected virtual void OnDestroyed() {
		if (is_harmful) {
			harmful_enemies--;
		}
		if (is_boss) {
			boss_enemies--;
		}
		total_enemies--;

		all_enemies.Remove(this);
	}
}
