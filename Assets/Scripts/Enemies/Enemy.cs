using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public static HashSet<Enemy> all_enemies = new HashSet<Enemy>();
	public static int harmful_enemies = 0;
	public static int boss_enemies = 0;
	public static int total_enemies = 0;

	protected SpriteRenderer sprite_renderer = null;

	[SerializeField]
	protected int max_hp = 100;
	protected int hp;
	private bool is_dead = false;
	[SerializeField]
	public bool is_boss = false;

	[SerializeField]
	public bool is_harmful = true;

	protected void Awake() {
		hp = max_hp;
		OnSpawned();
		all_enemies.Add(this);
		if (is_harmful) {
			harmful_enemies++;
		}
		if (is_boss) {
			boss_enemies++;
		}
		total_enemies++;
		sprite_renderer = GetComponent<SpriteRenderer>();
	}

	public void Damage(int amount) {
		if (is_dead) {
			return;
		}

		hp -= amount;
		OnDamaged(amount);
		if (hp < 0) {
			Kill();
		}
		UpdateHPBar();
	}

	private void UpdateHPBar() {
		float gb_values = (float) hp / max_hp;
		float r_value = gb_values / 2 + 0.5f;
		sprite_renderer.color = new Color(r_value, gb_values, gb_values);
	}

	private void FixedUpdate() {
		OnFixedUpdate();
	}

	protected virtual void OnFixedUpdate() {

	}

	public void Kill() {
		if (is_dead) {
			return;
		}
		is_dead = true;
		OnKilled();
		Destroy(gameObject);
	}

	protected virtual void OnDestroy() {
		OnDestroyed();

		if (is_harmful) {
			harmful_enemies--;
		}
		if (is_boss) {
			boss_enemies--;
		}
		total_enemies--;

		all_enemies.Remove(this);
	}

	protected virtual void OnSpawned() {
	}

	protected virtual void OnDamaged(int amount) {
	}

	protected virtual void OnDestroyed() {
	}

	protected virtual void OnKilled() {
	}
}
