using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	protected SpriteRenderer sprite_renderer = null;

	[SerializeField]
	protected int max_hp = 100;
	protected int hp;
	public bool deletable = true;
	public int prize = 5;
	private bool is_dead = false;

	protected void Start() {
		hp = max_hp;
		OnSpawned();
		GameController.instance.AddEnemy(this);
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

	public void Delete() {
		is_dead = true;
		OnDeleted();
		Destroy(gameObject);
	}

	public void Kill() {
		is_dead = true;
		GameController.instance.AddZollars(prize);
		OnKilled();
		Destroy(gameObject);
	}

	protected virtual void OnSpawned() {
	}

	protected virtual void OnDamaged(int amount) {
	}

	protected virtual void OnDeleted() {
	}

	protected virtual void OnKilled() {
	}

	protected virtual void OnDestroy() {
		GameController.instance.RemoveEnemy(this);
	}
}
