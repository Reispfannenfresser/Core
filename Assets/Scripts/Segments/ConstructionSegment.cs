using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSegment : MonoBehaviour {
	protected List<Joint2D> connectors = new List<Joint2D>();

	[SerializeField]
	protected SpriteRenderer[] sprite_renderers = new SpriteRenderer[0];
	public Rigidbody2D rb2d = null;

	public int max_hp = 100;
	public int hp = 100;
	public bool deletable = true;
	public bool meltable = true;
	public int value = 10;

	protected void Start() {
		OnPlaced();
		GameController.instance.AddSegment(this);
		rb2d = GetComponent<Rigidbody2D>();
	}

	private void LateUpdate() {
		if (transform.position.magnitude > 50) {
			Destroy();
		}
	}

	public void Damage(int amount) {
		hp -= amount;
		OnDamaged(amount);
		if (hp < 0) {
			Destroy();
		}
		UpdateColor();
	}

	public void Heal(int amount) {
		hp += amount;
		if (hp > max_hp) {
			hp = max_hp;
		}
		OnHealed(amount);
		UpdateColor();
	}

	private void UpdateColor() {
		foreach (SpriteRenderer renderer in sprite_renderers) {
			float gb_values = (float) hp / max_hp;
			float r_value = gb_values / 2 + 0.5f;
			float a_value = renderer.color.a;
			renderer.color = new Color(r_value, gb_values, gb_values, a_value);
		}
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
	}

	protected virtual void OnDamaged(int amount) {
	}

	protected virtual void OnHealed(int amount) {
	}

	protected virtual void OnDeleted() {
	}

	protected virtual void OnDestroyed() {
	}

	protected void OnDestroy() {
		foreach(Joint2D connector in connectors) {
			Destroy(connector);
		}
		GameController.instance.RemoveSegment(this);
	}
}
