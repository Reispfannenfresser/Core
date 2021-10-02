using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSegment : ConstructionSegment {
	float stopped = 0;

	[SerializeField]
	SpriteRenderer[] to_freeze = new SpriteRenderer[0];

	private void FixedUpdate() {
		if (stopped < 0) {
			stopped = 0;
			rb2d.freezeRotation = false;
			foreach (SpriteRenderer renderer in to_freeze) {
				renderer.color = Color.white;
			}
		} else {
			stopped -= Time.deltaTime;
		}
	}

	public void StopRotating(float time) {
		stopped += time;
		rb2d.freezeRotation = true;
		foreach (SpriteRenderer renderer in to_freeze) {
			renderer.color = new Color(0.2f, 0.5f, 0.75f);
		}
	}
}
