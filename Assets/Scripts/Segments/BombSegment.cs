using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSegment : ConstructionSegment {
	[SerializeField]
	float stop_time = 3f;

	protected override void OnDestroyed() {
		foreach (Enemy enemy in GameController.instance.enemies) {
			enemy.Delete();
		}
		GameController.instance.StopCoreRotation(stop_time);
	}
}
