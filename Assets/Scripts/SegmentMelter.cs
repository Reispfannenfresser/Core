using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMelter : MonoBehaviour {
	[SerializeField]
	int damage = 1;

	Collider2D own_collider = null;

	[SerializeField]
	LayerMask construction = 0;

	ContactFilter2D contact_filter = new ContactFilter2D();
	Collider2D[] to_melt = new Collider2D[10];

	private void Start() {
		own_collider = GetComponent<Collider2D>();
		contact_filter.useLayerMask = true;
		contact_filter.layerMask = construction;
	}

	private void OnTriggerStay2D(Collider2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment == null || !segment.meltable) {
			return;
		}
		segment.damageable.Damage(damage);
	}
}
