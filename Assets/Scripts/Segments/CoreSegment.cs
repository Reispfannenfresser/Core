using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSegment : ConstructionSegment {

	protected override void Place() {
		base.Place();

		damageable.on_killed_wrapper.AddAction("EndGame", e => {
			GameController.instance.LoseGame();
		});
	}
}
