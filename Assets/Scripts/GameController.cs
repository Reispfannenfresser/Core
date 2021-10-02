using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	[SerializeField]
	SegmentPlacer segment_placer = null;
	[SerializeField]
	GameObject[] segments = new GameObject[0];

	static GameController instance = null;

	void Awake() {
		instance = this;
	}

	void Start() {
		instance.SetSegment(segments[0]);
	}

	public void SetSegment(GameObject to_place) {
		segment_placer.SetSegment(to_place);
	}
}
