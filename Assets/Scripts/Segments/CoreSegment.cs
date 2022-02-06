using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConstructionSegment))]
public class CoreSegment : MonoBehaviour {
	protected ConstructionSegment segment = null;

	private void Awake() {
		segment = GetComponent<ConstructionSegment>();
	}

	private void Start() {
		segment.damageable.on_killed_wrapper.AddAction("Core", OnKilled);
	}

	private void OnKilled(Damageable damageable) {
		GameController.instance.LoseGame();
	}
}
