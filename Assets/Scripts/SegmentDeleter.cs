using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentDeleter : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null || !segment.deletable) {
			return;
		}

		segment.Delete();
	}
}
