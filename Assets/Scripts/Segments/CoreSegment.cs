using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSegment : ConstructionSegment {

	protected override void OnKilled() {
		GameController.instance.LoseGame();
	}
}
