using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSegment : MonoBehaviour {
	protected List<Joint2D> connectors = new List<Joint2D>();

	protected int max_hp = 100;
	protected int hp = 100;
	public bool deletable = true;
	public bool meltable = true;

	protected void Start() {
		OnPlaced();
	}

	public void Damage(int amount) {
		hp -= amount;
		OnDamaged(amount);
		if (hp < 0) {
			Destroy();
		}
	}

	public void Heal(int amount) {
		hp += amount;
		if (hp > max_hp) {
			hp = max_hp;
		}
		OnHealed(amount);
	}

	public void AddConnector(Joint2D joint) {
		connectors.Add(joint);
	}

	public void Delete() {
		OnDeleted();
		Destroy(gameObject);
	}

	public void Destroy() {
		OnDestroyed();
		Destroy(gameObject);
	}

	protected virtual void OnPlaced() {
		Debug.Log("Hi");
	}

	protected virtual void OnDamaged(int amount) {
		Debug.Log("Ouch");
	}

	protected virtual void OnHealed(int amount) {
		Debug.Log("ahh");
	}

	protected virtual void OnDeleted() {
		Debug.Log("Bye");
	}

	protected virtual void OnDestroyed() {
		Debug.Log("ARGH!!!");
	}

	protected void OnDestroy() {
		foreach(Joint2D connector in connectors) {
			Destroy(connector);
		}
	}
}
