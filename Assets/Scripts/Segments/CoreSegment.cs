using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSegment : MoneySegment {

	protected override void OnDestroyed() {
		GameController.instance.LoseGame();
	}
}
