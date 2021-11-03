using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSegment : ConstructionSegment {

	public override void Kill() {
		base.Kill();
		GameController.instance.LoseGame();
	}
}
