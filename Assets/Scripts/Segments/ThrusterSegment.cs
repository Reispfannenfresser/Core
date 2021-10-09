using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterSegment : ConstructionSegment {
	[SerializeField]
	float max_power = 3f;
	[SerializeField]
	float acceleration = 0.1f;
	float current_power = 0.0001f;

	[SerializeField]
	private SpriteRenderer fire = null;

	protected override void OnFixedUpdate() {
		if (current_power < max_power) {
			current_power += Time.deltaTime * acceleration;
			rb2d.mass = current_power;
			Color c = fire.color;
			c.a = Mathf.Min(current_power / max_power, 1);
			fire.color = c;
		}
		if (current_power > max_power) {
			current_power = max_power;
			rb2d.mass = current_power;
		}
	}
}
