using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	protected SpriteRenderer sprite_renderer = null;

	protected int max_hp = 100;
	protected int hp = 100;
	public bool deletable = true;

	protected void Start() {
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
		Debug.Log("Hi");
	}

	protected virtual void OnDamaged(int amount) {
		Debug.Log("Ouch");
	}

	protected virtual void OnDeleted() {
		Debug.Log("Bye");
	}

	protected virtual void OnKilled() {
		Debug.Log("ARGH!!!");
	}

	protected virtual void OnDestroy() {
		GameController.instance.RemoveEnemy(this);
	}
}
