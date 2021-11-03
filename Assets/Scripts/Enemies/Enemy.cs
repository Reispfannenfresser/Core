using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable {
	public static HashSet<Enemy> all_enemies = new HashSet<Enemy>();
	public static int harmful_enemies = 0;
	public static int boss_enemies = 0;
	public static int total_enemies = 0;

	protected SpriteRenderer sprite_renderer = null;

	[SerializeField]
	protected int max_hp = 100;
	protected int hp;
	[SerializeField]
	public bool is_boss = false;

	[SerializeField]
	public bool is_harmful = true;

	protected void Awake() {
		Initialize();
	}

	protected virtual void Initialize() {
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
		hp = max_hp;
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
			Kill();
			hp = 0;
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

	private void UpdateColor() {
		float gb_values = (float) hp / max_hp;
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
