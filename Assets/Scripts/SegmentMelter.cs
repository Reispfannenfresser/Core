using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMelter : MonoBehaviour {
	[SerializeField]
	int damage = 1;

	void OnTriggerStay2D(Collider2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null || !segment.meltable) {
			return;
		}

		segment.Damage(damage);
	}
}
