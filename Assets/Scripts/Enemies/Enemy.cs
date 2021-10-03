using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	protected SpriteRenderer sprite_renderer = null;

	[SerializeField]
	protected int max_hp = 100;
	protected int hp;
	public bool deletable = true;

	protected void Start() {
		hp = max_hp;
		OnSpawned();
		GameController.instance.AddEnemy(this);
		sprite_renderer = GetComponent<SpriteRenderer>();
	}

	public void Damage(int amount) {
		hp -= amount;
		OnDamaged(amount);
		if (hp < 0) {
			Kill();
		}
		UpdateHPBar();
	}

	private void UpdateHPBar() {

	}

	public void Delete() {
		OnDeleted();
		Destroy(gameObject);
	}

	public void Kill() {
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
