using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingGraphic : MonoBehaviour {
	[SerializeField]
	int cooldown = 1;
	float current_cooldown = 0;

	[SerializeField]
	float speed = 1;

	Graphic[] graphics = new Graphic[0];

	void Start() {
		graphics = GetComponentsInChildren<Graphic>();
		current_cooldown = cooldown;
	}

	void Update() {
		current_cooldown -= Time.deltaTime;

		foreach (Graphic g in graphics) {
			Color c = g.color;
			c.a = Mathf.Max(0, current_cooldown / cooldown);
			g.color = c;
		}

		gameObject.transform.position += Time.deltaTime * Vector3.up * speed;

		if (current_cooldown <= 0) {
			Destroy(gameObject);
		}
	}
}
