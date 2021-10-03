using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSegment : ConstructionSegment {
	float stopped = 0f;

	private void FixedUpdate() {
		if (stopped < 0) {
			stopped = 0;
			rb2d.freezeRotation = false;
			foreach (SpriteRenderer renderer in sprite_renderers) {
				Color color = renderer.color;
				color.a = 1f;
				renderer.color = color;
			}
		} else {
			stopped -= Time.deltaTime;
		}
	}

	public void StopRotating(int time) {
		stopped += time;
		if (rb2d != null) {
			rb2d.freezeRotation = true;
		}
		foreach (SpriteRenderer renderer in sprite_renderers) {
			if (renderer == null) {
				continue;
			}
			Color color = renderer.color;
			color.a = 0.5f;
			renderer.color = color;
		}
	}

	protected override void OnDestroyed() {
		GameController.instance.LoseGame();
	}
}
